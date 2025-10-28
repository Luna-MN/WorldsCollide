using Godot;
using System;

public partial class FloorFire : Sprite2D
{
    public override void _Ready()
    {
        GetParent().MoveChild(this, 0);
    }
}
