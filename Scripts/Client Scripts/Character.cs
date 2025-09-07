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
    
    // Should be able to ignore this class (see ClassRpc)
    [Export]
    protected string CharacterName = "Class1";
    [Export]
    public string PrimaryEquipment = "Gun";
    [Export(PropertyHint.ResourceType)]
    public Skill[] skills;

    public List<int> selectedSkillIndexes = new List<int>() { 0, 1, 2, 3};

    #region Passive Veriables
    protected  event Action<Node2D> OnHit;
    protected  event Action<Node2D> OnKill;
    protected  event Action<Node2D> OnDeath;
    protected  List<Timer> PassiveMoveTimers = new List<Timer>();
    protected  List<Timer> PassiveTimers = new List<Timer>();
    #endregion
    #region Player Properties
    
    [Export] public float Speed = 200f;
    [Export] public float Health = 100f;
    [Export] public float Armor = 0f;
    
    #endregion
    public override void _EnterTree()
    {
        if (IsDummy)
        {
            if (Multiplayer.IsServer())
            {

            }

            return;
        }
        SetMultiplayerAuthority(Convert.ToInt32(Name));
        PositionSync.SetMultiplayerAuthority(1);
    }

    public override void _Ready()
    {
        if (IsDummy) return;
        SetSkills();
    }
    public void SetSkills()
    {
        OnHit = null;
        PassiveMoveTimers.Clear();
        foreach (var skill in skills)
        {
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
                        var foundTimer = PassiveMoveTimers.FirstOrDefault(x => x.WaitTime == skill.TimerWaitTime);
                        if (foundTimer == null)
                        {
                            foundTimer = new Timer()
                            {
                                Autostart = false,
                                OneShot = false,
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
                        break;
                    case Skill.PassiveType.OnTimerTimeout:
                        var timer = PassiveMoveTimers.FirstOrDefault(x => x.WaitTime == skill.TimerWaitTime);
                        if (timer == null)
                        {
                            timer = new Timer()
                            {
                                Autostart = true,
                                OneShot = false,
                                WaitTime = skill.TimerWaitTime,
                            };
                        }
                        timer.Timeout += () =>
                        {
                            if (!Multiplayer.IsServer()) return;
                            info.Invoke(this, new object[] { });
                        };
                        if (!Multiplayer.IsServer())
                        {
                            AddChild(timer);
                            PassiveTimers.Add(timer);
                        }
                        break;
                    case Skill.PassiveType.StatBoost:
                        var stat = GetType().GetField(skill.PassiveStat);
                        stat?.SetValue(this, (float)stat?.GetValue(this)! + skill.PassiveValue);
                        break;
                }
            }
        }
    }
    protected Node ResolveRpcNode(Skill.RpcLocation loc)
    {
        return loc switch
        {
            Skill.RpcLocation.ClassRpc     => ServerManager.ClassRpcs,
            Skill.RpcLocation.EquipmentRpc => ServerManager.EquipmentRpcs,
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
        GameManager.EquipmentRpcs.RpcId(1, PrimaryEquipment + "_LeftClick", Convert.ToInt32(Name));
    }
    #endregion
    #region skill stuff
    protected virtual void Skill1()
    {
        GD.Print("Hit");
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
            Position += input.Normalized() * Speed * delta;
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
    public void TakeDamage(float damage, int attacker)
    {
        if (!Multiplayer.IsServer()) return;
        Health -= damage;
        if (Health <= 0)
        {
            if (this is Player p)
            {
                ServerManager.ServerRpcs.RpcId(1, "KillPlayer", p.Name);
            }
            OnDeath?.Invoke(this);
            ServerManager.NodeDictionary[attacker].CallOnKill(this);
            ServerManager.NodeDictionary.Remove(ID);
            if (IsMultiplayerAuthority())
            {
                QueueFree();
                ServerManager.ClientRpcs.Rpc("RemovePlayer", GetPath().ToString());
            }

        }
    }
    public void Heal(float heal)
    {
        if (Health <= 0) return;
        if (Health + heal > 100)
        {
            heal = 100 - Health;
        }
        Health += heal;
    }
}
