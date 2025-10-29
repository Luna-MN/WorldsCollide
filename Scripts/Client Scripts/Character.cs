using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;

[GlobalClass]
public partial class Character : CharacterBody2D
{
    [Export] public bool IsDummy = false;
    public bool isDead = false;
    public string ID = "-1";
    [Export] public PackedScene FloatingText;
    [Export] public InputSync inputSync;
    [Export] public MultiplayerSynchronizer PositionSync;

    [Export]
    // mostly for RPC this will be used to call the RPC of the skills on the server (CharacterName)_(SkillName) is the function that will be called
    protected string CharacterName = "Class1";

    [Export] protected AnimatedSprite2D WepSprite;
    [Export] protected AnimatedSprite2D OffHandSprite;
    [Export] protected AnimatedSprite2D Sprite;
    [Export] public Node2D ShootPosition;
    [Export] public Node2D OffHandShootPosition;
    public List<PrimaryWeapon> PrimaryEquipment = new();
    public List<PrimaryWeapon> SecondaryEquipment = new();
    [ExportGroup("Equipment")] [Export] public EquipmentSlot[] EquipmentSlots;
    [Export] public Inventory inventory;
    [Export] public Vector2 GunPos;
    [Export] public Vector2 OffHandPos;

    [ExportSubgroup("Loot Drop")] [Export(PropertyHint.GroupEnable)]
    public bool DropLootOnDeath;

    [Export] public int Prestige = 1;
    [Export(PropertyHint.ResourceType)] public BaseEquipment[] DroppableEquipment;
    public List<string> PlayerIds = new();
    [Export] public PackedScene EquipmentSpawner;
    public FloatingText RunningTotal;

    [ExportGroup("Skills")] [Export(PropertyHint.ResourceType)]
    public Skill[] skills;

    [Export(PropertyHint.ResourceType)] public Skill UltSkill;

    public List<int> selectedSkillIndexes = new List<int>() { 0, 1, 2, 3 };

    #region Passive Variables

    public event Action<Node2D, Projectile, float> OnHit;
    public event Action<Node2D, Projectile, float> OnHitSkill;
    public event Action<Node2D, Projectile, float> OnHitEquip;
    public event Action<Node2D> OnKill;
    public event Action<Node2D> OnKillSkill;
    public event Action<Node2D> OnKillEquip;
    public event Action<Node2D> OnDeath;
    public event Action<Node2D> OnDeathSkill;
    public event Action<Node2D> OnDeathEquip;
    public event Action OnStatCalc;
    public event Action OnStatCalcSkill;
    public event Action OnStatCalcEquip;
    public event Action<Node2D> OnCrit;
    public event Action<Node2D> OnCritSkill;
    public event Action<Node2D> OnCritEquip;
    public event Action OnMove;
    public event Action OnMoveSkill;
    public event Action OnMoveEquip;
    public event Action OnFire;
    public event Action OnFireSkill;
    public event Action OnFireEquip;
    protected List<Timer> PassiveMoveTimers = new List<Timer>();
    protected List<Timer> PassiveTimers = new List<Timer>();

    #endregion

    #region Player Properties

    [ExportGroup("Stats")] [Export] public Stats characterStats;
    [Export] public int Level;

    #endregion

    public override void _EnterTree()
    {
        if (IsDummy)
        {
            return;
        }

        OnHit += (b, p, f) => OnHitSkill?.Invoke(b, p, f);
        OnHit += (b, p, f) => OnHitEquip?.Invoke(b, p, f);

        OnKill += b => OnKillSkill?.Invoke(b);
        OnKill += b => OnKillEquip?.Invoke(b);

        OnDeath += b => OnDeathSkill?.Invoke(b);
        OnDeath += b => OnDeathEquip?.Invoke(b);

        OnStatCalc += () => OnStatCalcSkill?.Invoke();
        OnStatCalc += () => OnStatCalcEquip?.Invoke();

        OnCrit += b => OnCritSkill?.Invoke(b);
        OnCrit += b => OnCritEquip?.Invoke(b);

        OnFire += () => OnFireSkill?.Invoke();
        OnFire += () => OnFireEquip?.Invoke();
        if (this is Player p)
        {
            SetMultiplayerAuthority(Convert.ToInt32(Name));
        }
        else
        {
            SetMultiplayerAuthority(1);       
        }

        PositionSync.SetMultiplayerAuthority(1);
    }

