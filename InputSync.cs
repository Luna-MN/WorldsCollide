using Godot;
using System;

public partial class InputSync : MultiplayerSynchronizer
{
    [Export] public Vector2 moveInput;
    [Export] public Vector2 mousePosition;
    public override void _Ready()
    {
        if(GetMultiplayerAuthority() != Multiplayer.GetUniqueId())
        {
            SetPhysicsProcess(false);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        moveInput = Input.GetVector("move_left", "move_right", "move_up", "move_down");
    }
}
