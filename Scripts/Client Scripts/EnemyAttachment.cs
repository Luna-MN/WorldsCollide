using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class EnemyAttachment : Node2D
{
    public Vector2 targetPos;
    private bool _isMoving;
    private Timer spawnDebounceTimer;
    private bool spawned = false;
    private Character enemy;
    public AgroManager agroManager = null;
    public override void _Ready()
    {
        enemy = (Character)GetParent();
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

    public void Move(float delta)
    {
        if(!Multiplayer.IsServer() || !spawned || enemy.IsDummy) return;
        // Move using physics — do not write Position each frame
        Vector2 toTarget = targetPos - GlobalPosition;
        float dist = toTarget.Length();
        float speed = enemy.stats[StatMaths.StatNum.speed];
        float step = speed * (float)delta;

        if (targetPos != Vector2.Zero && dist > 0.001f)
        {
            if (dist <= step)
            {
                GlobalPosition = targetPos;
                enemy.Velocity = Vector2.Zero;
                targetPos = Vector2.Zero;
                SetMoving(false);
            }
            else
            {
                enemy.Velocity = toTarget / dist * speed;
                SetMoving(true);
            }
        }
        else
        {
            enemy.Velocity = Vector2.Zero;
            SetMoving(false);
        }
        
        enemy.MoveAndSlide();

        if (enemy.Velocity.X > 0)
        {
            enemy.Sprite.FlipH = false;
        }
        else if (enemy.Velocity.X < 0)
        {
            enemy.Sprite.FlipH = true;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (Multiplayer.IsServer())
        {
            Move((float)delta);
        }
    }

    private void JoinAgro(Character character)
    {
        if(agroManager == null)
        {
            agroManager = new AgroManager();
            AddChild(agroManager);
        }
        if(agroManager.charactersAgros.Any(x => x.character == character)) return;
        agroManager.charactersAgros.Add(new CharacterAgro()
        {
            character = character,
        });
        
    }
    private void SetMoving(bool moving)
    {
        if (_isMoving == moving) return;
        _isMoving = moving;
        enemy.OnMoveToggle(moving);
    }
}