    public override void _Ready()
    {
        if (!Multiplayer.IsServer() && ID == "-1")
        {
            ID = Name.ToString();
        }

        if (DropLootOnDeath)
        {
            OnDeath += DropLoot;
        }

        if (IsDummy) return;
        SetSkills();
        equipAll();
    }

    public void equipAll()
    {
        OnHitEquip = null;
        OnKillEquip = null;
        OnDeathEquip = null;
        OnStatCalcEquip = null;
        if (WepSprite.SpriteFrames != null)
        {
            WepSprite.Visible = false;
        }

        if (OffHandSprite.SpriteFrames != null)
        {
            OffHandSprite.Visible = false;
        }

        PrimaryEquipment.Clear();
        SecondaryEquipment.Clear();
        var equipment = EquipmentSlots.Select(x => x.EquippedEquipment).Where(x => x != null).ToList();
        if (equipment.Count > 0)
        {
            foreach (var equip in equipment)
            {
                equip?.OnEquip(this);
            }
        }

        if (PrimaryEquipment.Count > 0)
        {
            WepSprite.SpriteFrames = PrimaryEquipment[0].SpriteFrames;
            WepSprite.Position = GunPos;
            WepSprite.Visible = true;
            WepSprite.Scale = PrimaryEquipment[0].Scale;
        }


        if (SecondaryEquipment.Count > 0 && !SecondaryEquipment[0].TwoHandedMode)
        {
            OffHandSprite.SpriteFrames = SecondaryEquipment[0].SpriteFrames;
            OffHandSprite.Position = OffHandPos;
            OffHandSprite.Visible = true;
            OffHandSprite.Scale = SecondaryEquipment[0].Scale;
        }

    }

    public void SetSkills()
    {
        OnHitSkill = null;
        OnDeathSkill = null;
        OnKillSkill = null;
        PassiveMoveTimers.ForEach(x => x.QueueFree());
        PassiveMoveTimers.Clear();
        PassiveTimers.ForEach(x => x.QueueFree());
        PassiveTimers.Clear();
        foreach (var skill in skills)
        {
            if (!selectedSkillIndexes.Contains(skills.ToList().IndexOf(skill))) continue;
            if (skill.IsPassive)
            {
                var info = GetType().GetMethod("Skill" + (skills.ToList().IndexOf(skill) + 1), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);

                switch (skill.passiveType)
                {
                    case Skill.PassiveType.OnHit:
                        OnHitSkill += (b, p, f) => { info.Invoke(this, new object[] { b, p, f }); };
                        break;
                    case Skill.PassiveType.OnKill:
                        OnKillSkill += _ => { info.Invoke(this, new object[] { }); };
                        break;
                    case Skill.PassiveType.OnDeath:
                        OnDeathSkill += _ => { info.Invoke(this, new object[] { }); };
                        break;
                    case Skill.PassiveType.OnMove:
                        SetUpTimer(skill, info, false, false);
                        break;
                    case Skill.PassiveType.OnTimerTimeout:
                        SetUpTimer(skill, info, true, false);
                        break;
                    case Skill.PassiveType.StatBoost:
                        // var fieldInfo = GetType().GetField(skill.PassiveStat,
                        //     BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                        //     BindingFlags.FlattenHierarchy);
                        // StatBoost(fieldInfo, skill.PassiveValue);
                        break;
                    case Skill.PassiveType.DynamicStatBoost:
                        // OnStatCalcSkill += () => { info.Invoke(this, new object[] { }); };
                        
                        break;
                }
            }
        }
        equipAll();
    }

