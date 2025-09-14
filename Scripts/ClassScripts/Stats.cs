using Godot;
using System;
using System.Collections.Generic;
using Godot.NativeInterop;

[GlobalClass]
public partial class Stats : Resource
{
    [Export] 
    private Godot.Collections.Dictionary<string, Stat> stats { get; set; } = new();
    public Stats()
    {
    }

    public Stats(string startingStatsType)
    {
        if (startingStatsType == "player")
        {
            setValue("speed", 200f);
            setValue("currentHealth", 100f);
            setValue("maxHealth", 100f);
            setValue("armour", 0.0f);
            setValue("damageIncrease", 1.1f);
            setValue("itemFind", 1.0f);
            setValue("critChance", 0.5f);
            setValue("critDamage", 1.25f);
        }
    }
    
    public float this[string i]
    {
        set => setValue(i, value);
        get => getValue(i);
    }

    public float this[string i, string s]
    {
        get => s=="d"? getDisplayValue(i): getValue(i);
        set => setValue(i, value);
    }

    public void setValue(string name, float value)
    {
        if (!stats.TryGetValue(name, out var stat))
        {
            stats.Add(name, new Stat(name, value));
        }
        else
        {
            stat.Value = value;
        }
    }
    public float getValue(string name)
    {
        float value;
        if (!stats.TryGetValue(name, out var stat))
        {
            setValue(name, 0.0f);
            value = getValue(name);
        }
        else
        {
            value = stat.Value;
        }
        //GD.Print("getting " + name + ": " + value);
        
        return value;
    }
    public float getDisplayValue(string name)
    {
        return stats[name].DisplayValue;
    }

    public void addFunc(string statName, string funcName, Func<float, float> func, int priority)
    {
        if (stats.TryGetValue(statName, out var stat))
        {
            stat.addFunc(funcName, func, priority);
        }
    }

    public void removeFunc(string statName, string funcName)
    {
        if (stats.TryGetValue(statName, out var stat))
        {
            stat.removeFunc(funcName);
        }
    }
}
