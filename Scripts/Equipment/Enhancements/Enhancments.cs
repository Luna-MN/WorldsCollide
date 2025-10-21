using Godot;
using System;
[GlobalClass]
public partial class Enhancments : Node
{
    #region Helpers
        private void StatsOnMove(Character character, string StatName)
        {
            character.OnMove += () =>
            {
                character.characterStats.Recalculate(StatName);
            };
        }

        private void StatsOnHit(Character character, string StatName)
        {
            character.OnHit += (b, p, damage) =>
            {
                character.characterStats.Recalculate(StatName);
            };
        }
        private void StatsOnKill(Character character, string StatName)
        {
            character.OnKill += b =>
            {
                character.characterStats.Recalculate(StatName);
            };
        }
        private void StatsOnDeath(Character character, string StatName)
        {
            character.OnDeath += b =>
            {
                character.characterStats.Recalculate(StatName);
            };
        }
        private void StatsOnCrit(Character character, string StatName)
        {
            character.OnCrit += b =>
            {
                character.characterStats.Recalculate(StatName);
            };
        }

        private void StatsOnFire(Character character, string StatName)
        {
            character.OnFire += () =>
            {
                character.characterStats.Recalculate(StatName);
            };
        }
    #endregion
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
