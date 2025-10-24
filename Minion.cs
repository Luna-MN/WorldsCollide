using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Minion : Character
{
    [ExportGroup("Traps")]
    [Export(PropertyHint.GroupEnable)]
    public bool Traps;
    [ExportGroup("Little")]
    [Export(PropertyHint.GroupEnable)]
    public bool Little;
    [ExportGroup("Big")]
    [Export(PropertyHint.GroupEnable)]
    public bool Big;
    [ExportGroup("Turrets")]
    [Export(PropertyHint.GroupEnable)]
    public bool Turrets;
    [Export]
    private Area2D TurretEnemyAttackRange;
    [Export]
    public bool TurretAttacking;
    [Export]
    public bool TurretHealing;
    public List<Enemy> EnemyBodysIn = [];
    public List<Character> Player_MinionsBodysIn = [];
    public Character ClosestCharacter = null;
    [Export]
    public Vector2 TargetPosition;
    protected bool LockedOn;
    [ExportGroup("Mirrors")]
    [Export(PropertyHint.GroupEnable)]
    public bool Mirrors;

    public override void _Ready()
    {
        if (Turrets)
        {
            TurretEnemyAttackRange.BodyEntered += TurretRangeEnter;
            TurretEnemyAttackRange.BodyExited += TurretRangeLeave;
            OnKill += node2D =>
            {
                if (node2D == ClosestCharacter)
                {
                    if (EnemyBodysIn.Contains(ClosestCharacter))
                    {
                        EnemyBodysIn.Remove((Enemy)ClosestCharacter);
                    }

                    if (Player_MinionsBodysIn.Contains(ClosestCharacter))
                    {
                        Player_MinionsBodysIn.Remove(ClosestCharacter);
                    }
                    ClosestCharacter = null;
                    TargetPosition = Vector2.Zero;
                }
            };
        }
        base._Ready();

    }

    public override void _Process(double delta)
    {
        if (!Multiplayer.IsServer())
        {
            return;
        } 
        base._Process(delta);
        if (Turrets)
        {
            TurretProcessLogic();
        }
    }

    #region Turrets
        public virtual void TurretRangeEnter(Node2D body)
        {
            if (body is Character c)
            {
                if (TurretAttacking)
                {
                    if (c is Enemy e)
                    {
                        EnemyBodysIn.Add(e);
                    }
                }
                if (TurretHealing)
                {
                    if (c is Player or Minion)
                    {
                        Player_MinionsBodysIn.Add(c);
                    }
                }
            }
        }
        public virtual void TurretRangeLeave(Node2D body)
        {
            if (body is Character c)
            {
                if (c is Enemy e)
                {
                    if (EnemyBodysIn.Contains(e))
                    {
                        EnemyBodysIn.Remove(e);
                    }
                }
                if (c is Player or Minion)
                {
                    if (Player_MinionsBodysIn.Contains(c))
                    {
                        Player_MinionsBodysIn.Remove(c);
                    }
                }
            }
        }
        public virtual void TurretProcessLogic()
        {
            if (EnemyBodysIn.Count() != 0)
            {
                ClosestCharacter = GetClosest(EnemyBodysIn.ConvertAll(x => x as Character));
            }

            if (Player_MinionsBodysIn.Count != 0)
            {
                var ClosestPlayer = GetClosest(Player_MinionsBodysIn.ConvertAll(x => x as Character));
                if (ClosestPlayer.GlobalPosition.DistanceTo(GlobalPosition) < ClosestCharacter?.GlobalPosition.DistanceTo(GlobalPosition))
                {
                    ClosestCharacter = ClosestPlayer;
                }
            }

            if (ClosestCharacter != null)
            {
                // this aiming won't be good enough for moving targets
                TargetPosition = ClosestCharacter.GlobalPosition;
                WepSprite.LookAt(ClosestCharacter.GlobalPosition);
                LockedOn = true;
            }
            else
            {
                LockedOn = false;
            }

            if (WepSprite.RotationDegrees > 360)
            {
                WepSprite.RotationDegrees -= 360;
            }
            else if (WepSprite.RotationDegrees < 0)
            {
                WepSprite.RotationDegrees += 360;
            }
        }
        
    #endregion
    public Character GetClosest(List<Character> characters)
    {
        var ClosestPos = Vector2.Inf;
        var ClosestChar = characters[0];
        foreach (var character in characters)
        {
            if (character.GlobalPosition.DistanceTo(GlobalPosition) < ClosestPos.DistanceTo(GlobalPosition))
            {
                ClosestPos = character.GlobalPosition;
                ClosestChar = character;
            }
        }
        return ClosestChar;
    }
}
