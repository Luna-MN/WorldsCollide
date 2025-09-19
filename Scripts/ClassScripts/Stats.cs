using Godot;
using System;
using System.Collections.Generic;
using Godot.NativeInterop;

[GlobalClass]
public partial class Stats : Resource
{
    //dictionary of all name & stat classes
    [Export] 
    public Godot.Collections.Dictionary<string, Stat> stats { get; set; } = new();
    public Stats()
    {
    }
    //getter setter for stats
    public float this[string i]
    {
        set => setValue(i, value);
        get => getValue(i);
    }
    //getter setter for display value stats
    public float this[string i, string s]
    {
        get => s.ToLower()=="d"? getDisplayValue(i): getValue(i);
        set => setValue(i, value);
    }
    //set adds if nonexistent
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
    //get adds if nonexistent - default value = 0 could be edited by vaildation functions
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
    //display value getter
    public float getDisplayValue(string name)
    {
        return stats[name].DisplayValue;
    }
    //add a function to a stat's calculation order
    public void addFunc(string statName, string funcName, Func<float, float> func, int priority)
    {
        if (stats.TryGetValue(statName, out var stat))
        {
            stat.addFunc(funcName, func, priority);
        }
    }
    //remove a function
    public void removeFunc(string statName, string funcName)
    {
        if (stats.TryGetValue(statName, out var stat))
        {
            stat.removeFunc(funcName);
        }
    }
}
