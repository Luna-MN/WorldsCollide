using Godot;
using System;
[GlobalClass]
public partial class Character : CharacterBody2D
{
    public int ID;
    [Export] public float Speed = 200f;
    [Export] public InputSync inputSync;
    [Export] public MultiplayerSynchronizer PositionSync;
    public bool IsPlayer { get; set; }
    public override void _EnterTree()
    {
        SetMultiplayerAuthority(Convert.ToInt32(Name));
        PositionSync.SetMultiplayerAuthority(1);
    }

    public override void _Ready()
    {
    }
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
