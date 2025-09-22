using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

[GlobalClass]
public partial class Character : CharacterBody2D
{
    [Export]
    public bool IsDummy = false;
    public int ID;
    [Export]
    public PackedScene FloatingText;
    [Export] public InputSync inputSync;
    [Export] public MultiplayerSynchronizer PositionSync;
    [Export]
    protected string CharacterName = "Class1";
    [Export]
    protected AnimatedSprite2D WepSprite;

    [Export] public Node2D ShootPosition;
    public List<PrimaryWeapon> PrimaryEquipment = new();
    [ExportGroup("Equipment")]
    [Export]
    public EquipmentSlot[] EquipmentSlots;
    [Export] public Inventory inventory;
    [ExportSubgroup("Loot Drop")]
    [Export(PropertyHint.GroupEnable)] public bool DropLootOnDeath;
    [Export] public int Prestige = 1;
    [Export(PropertyHint.ResourceType)] public BaseEquipment[] DroppableEquipment;
    public List<int> PlayerIds = new();
    [Export]
    public PackedScene EquipmentSpawner;
    
    [ExportGroup("Skills")]
    [Export(PropertyHint.ResourceType)]
    public Skill[] skills;
    [Export(PropertyHint.ResourceType)]
    public Skill UltSkill;

    public List<int> selectedSkillIndexes = new List<int>() { 0, 1, 2, 3};

    #region Passive Variables
    public event Action<Node2D> OnHit;
    public event Action<Node2D> OnHitSkill;
    public event Action<Node2D> OnHitEquip;
    public event Action<Node2D> OnKill;
    public event Action<Node2D> OnKillSkill;
    public event Action<Node2D> OnKillEquip;
    public event Action<Node2D> OnDeath;
    public event Action<Node2D> OnDeathSkill;
    public event Action<Node2D> OnDeathEquip;
    public event Action OnStatCalc;
    public event Action OnStatCalcSkill;
    public event Action OnStatCalcEquip;
    protected  List<Timer> PassiveMoveTimers = new List<Timer>();
    protected  List<Timer> PassiveTimers = new List<Timer>();
    #endregion
    #region Player Properties
    
    [ExportGroup("Stats")]
    [Export]
    public Stats characterStats;
    [Export] public int Level;
    
    #region StatMirrors
            /*
            [Export] public float Speed
            {
                get => characterStats["Speed", "d"];
                set => characterStats["Speed"] = value;
            }

            [Export] public float CurrentHealth
            {
                get => characterStats["CurrentHealth", "d"];
                set => characterStats["CurrentHealth"] = value;
            }

            [Export] public float MaxHealth
            {
                get => characterStats["MaxHealth", "d"];
                set => characterStats["MaxHealth"] = value;
            }

            [Export] public float Armour
            {
                get => characterStats["Armour", "d"];
                set => characterStats["Armour"] = value;
            }

            [Export] public float DamageIncrease
            {
                get => characterStats["DamageIncrease", "d"];
                set => characterStats["DamageIncrease"] = value;
            }

            [Export] public float ItemFind
            {
                get => characterStats["ItemFind", "d"];
                set => characterStats["ItemFind"] = value;
            }

            [Export] public float CritChance
            {
                get => characterStats["CritChance", "d"];
                set => characterStats["CritChance"] = value;
            }

            [Export] public float CritDamage
            {
                get => characterStats["CritDamage", "d"];
                set => characterStats["CritDamage"] = value;
            }
            */

        #endregion
    #endregion
    public override void _EnterTree()
    {
        if (IsDummy)
        {
            return;
        }
        OnHit += b => OnHitSkill?.Invoke(b);
        OnHit += b => OnHitEquip?.Invoke(b);
        
        OnKill += b => OnKillSkill?.Invoke(b);
        OnKill += b => OnKillEquip?.Invoke(b);
        
        OnDeath += b => OnDeathSkill?.Invoke(b);
        OnDeath += b => OnDeathEquip?.Invoke(b);

        OnStatCalc += () => OnStatCalcSkill?.Invoke();
        OnStatCalc += () => OnStatCalcEquip?.Invoke();
        
        SetMultiplayerAuthority(Convert.ToInt32(Name));
        PositionSync.SetMultiplayerAuthority(1);
    }

