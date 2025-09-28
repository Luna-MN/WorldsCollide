using Godot;
using System;

public partial class EquipmentSlotUI : Panel
{
    [Export]
    public Sprite2D IconSprite;

    public EquipmentUI equip;
    public bool Blocked;
    [Export]
    public Texture2D Icon
    {
        get => IconSprite.Texture;
        set => IconSprite.Texture = value;
    }
    
}
