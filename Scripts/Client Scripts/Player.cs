using Godot;
using System;
[GlobalClass]
public partial class Player : Character
{
    // should be able to ignore this
    protected string ClassName = "Class1";
    #region Input Handling
    public Button TB1, TB2, TB3, TB4, TB5;
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
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (Input.IsActionJustPressed("skill_1"))
        {
            SetUI(TB1);
        }
        if (Input.IsActionPressed("skill_2"))
        {
            SetUI(TB2);
        }

        if (Input.IsActionJustPressed("skill_3"))
        {
            SetUI(TB3);
        }

        if (Input.IsActionJustPressed("skill_4"))
        {
            SetUI(TB4);
        }

        if (Input.IsActionJustPressed("skill_5"))
        {
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
            GD.Print("Clicked");
            if (Button.ButtonIndex == MouseButton.Left)
            {
                LeftClick();
            }
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
    
    #region skill stuff
    protected virtual void LeftClick()
    {
        GD.Print("Left Clicked");
        GameManager.ClassRpcs.RpcId(1, ClassName + "_LeftClick", Multiplayer.GetUniqueId());
    }
    
    #endregion
}
