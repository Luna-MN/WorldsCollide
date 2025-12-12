using Godot;
using System;
[GlobalClass]
public partial class Enhancments : Node
{
    #region Helpers
        private void StatsOnMove(Character character, StatMaths.StatNum StatName)
        {
            character.OnMoveEquip += () =>
            {
                character.stats.Recalculate(StatName);
            };
        }

        private void StatsOnHit(Character character, StatMaths.StatNum StatName)
        {
            character.OnHitEquip += (b, p, damage) =>
            {
                character.stats.Recalculate(StatName);
            };
        }
        private void StatsOnKill(Character character, StatMaths.StatNum StatName)
        {
            character.OnKillEquip += b =>
            {
                character.stats.Recalculate(StatName);
            };
        }
        private void StatsOnDeath(Character character, StatMaths.StatNum StatName)
        {
            character.OnDeathEquip += b =>
            {
                character.stats.Recalculate(StatName);
            };
        }
        private void StatsOnCrit(Character character, StatMaths.StatNum StatName)
        {
            character.OnCritEquip += b =>
            {
                character.stats.Recalculate(StatName);
            };
        }

        private void StatsOnFire(Character character, StatMaths.StatNum StatName)
        {
            character.OnFireEquip += () =>
            {
                character.stats.Recalculate(StatName);
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
