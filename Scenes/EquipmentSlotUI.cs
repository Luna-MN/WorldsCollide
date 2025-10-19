using Godot;
using System;

public partial class EquipmentSlotUI : Panel
{
    [Export]
    public TextureRect IconSprite;

    public EquipmentUI equip;
    public bool Blocked;
    [Export]
    public Texture2D Icon
    {
        get => IconSprite.Texture;
        set => IconSprite.Texture = value;
    }
    
}