    protected void SetUpTimer(Skill skill, MethodInfo info, bool autostart, bool oneShot)
    {
        var foundTimer = PassiveMoveTimers.FirstOrDefault(x => x.WaitTime == skill.TimerWaitTime);
        if (foundTimer == null)
        {
            foundTimer = new Timer()
            {
                Autostart = autostart,
                OneShot = oneShot,
                WaitTime = skill.TimerWaitTime,
            };
        }
        foundTimer.Timeout += () =>
        {
            if (!Multiplayer.IsServer()) return;
            info.Invoke(this, new object[] { });
        };
        if (!Multiplayer.IsServer())
        {
            AddChild(foundTimer);
            PassiveMoveTimers.Add(foundTimer);
        }
    }

    public void StatBoost(FieldInfo stat, float value)
    {
        // i don't think this works anymore
        stat?.SetValue(this, (float)stat?.GetValue(this)! + value);
    }
    protected Node ResolveRpcNode(Skill.RpcLocation loc)
    {
        return loc switch
        {
            Skill.RpcLocation.ClassRpc     => ServerManager.ClassRpcs == null ? GameManager.ClassRpcs : ServerManager.ClassRpcs,
            Skill.RpcLocation.EquipmentRpc => ServerManager.EquipmentRpcs == null ? GameManager.EquipmentRpcs : ServerManager.EquipmentRpcs,
            Skill.RpcLocation.EnemyRpc     => throw(new NotImplementedException("Not implemented yet")),
            _ => null
        };
    }
    #region Passive Calls
    public void CallOnHit(Node2D body, Projectile proj, float damage)
    {
        OnHit?.Invoke(body, proj, damage);
    }
    public void CallOnKill(Node2D body)
    {
        OnKill?.Invoke(body);
    }

    public void CallOnCrit(Node2D body)
    {
        OnCrit?.Invoke(body);
    }

    public void CallOnFire()
    {
        OnFire?.Invoke();   
    }
    #endregion
    public override void _PhysicsProcess(double delta)
    {
        if(IsDummy) return;
        if (Multiplayer.IsServer() && this is Player p)
        {
            Move((float)delta);
        }
        else if (inputSync != null && Multiplayer.GetUniqueId() == Convert.ToInt32(Name))
        {
            inputSync.mousePosition = GetGlobalMousePosition();
        }
    }
    #region Inputs
    #region Equipment
    public virtual void LeftClick(PrimaryWeapon equip, Vector2 direction)
    {
        equip.Left_Click(ID, direction);
    }
    protected virtual void RightClick(PrimaryWeapon equip, Vector2 direction)
    {
        equip.Right_Click(ID, direction);
    }
    #endregion
    #region skill stuff
    protected virtual void Skill1()
    {
        var skill = skills[selectedSkillIndexes[0]];
        GD.Print(CharacterName + skill.RpcName);
        var rpcNode = ResolveRpcNode(skill.RpcCallLocation);
        rpcNode.RpcId(1, CharacterName + skill.RpcName, Convert.ToInt32(Name));
    }
    protected virtual void Skill2()
    {
        var skill = skills[selectedSkillIndexes[1]];
        GD.Print(CharacterName + skill.RpcName);
        var rpcNode = ResolveRpcNode(skill.RpcCallLocation);
        rpcNode.RpcId(1, CharacterName + skill.RpcName, Convert.ToInt32(Name));
    }

