using Godot;
using System;
using Godot.NativeInterop;

public partial class FloorEquipmentDefault : Node2D
{
    [Export]
    public AnimatedSprite2D sprite;

    [Export] public Color[] colors;
    
    [Export] public BaseEquipment equipment;
    public int Id;
    public override void _Ready()
    {
        // if the Id is not this client, loop through colors and make em' darker
        ((ShaderMaterial)sprite.Material).SetShaderParameter("output_palette_array", colors);
    }
}
