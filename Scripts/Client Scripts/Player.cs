using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

[GlobalClass]
public partial class Player : Character
{
    // Should be able to ignore this class (see ClassRpc)
    [Export]
    protected string ClassName = "Class1";
    [Export]
    public string PrimaryEquipment = "Gun";
    [Export(PropertyHint.ResourceType)]
    public Skill[] skills;
    
    public List<int> selectedSkillIndexes = new List<int>() { 0, 1, 2, 3};
    #region Input Handling
    public Button TB1, TB2, TB3, TB4, TB5;
    public event Action<Node2D> OnHit;
    public override void _Ready()
    {
        if (GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
        {   
            TB1 = GetNode<Button>("/root/Node2D2/Camera2D/CanvasLayer/Control/HBoxContainer/1");
            TB2 = GetNode<Button>("/root/Node2D2/Camera2D/CanvasLayer/Control/HBoxContainer/2");
            TB3 = GetNode<Button>("/root/Node2D2/Camera2D/CanvasLayer/Control/HBoxContainer/3");
            TB4 = GetNode<Button>("/root/Node2D2/Camera2D/CanvasLayer/Control/HBoxContainer/4");
            TB5 = GetNode<Button>("/root/Node2D2/Camera2D/CanvasLayer/Control/HBoxContainer/5");
        }
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
    public override void _UnhandledInput(InputEvent @event)
    {
        if (GetMultiplayerAuthority() != Multiplayer.GetUniqueId()) return;
        if (Input.IsActionJustPressed("skill_1"))
        {
            if(skills[selectedSkillIndexes[1]].IsPassive) return;
            Skill1();
            SetUI(TB1);
        }
        if (Input.IsActionPressed("skill_2"))
        {
            if (skills[selectedSkillIndexes[2]].IsPassive) return;
            Skill2();
            SetUI(TB2);
        }

        if (Input.IsActionJustPressed("skill_3"))
        {
            if (skills[selectedSkillIndexes[3]].IsPassive) return;
            Skill3();
            SetUI(TB3);
        }

        if (Input.IsActionJustPressed("skill_4"))
        {
            if (skills[selectedSkillIndexes[4]].IsPassive) return;
            Skill4();
            SetUI(TB4);
        }

        if (Input.IsActionJustPressed("skill_5"))
        {
            Skill5();
            SetUI(TB5);
        }
        if (Input.IsActionJustReleased("skill_1"))
        {
            ResetUI(TB1);
        }
        if (Input.IsActionJustReleased("skill_2"))
        {
            ResetUI(TB2);
        }

        if (Input.IsActionJustReleased("skill_3"))
        {
            ResetUI(TB3);
        }
        if (Input.IsActionJustReleased("skill_4"))
        {
            ResetUI(TB4);
        }
        if (Input.IsActionJustReleased("skill_5"))
        {
            ResetUI(TB5);
        }

        if (@event is InputEventMouseButton Button && Button.Pressed)
        {
            if (Button.ButtonIndex == MouseButton.Left)
            {
                LeftClick();
            }
        }
    }
    #region Passive Calls
        public void CallOnHit(Node2D body)
        {
            OnHit?.Invoke(body);
        }
    #endregion
    protected virtual void SetUI(Button button)
    {
        button.Modulate = new Color(0, 0, 1);
    }
    protected virtual void ResetUI(Button button)
    {
        button.Modulate = new Color(1, 1, 1);
    }
    private Node ResolveRpcNode(Skill.RpcLocation loc)
    {
        return loc switch
        {
            Skill.RpcLocation.ClassRpc     => ServerManager.ClassRpcs,
            Skill.RpcLocation.EquipmentRpc => ServerManager.EquipmentRpcs,
            Skill.RpcLocation.EnemyRpc     => throw(new NotImplementedException("Not implemented yet")),
            _ => null
        };
    }
    #endregion
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
                GD.Print(ClassName + skill.RpcName);
                var rpcNode = ResolveRpcNode(skill.RpcCallLocation);
                rpcNode.RpcId(1, ClassName + skill.RpcName, Convert.ToInt32(Name));
            }
            protected virtual void Skill2()
            {
                var skill = skills[selectedSkillIndexes[1]];
                GD.Print(ClassName + skill.RpcName);
                var rpcNode = ResolveRpcNode(skill.RpcCallLocation);
                rpcNode.RpcId(1, ClassName + skill.RpcName, Convert.ToInt32(Name));
            }

            protected virtual void Skill3()
            {
                var skill = skills[selectedSkillIndexes[2]];
                GD.Print(ClassName + skill.RpcName);
                var rpcNode = ResolveRpcNode(skill.RpcCallLocation);
                rpcNode.RpcId(1, ClassName + skill.RpcName, Convert.ToInt32(Name));
            }
            protected virtual void Skill4()
            {
                var skill = skills[selectedSkillIndexes[3]];
                GD.Print(ClassName + skill.RpcName);
                var rpcNode = ResolveRpcNode(skill.RpcCallLocation);
                rpcNode.RpcId(1, ClassName + skill.RpcName, Convert.ToInt32(Name));
            }
            protected virtual void Skill5()
            {
                // wacky ult stuff not touching rn
                GameManager.ClassRpcs.RpcId(1, ClassName + "_Skill5", Convert.ToInt32(Name));
            }
        #endregion
    #endregion
}
