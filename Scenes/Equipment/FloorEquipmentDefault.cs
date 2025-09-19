using Godot;
using System;
using Godot.NativeInterop;

public partial class FloorEquipmentDefault : Node2D
{
    [Export]
    public AnimatedSprite2D sprite;

    [Export] public Color[] colors;
    
    [Export] public BaseEquipment equipment;
    public override void _Ready()
    {
        ((ShaderMaterial)sprite.Material).SetShaderParameter("output_palette_array", colors);
    }
}
