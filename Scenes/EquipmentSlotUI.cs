using Godot;
using System;
[Tool]
public partial class EquipmentSlotUI : Panel
{
    [Export]
    public Sprite2D IconSprite;

    [Export]
    public Texture2D Icon
    {
        get => IconSprite.Texture;
        set => IconSprite.Texture = value;
    }
    
}
