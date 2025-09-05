using Godot;
using System;
[GlobalClass]
public partial class Player : Character
{
    // should be able to ignore this

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
    }

    public virtual void SetUI(Button button)
    {

    }
    #endregion
    
    
}
