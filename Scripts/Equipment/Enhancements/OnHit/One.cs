using Godot;
using System;
[GlobalClass]
public partial class One : BaseEnhancement
{

    public override void Enhance(Character character)
    {
        character.OnHitEquip += (Node2D b, float damage) =>
        {
            if (b is Character c)
            {
                c.DamageText(damage);
                c.TakeDamage(damage, Convert.ToInt32(character.Name));
            }
        };
    }
}
