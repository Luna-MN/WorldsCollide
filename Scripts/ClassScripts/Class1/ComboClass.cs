using Godot;
using System;
using System.Collections.Generic;

public partial class ComboClass : Player
{
    public int currentCombo = 0;
    public Timer comboTimer = new Timer()
    {
        OneShot = true,
        WaitTime = 0.05f
    };
    [Export] public float comboLossTime = 2.0f;
    public List<string> hitList = new();

    public override void _Ready()
    {
        base._Ready();
        AddChild(comboTimer);
        comboTimer.Timeout += () =>
        {
            GD.Print(currentCombo);
            hitList.Clear();
            currentCombo = 0;
        };
        OnHit += (b, f) =>
        {
            if (b is Character && !hitList.Contains(b.Name))
            {
                hitList.Add(b.Name);
            }
            currentCombo++;
            comboTimer.Start(comboLossTime);
        };
        OnKill += b =>
        {
            if (b is Character)
            {
                hitList.Remove(b.Name);
            }
        };
    }
}
