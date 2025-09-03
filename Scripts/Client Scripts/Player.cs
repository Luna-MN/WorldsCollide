using Godot;
using System;

public partial class Player : Node2D
{
    public long ID;
    public Movement Movement;
    public bool IsPlayer { get; set; }
    public override void _Ready()
    {
        Movement = GetNode<Movement>("Movement");    
    }
}
