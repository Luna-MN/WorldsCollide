using Godot;
using System;
using Godot.NativeInterop;

public partial class FloorEquipmentDefault : Node2D
{
    [Export]
    public AnimatedSprite2D sprite;

    [Export] public Color[] colors;
    
    [Export] public BaseEquipment equipment;
    
    public Vector2 FinalPosition;
    public int Id;
    private bool moving = true;
    public override void _Ready()
    {
        // if the Id is not this client, loop through colors and make em' darker
        ((ShaderMaterial)sprite.Material).SetShaderParameter("output_palette_array", colors);
    }
    public override void _Process(double delta)
    {
        if(!Multiplayer.IsServer()) return;
        if (moving)
        {
            GlobalPosition = GlobalPosition.Lerp(FinalPosition, (float)delta);
            if (GlobalPosition.DistanceTo(FinalPosition) < 0.1f)
            {
                moving = false;
            }
        }

    }
}
