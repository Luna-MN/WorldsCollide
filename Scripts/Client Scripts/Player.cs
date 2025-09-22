using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

[GlobalClass]
public partial class Player : Character
{
    #region Input Handling
    public SkillOnScreen TB1, TB2, TB3, TB4, TB5;
    
    public override void _Ready()
    {
        if (GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
        {   
            GameManager.player = this;
            TB1 = GetNode<SkillOnScreen>("/root/Node2D2/Camera2D/CanvasLayer/Control/HBoxContainer/1");
            TB1.SetIcon();
            TB2 = GetNode<SkillOnScreen>("/root/Node2D2/Camera2D/CanvasLayer/Control/HBoxContainer/2");
            TB2.SetIcon();
            TB3 = GetNode<SkillOnScreen>("/root/Node2D2/Camera2D/CanvasLayer/Control/HBoxContainer/3");
            TB3.SetIcon();
            TB4 = GetNode<SkillOnScreen>("/root/Node2D2/Camera2D/CanvasLayer/Control/HBoxContainer/4");
            TB4.SetIcon();
            TB5 = GetNode<SkillOnScreen>("/root/Node2D2/Camera2D/CanvasLayer/Control/HBoxContainer/5");
            TB5.SetIcon(true);

        }
        base._Ready();
    }
    public override void _UnhandledInput(InputEvent @event)
    {
        if (GetMultiplayerAuthority() != Multiplayer.GetUniqueId()) return;
        if (Input.IsActionJustPressed("skill_1"))
        {
            if(skills[selectedSkillIndexes[0]].IsPassive) return;
            Skill1();
            SetUI(TB1);
        }
        if (Input.IsActionJustPressed("skill_2"))
        {
            if (skills[selectedSkillIndexes[1]].IsPassive) return;
            Skill2();
            SetUI(TB2);
        }

        if (Input.IsActionJustPressed("skill_3"))
        {
            GD.Print("3 Pressed");
            if (skills[selectedSkillIndexes[2]].IsPassive)
            {
                GD.Print("3 is passive");
                return;
            }
            Skill3();
            SetUI(TB3);
        }

        if (Input.IsActionJustPressed("skill_4"))
        {
            if (skills[selectedSkillIndexes[3]].IsPassive) return;
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

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (GetMultiplayerAuthority() != Multiplayer.GetUniqueId()) return;
        WepSprite.LookAt(inputSync.mousePosition);
        if (WepSprite.RotationDegrees > 360)
        {
            WepSprite.RotationDegrees -= 360;
        }
        if (WepSprite.RotationDegrees < 0)
        {
            WepSprite.RotationDegrees += 360;       
        }

        if (WepSprite.RotationDegrees > 90 && WepSprite.RotationDegrees < 270)
        {
            WepSprite.FlipV = true;
            ShootPosition.Position = new Vector2(14.857f, 3.429f);      
        }
        else
        {
            WepSprite.FlipV = false;       
            ShootPosition.Position = new Vector2(14.857f, -3.429f);       
        }
    }

    protected virtual void SetUI(Button button)
    {
        button.Modulate = new Color(0, 0, 1);
    }
    protected virtual void ResetUI(Button button)
    {
        button.Modulate = new Color(1, 1, 1);
    }
    #endregion

}
