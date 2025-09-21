using Godot;
using System;
using Godot.NativeInterop;

public partial class FloorEquipmentDefault : Node2D
{
    [Export]
    public AnimatedSprite2D sprite;
    [Export] public Area2D area;
    public Color[] colors;
    private Color[] StoredColors;
    
    public BaseEquipment equipment;
    
    public Vector2 FinalPosition;
    public int Id;
    private bool moving = true;
    public override void _Ready()
    {
        sprite.Play();
        if (!Multiplayer.IsServer())
        {
            return;
        }
        // if the Id is not this client, loop through colors and make em' darker
        ((ShaderMaterial)sprite.Material).SetShaderParameter("output_palette_array", colors);
        area.BodyEntered += OnBodyEntered;
        // Create a throwing animation with a tween
        var tween = CreateTween();
        
        // Calculate a mid-point for the arc (higher than both start and end)
        Vector2 startPos = GlobalPosition;
        Vector2 midPoint = new Vector2(
            (startPos.X + FinalPosition.X) / 2,  // Halfway between start and end X
            Math.Min(startPos.Y, FinalPosition.Y) - 75f  // Above both points
        );

        tween.TweenProperty(this, "global_position", midPoint, 0.3f)
            .SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.Out);
        tween.TweenProperty(this, "global_position", FinalPosition, 0.4f)
            .SetTrans(Tween.TransitionType.Bounce).SetEase(Tween.EaseType.Out);

        // When finished, we're no longer moving
        tween.Finished += () => { 
            moving = false;
            // Make sure we're exactly at the final position
            GlobalPosition = FinalPosition;
        };
    }


    public override void _Process(double delta)
    {
        if (colors != StoredColors)
        {
            StoredColors = colors;
            ApplyColors();
        }

    }
    public void ApplyColors()
    {
        if (colors != null && colors.Length > 0 && sprite != null && sprite.Material is ShaderMaterial material)
        {
            material.SetShaderParameter("output_palette_array", colors);
        }
    }

    public void OnBodyEntered(Node2D Body)
    {
        if (Body.Name != Id.ToString())
        {
            return;
        }
        //move towards player
    }
}
