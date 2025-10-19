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
    [Export]
    public StatMaths.OriginType _origin;
    
    public Stats()
    {
        var s = (Script)this.GetLocalScene()?.GetScript().AsGodotObject();
        GD.Print(s?.GetBaseScript());
    }
    /// <summary>
    /// Gets The calculated value
    /// Sets the value
    /// </summary>
    /// <param name="i">String name of stat</param>
    public float this[string i]
    {
        set => SetValue(i, value);
        get => GetCalcValue(i);
    }
    /// <summary>
    /// Gets the display value or the normal value
    /// Sets the value
    /// </summary>
    /// <param name="i">String name of stat</param>
    /// <param name="s">D for display value anything else for other value</param>
    public float this[string i, string s]
    {
        get => s.ToLower()=="d"? GetDisplayValue(i): GetCalcValue(i);
        set => SetValue(i, value);
    }
    /// <summary>
    /// Sets Value of a stat, creates it if non-existent
    /// </summary>
    /// <param name="name">Stat Name</param>
    /// <param name="value">Value to set it to</param>
    public void SetValue(string name, float value)
    {
        if (!stats.TryGetValue(name, out var stat))
        {
            stats.Add(name, new Stat(name, value));
        }
        else
        {
            stat.CalcValue = value;
        }
    }
    /// <summary>
    /// Get Calculated Value of a stat
    /// </summary>
    /// <param name="name">Stat Name</param>
    /// <returns>Calculation Value of a stat</returns>
    public float GetCalcValue(string name)
    {
        float value;
        if (!stats.TryGetValue(name, out var stat))
        {
            SetValue(name, 0.0f);
            value = GetCalcValue(name);
        }
        else
        {
            value = stat.CalcValue;
        }
        //GD.Print("getting " + name + ": " + value);
        
        return value;
    }
    /// <summary>
    /// Gets display value of a stat
    /// </summary>
    /// <param name="name">Stat Name</param>
    /// <returns>Display Value of a stat - no Calculation applied</returns>
    public float GetDisplayValue(string name)
    {
        return stats[name].DisplayValue;
    }
    /// <summary>
    /// Adds a function to a stats calculation stream
    /// </summary>
    /// <param name="statName">Stat Name</param>
    /// <param name="funcName">Function Name - must be unique</param>
    /// <param name="func">The function itself</param>
    /// <param name="priority">Priority of the stat - when it should run in the stream</param>
    public void addFunc(string id, string statName, string funcName, Func<float, float> func, int priority)
    {
        if (stats.TryGetValue(statName, out var stat))
        {
            stat.addFunc(id, funcName, func, priority);
        }
    }
    /// <summary>
    /// Remove a function from a stream
    /// </summary>
    /// <param name="statName">Stat Name</param>
    /// <param name="funcName">Function Name</param>
    public void removeFunc(string id, string statName, string funcName)
    {
        if (stats.TryGetValue(statName, out var stat))
        {
            stat.removeFunc(id, funcName);
        }
    }
}
