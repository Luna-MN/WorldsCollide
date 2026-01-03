using Godot;
using System;

public partial class CharacterAgro : Resource
{
    public Character character;
    public float HPS;
    public float DPS;
    public float Damage;
    public float Healing;
    public float Agro;
    
    public float Time = 0;

    public void TimeCounter(double delta)
    {
        Time += (float)delta;
    }
    public void CalculatePS()
    {
        DPS = Damage / Time;
        HPS = Healing / Time;
    }
}
