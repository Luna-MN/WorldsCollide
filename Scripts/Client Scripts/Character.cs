using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

[GlobalClass]
public partial class Character : CharacterBody2D
{
    public int ID;
    [Export] public float Speed = 200f;
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
    public event Action<Node2D> OnHit;
    public bool IsPlayer { get; set; }
    public override void _EnterTree()
    {
        SetMultiplayerAuthority(Convert.ToInt32(Name));
        PositionSync.SetMultiplayerAuthority(1);
    }

    public override void _Ready()
    {
        foreach (var skill in skills)
        {
            if (skill.IsPassive)
            {
                switch (skill.passiveType)
                {
                    case Skill.PassiveType.OnHit:
                        GD.Print("Skill" + (skills.ToList().IndexOf(skill) + 1));
                        var info = GetType().GetMethod("Skill" + (skills.ToList().IndexOf(skill) + 1), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy );
                        GD.Print(info);
                        OnHit += (Node2D body) => { info.Invoke(this, new object[] { }); };
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
    #endregion
    public override void _PhysicsProcess(double delta)
    {
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
        // Godot: up is negative Y
        Vector2 input = inputSync.moveInput.Normalized();
        if (input != Vector2.Zero)
        {
            Position += input.Normalized() * Speed * delta;
        }
    }
}
