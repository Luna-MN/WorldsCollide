using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class BasicAiTest : Node2D
{
    [Export] private Area2D PlayerSightRange;
    [Export] private Area2D AgroRange;
    public List<Character> playersInSightRange = [];
    public List<Character> playersInAgro = [];
    public Enemy enemy;
    private readonly RandomNumberGenerator rng = new();

    [Export] public float RoamRadius { get; set; } = 300f; 
    [Export] public float RoamMinRadius { get; set; } = 0f;

    public override void _Ready()
    {
        PlayerSightRange.BodyEntered += OnPlayerSightBodyEntered;
        PlayerSightRange.BodyExited += OnPlayerSightBodyExited;
        AgroRange.BodyEntered += OnAgroBodyEntered;
        AgroRange.BodyExited += OnAgroBodyExited;
        enemy = (Enemy)GetParent();
        rng.Randomize();

    }

    public bool IsServer()
    {
        return Multiplayer.IsServer();    
    }
    public virtual float Roam()
    {
        // Uniform random point within a ring [RoamMinRadius, RoamRadius]
        float min = Mathf.Max(0f, RoamMinRadius);
        float max = Mathf.Max(min, RoamRadius);

        float angle = rng.RandfRange(0f, Mathf.Tau);
        float u = rng.Randf();
        float r = Mathf.Sqrt(Mathf.Lerp(min * min, max * max, u)); // uniform area distribution

        Vector2 offset = Vector2.FromAngle(angle) * r;
        enemy.targetPos = enemy.GlobalPosition + offset;
        
        float distance = offset.Length();
        GD.Print(offset.Length());
        return distance / enemy.stats[StatMaths.StatNum.speed];
    }

    public virtual float Idle()
    {
        return 5f;
    }

    public bool CheckPlayersInRange()
    {
        return playersInSightRange.Count > 0;
    }
    public bool CheckAgro()
    {
        return playersInAgro.Count > 0;
    }
    private void OnPlayerSightBodyEntered(Node body)
    {
        if ((body is not Player) && (body is not Minion)) return;
        playersInSightRange.Add((Character)body);
    }
    private void OnPlayerSightBodyExited(Node body)
    {
        if ((body is not Player) && (body is not Minion)) return;
        if (playersInSightRange.Contains((Character)body))
        {
            playersInSightRange.Remove((Character)body);
        }
    }
    private void OnAgroBodyEntered(Node body)
    {
        if (body is Player p)
        {
            playersInAgro.Add(p);
        }
        else if (body is Minion m)
        {
            playersInAgro.Add(m.summoner);
        }
    }
    private void OnAgroBodyExited(Node body)
    {
        if (body is Player p && playersInAgro.Contains(p))
        {
            playersInAgro.Remove(p);
        }
        else if (body is Minion m && playersInAgro.Contains(m.summoner))
        {
            playersInAgro.Remove(m.summoner);
        }
    }
}
