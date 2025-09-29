using Godot;
using System;
[GlobalClass]
public partial class One : BaseEnhancement
{

    public override void Enhance(Character character)
    {
        character.OnHitEquip += ( b, p, damage) =>
        {
            if (b is Character c)
            {
                p.amountOfTimes += 1;
            }
        };
    }
}
