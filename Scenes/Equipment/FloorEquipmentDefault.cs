using Godot;
using System;
using System.Linq;
using Godot.NativeInterop;

public partial class FloorEquipmentDefault : Node2D
{
    [Export]
    public AnimatedSprite2D sprite;
    [Export] public Area2D attractArea;
    [Export] public Area2D DestroyArea;
    public Color[] colors;
    private Color[] StoredColors;
    
    public BaseEquipment equipment;
    
    public Vector2 FinalPosition;
    public int Id;
    private bool moving;
    public override void _Ready()
    {
        sprite.Play();
        if (!Multiplayer.IsServer())
        {
            return;
        }
        // if the Id is not this client, loop through colors and make em' darker
        ((ShaderMaterial)sprite.Material).SetShaderParameter("output_palette_array", colors);
        attractArea.BodyEntered += OnAttractEntered;
        DestroyArea.BodyEntered += OnDestroyEntered;
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

        if (moving)
        {
            GlobalPosition = GlobalPosition.Lerp(FinalPosition, (float)delta * 10);
        }
    }
    public void ApplyColors()
    {
        if (colors != null && colors.Length > 0 && sprite != null && sprite.Material is ShaderMaterial material)
        {
            material.SetShaderParameter("output_palette_array", colors);
        }
    }

    private void OnAttractEntered(Node2D Body)
    {
        if (Body.Name != Id.ToString())
        {
            return;
        }
        moving = true;
        //move towards player
        FinalPosition = Body.GlobalPosition;
    }

    private void OnDestroyEntered(Node2D Body)
    {
        if (Body.Name != Id.ToString())
        {
            return;
        }
        // add item to player inventory (server)
        var currentEquipment = ServerManager.NodeDictionary[Id].inventory.AllEquipment;
        Array.Resize(ref currentEquipment, currentEquipment.Length + 1);
        currentEquipment[currentEquipment.Length - 1] = equipment;
        GD.Print(equipment.ResourceName);
        ServerManager.NodeDictionary[Id].inventory.AllEquipment = currentEquipment;
        //RPC that into the inv on the client
        var enhancmentIds = equipment.enhancements.Select(x => ServerManager.EquipmentRpcs.Enhancments.ToList().IndexOf(x)).ToArray();
        var equipmentId = ServerManager.EquipmentRpcs.equipment.ToList().FindIndex(x => x.ResourceName == equipment.ResourceName);
        ServerManager.EquipmentRpcs.RpcId(Id, "AddEquipmentToInv", enhancmentIds, equipmentId, equipment.ItemId, (int)equipment.Rarity);;
        QueueFree();
    }
}
