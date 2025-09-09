using Godot;
using System;

public partial class One : BaseEnhancement
{

    public override void Enhance(Character character)
    {
        character.OnHitEquip += (Node2D b) =>
        {
            GD.Print("One");
        };
    }
}
