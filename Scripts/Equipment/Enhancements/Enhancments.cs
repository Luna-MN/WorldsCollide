using Godot;
using System;
[GlobalClass]
public partial class Enhancments : Node
{
    public void One(Character character)
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
