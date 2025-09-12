using Godot;
using System;
[GlobalClass]
public partial class Class1_Shuriken : Gun_Bullet
{
    private AnimatedSprite2D sprite;
    public override void _Ready()
    {
        base._Ready();
        sprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
        sprite?.Play("default");
    }
}
