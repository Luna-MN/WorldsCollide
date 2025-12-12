using Godot;
using System;
using System.Collections.Generic;

public partial class Enemy : Character
{
    public Vector2 targetPos;
    public override void Move(float delta)
    {
        base.Move(delta);
        if (targetPos != Vector2.Zero && Position != targetPos)
        {
            // move the enemy to the target pos
            Position = Position.MoveToward(targetPos, stats[StatMaths.StatNum.speed] * delta);
            OnMoveToggle(true);
            if (Position == targetPos)
            {
                targetPos = Vector2.Zero;
            }
        }
        else
        {
            OnMoveToggle(false);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        Move((float)delta);
    }
}