    public override void _Ready()
    {
        if (DropLootOnDeath)
        {
            OnDeath += DropLoot;
        }
        if (IsDummy) return;
        SetSkills();
        equipAll();
        if (PrimaryEquipment.Count > 0)
        {
            WepSprite.SpriteFrames = PrimaryEquipment[0].SpriteFrames;
        }
    }
    public void equipAll()
    {
        PrimaryEquipment.Clear();
        var equipment = EquipmentSlots.Select(x => x.EquippedEquipment).Where(x => x != null).ToList();
        if (equipment.Count > 0)
        {
            foreach (var equip in equipment)
            {
                equip.OnEquip(this);
            }
        }
    }
    public void SetSkills()
    {
        OnHitSkill = null;
        OnDeathSkill = null;
        OnKillSkill = null;
        PassiveMoveTimers.ForEach(x => x.QueueFree());;
        PassiveMoveTimers.Clear();
        PassiveTimers.ForEach(x => x.QueueFree());;
        PassiveTimers.Clear();
        foreach (var skill in skills)
        {
            if(!selectedSkillIndexes.Contains(skills.ToList().IndexOf(skill))) continue;
            if (skill.IsPassive)
            {
                var info = GetType().GetMethod("Skill" + (skills.ToList().IndexOf(skill) + 1), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy );
                
                switch (skill.passiveType)
                {
                    case Skill.PassiveType.OnHit:
                        OnHit += _ => { info.Invoke(this, new object[] { }); };
                        break;
                    case Skill.PassiveType.OnKill:
                        OnKill += _ => { info.Invoke(this, new object[] { }); };
                        break;
                    case Skill.PassiveType.OnDeath:
                        OnDeath += _ => { info.Invoke(this, new object[] { }); };
                        break;
                    case Skill.PassiveType.OnMove:
                            SetUpTimer(skill, info, false, false);
                        break;
                    case Skill.PassiveType.OnTimerTimeout:
                            SetUpTimer(skill, info, true, false);
                        break;
                    case Skill.PassiveType.StatBoost:
                            var fieldInfo = GetType().GetField(skill.PassiveStat, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                            StatBoost(fieldInfo, skill.PassiveValue);
                        break;
                    case Skill.PassiveType.DynamicStatBoost:
                            OnStatCalcSkill += () => { info.Invoke(this, new object[] { }); };
                        break;
                }
            }
        }
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
    public void CallOnHit(Node2D body)
    {
        OnHit?.Invoke(body);
    }
    public void CallOnKill(Node2D body)
    {
        OnKill?.Invoke(body);
    }
    #endregion
    public override void _PhysicsProcess(double delta)
    {
        if(IsDummy) return;
        if (Multiplayer.IsServer())
        {
            Move((float)delta);
        }
        else if (Multiplayer.GetUniqueId() == Convert.ToInt32(Name) && inputSync != null)
        {
            inputSync.mousePosition = GetGlobalMousePosition();
        }
    }
    #region Inputs
    #region Equipment
    protected virtual void LeftClick()
    {
        if(PrimaryEquipment.Count == 0) return;
        if (PrimaryEquipment.Count == 1)
        {
            PrimaryEquipment[0].Left_Click();
        }
        else
        {
            var RandomEquipment = new Random().Next(0, PrimaryEquipment.Count);
            PrimaryEquipment[RandomEquipment].Left_Click();
        }
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
        // Godot: up is negative Y
        Vector2 input = inputSync.moveInput.Normalized();
        if (input != Vector2.Zero)
        {
            PassiveMoveTimers.ForEach(x => x.Start());
            Position += input.Normalized() * characterStats["speed"] * delta;
        }
        else
        {
            PassiveMoveTimers.ForEach(x => x.Stop());
        }
    }

    public void DamageText(float damage)
    {
        var text = FloatingText.Instantiate<FloatingText>();
        text.text.Text = damage.ToString();
        AddChild(text, true);
    }
    public virtual void TakeDamage(float damage, int attacker)
    {
        if (!Multiplayer.IsServer()) return;
        characterStats.stats["currentHealth"].Value -= damage;
        GD.Print(damage);
        GD.Print(characterStats.stats["currentHealth"].Value);
        if (characterStats["currentHealth"] <= 0)
        {
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
    public void AddPlayersToDropLoot(int attacker)
    {
        if(PlayerIds.Contains(attacker)) return;
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
        if (characterStats["currentHealth"] <= 0) return;
        if (characterStats["currentHealth"] + heal > characterStats["maxHealth"])
        {
            heal = characterStats["maxHealth"] - characterStats["currentHealth"];
        }
        characterStats["currentHealth"] += heal;
    }

}
