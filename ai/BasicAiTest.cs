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
    public override void _Ready()
    {
        PlayerSightRange.BodyEntered += OnPlayerSightBodyEntered;
        PlayerSightRange.BodyExited += OnPlayerSightBodyExited;
        AgroRange.BodyEntered += OnAgroBodyEntered;
        AgroRange.BodyExited += OnAgroBodyExited;
    }

    public virtual float Roam()
    {

        return 10f;
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
