using Godot;
using System;

public partial class CalculationStreamData : Resource
{
    public string Name;
    public Func<float, float> Func;
    public int Priority;

    public CalculationStreamData() : this("defaultFunc", null, 1) { }

    public CalculationStreamData(string name, Func<float, float> func, int priority)
    {
        this.Name = name;
        this.Func = func;
        this.Priority = priority;
    }
}