    protected virtual void Skill3()
    {
        var skill = skills[selectedSkillIndexes[2]];
        GD.Print(CharacterName + skill.RpcName);
        var rpcNode = ResolveRpcNode(skill.RpcCallLocation);
        rpcNode.RpcId(1, CharacterName + skill.RpcName, Convert.ToInt32(Name));
    }
    protected virtual void Skill4()
    {
        var skill = skills[selectedSkillIndexes[3]];
        GD.Print(CharacterName + skill.RpcName);
        var rpcNode = ResolveRpcNode(skill.RpcCallLocation);
        rpcNode.RpcId(1, CharacterName + skill.RpcName, Convert.ToInt32(Name));
    }
    protected virtual void Skill5()
    {
        // wacky ult stuff not touching rn
        GameManager.ClassRpcs.RpcId(1, CharacterName + "_Skill5", Convert.ToInt32(Name));
    }
    #endregion
    #endregion
    public void Move(float delta)
    {
        if (IsDummy) return;
        OnMove?.Invoke();
        // Godot: up is negative Y
        Vector2 input = inputSync.moveInput.Normalized();
        if (input != Vector2.Zero)
        {
            PassiveMoveTimers.ForEach(x => x.Start());
            Position += input.Normalized() * characterStats[StatMaths.StatNum.speed] * delta;
        }
        else
        {
            PassiveMoveTimers.ForEach(x => x.Stop());
        }
    }

    public void DamageText(float damage, float amountOfTimes = 1)
    {
        if (Multiplayer.IsServer())
        {
            GD.Print("Damage: " + damage);
        }
        string damageText = damage.ToString();
        if (amountOfTimes != 1)
        {
            damageText = damageText + "x" + amountOfTimes;       
        }
        ServerManager.ClientRpcs.Rpc("FloatingText", damage, amountOfTimes , GetPath().ToString(), new Color(1, 0.5f, 0.5f));
        var text = FloatingText.Instantiate<FloatingText>();
        text.Modulate = new Color(1, 0.5f, 0.5f);
        text.value = damage;
        text.multiplier = amountOfTimes;
        text.character = this;
        AddChild(text, true);
    }

    public void HealText(float heal)
    {
        var text = FloatingText.Instantiate<FloatingText>();
        text.Modulate = new Color(0.5f, 1, 0.5f);
        text.value = heal;
        AddChild(text, true);
    }
    public virtual void TakeDamage(float damage, string attacker)
    {
        if (!Multiplayer.IsServer()) return;
        damage *= characterStats[StatMaths.StatNum.armour];
        characterStats[StatMaths.StatNum.currentHealth] -= damage;
        if (characterStats[StatMaths.StatNum.currentHealth] <= 0 & !isDead)
        {
            isDead = true;
            OnDeath?.Invoke(this);
            ServerManager.NodeDictionary[attacker].CallOnKill(this);
            if (this is Player p)
            {
                ServerManager.ServerRpcs.RpcId(1, "KillPlayer", p.Name);
            }
            ServerManager.NodeDictionary.Remove(ID);
            if (IsMultiplayerAuthority())
            {
                ServerManager.ClientRpcs.Rpc("RemovePlayer", GetPath().ToString());
                QueueFree();
            }
        }
        if (DropLootOnDeath)
        {
            AddPlayersToDropLoot(attacker);
        }
    }
    public void AddPlayersToDropLoot(string attacker)
    {
        if(PlayerIds.Contains(attacker)) return;
        if (attacker.Contains('_'))
        {
            attacker = attacker.Split('_')[0];
        }
        PlayerIds.Add(attacker);
    }
    public void DropLoot(Node2D Killer)
    {
        var Gen = EquipmentSpawner.Instantiate<EquipmentGenerator>();
        Gen.Level = Level;
        Gen.Prestige = Prestige;
        Gen.CharacterIds = PlayerIds;
        Gen.GenerationEquipment = DroppableEquipment.ToList();
        Gen.GlobalPosition = GlobalPosition;
        ServerManager.spawner.AddChild(Gen);
    }
    public void Heal(float heal)
    {
        if (characterStats[StatMaths.StatNum.currentHealth] <= 0) return;
        if (characterStats[StatMaths.StatNum.currentHealth] + heal > characterStats[StatMaths.StatNum.maxHealth])
        {
            heal = characterStats[StatMaths.StatNum.maxHealth] - characterStats[StatMaths.StatNum.currentHealth];
        }
        characterStats[StatMaths.StatNum.currentHealth] += heal;
    }

}
