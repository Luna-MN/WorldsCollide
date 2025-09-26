using Godot;
using System;
[GlobalClass]
public partial class One : BaseEnhancement
{

    public override void Enhance(Character character)
    {
        character.OnHitEquip += (Node2D b, Projectile p, float damage) =>
        {
            if (b is Character c)
            {
                p.amountOfTimes += 1;
                c.TakeDamage(damage, Convert.ToInt32(character.Name));
            }
        };
    }
}
