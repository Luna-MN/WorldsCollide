using Godot;
using System;

public partial class Enemy : Character
{
    public Vector2 targetPos;
    private bool _isMoving;
    private Timer spawnDebounceTimer;
    private bool spawned = false;
    public override void _Ready()
    {
        base._Ready();
        spawnDebounceTimer = new Timer()
        {
            Autostart = true,
            OneShot = true,
            WaitTime = 0.5f
        };
        spawnDebounceTimer.Timeout += () =>
        {
            spawned = true;
            spawnDebounceTimer.QueueFree();
        };
        AddChild(spawnDebounceTimer);
    }

    public override void Move(float delta)
    {
        base.Move(delta);
        if(!Multiplayer.IsServer() || !spawned || IsDummy) return;
        // Move using physics — do not write Position each frame
        Vector2 toTarget = targetPos - GlobalPosition;
        float dist = toTarget.Length();
        float speed = stats[StatMaths.StatNum.speed];
        float step = speed * (float)delta;

        if (targetPos != Vector2.Zero && dist > 0.001f)
        {
            if (dist <= step)
            {
                GlobalPosition = targetPos;
                Velocity = Vector2.Zero;
                targetPos = Vector2.Zero;
                SetMoving(false);
            }
            else
            {
                Velocity = toTarget / dist * speed;
                SetMoving(true);
            }
        }
        else
        {
            Velocity = Vector2.Zero;
            SetMoving(false);
        }
        
        MoveAndSlide();

        // Flip based on actual motion
        if (Mathf.Abs(Velocity.X) > 0.01f)
            Sprite.FlipH = Velocity.X < 0f;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (Multiplayer.IsServer())
        {
            Move((float)delta);
        }
    }

    private void SetMoving(bool moving)
    {
        if (_isMoving == moving) return;
        _isMoving = moving;
        OnMoveToggle(moving);
    }
}