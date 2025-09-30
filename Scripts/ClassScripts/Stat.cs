using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

[GlobalClass]
public partial class Stat : Resource
{
    
    private string _name;
    private float _value = 0.0f;
    //might be useful, depends on how complicated we make formulas
    //private float _cachedResult;
    
    //priority-ed list - main calculation runs at priority 10
    private List<(string, Func<float, float>, int)> _funcs = new ();
    private MethodInfo _calcFunction;
    private MethodInfo _validationFunction;
    [Export]
    public string Name
    {
        get => _name;
        set => setName(value);
    }
    //value indexer returns the calculated value
    //allows setting
    public float Value
    {
        get => calculation();
        set => _value = (float)(_validationFunction?.Invoke(null, [value])?? _value);
    }
    //display value indexer returns the value straight
    //allows setting
    [Export]
    public float DisplayValue
    {
        get => _value;
        set => _value = (float)(_validationFunction?.Invoke(null, [value]) ?? _value);
    }
    [ExportGroup("Value Clamp")]
    [Export]
    public float MinValue;
    [Export]
    public float MaxValue;
    [Export(PropertyHint.GroupEnable)]
    public bool ClampValue;
    //default constructor for godot
    private Stat() : this("defaultStat", 0.0f) { }
    //constructor for use
    public Stat(string name, float startingValue)
    {
        if (name == "defaultStat" || name == "")
        {
            name = ResourceName;
        }
        setName(name);
        Value = startingValue;
    }
    private void setName(string name)
    {
        _name = name;
        GD.Print($"Stat name: {_name}");
        try
        {
            //[stat name]Vaildation = validation on setting a value
            _validationFunction = typeof(StatMaths).GetMethod(name + "Validation", BindingFlags.Static | BindingFlags.Public);
            if (_validationFunction == null)
                _validationFunction = typeof(StatMaths).GetMethod("defaultFallBack", BindingFlags.Static | BindingFlags.Public);
            
        }
        catch (Exception e)
        {
            _validationFunction = typeof(StatMaths).GetMethod("defaultFallBack", BindingFlags.Static | BindingFlags.Public);
        }
        try
        {
            //[stat name]Calc = calculation on value
            _calcFunction = typeof(StatMaths).GetMethod(name + "Calc", BindingFlags.Static | BindingFlags.Public);
            if (_calcFunction == null)
                _calcFunction = typeof(StatMaths).GetMethod("defaultFallBack", BindingFlags.Static | BindingFlags.Public);
        }
        catch (Exception e)
        {
            _calcFunction = typeof(StatMaths).GetMethod("defaultFallBack", BindingFlags.Static | BindingFlags.Public);
        }
    }
    //adding functions to priority-ed list - add in priority order
    public void addFunc(string name, Func<float, float> func, int priority)
    {
        if (_funcs.Count(tuple => tuple.Item1 == name) != 0)
            throw new Exception("There is already a function with that name");
        if (priority == 10)
            throw new Exception("Priority 10 not allowed");
        foreach (var valueTuple in _funcs)
        {
            if (valueTuple.Item3 <= priority)
                continue;
            _funcs.Insert(_funcs.IndexOf(valueTuple), (name, func, priority));
            return;
        }
        _funcs.Add((name, func, priority));
    }
    //removing
    public void removeFunc(string name)
    {
        _funcs.Remove(_funcs.Find(tuple => tuple.Item1 == name));
    }
    //do the math - loop through list - running the calculation at 10
    private float calculation()
    {
        GD.Print($"{Name} calculating : {_calcFunction?.Name} : {_validationFunction?.Name}");
        float v = _value;
        bool calcRun = false;
        if (_funcs.Count != 0)
        {
            //loop through _func
            foreach (var function in _funcs)
            {
                //priorities lower than 10 before the default calculation 
                if (function.Item3 < 10)
                {
                    v = function.Item2.Invoke(_value);
                }
                //do the default calc calculation
                else if (function.Item3 > 10 && !calcRun)
                {
                    v = (float)(_calcFunction?.Invoke(null, [v]) ?? v);
                    calcRun = true;
                }
                //do the rest
                else
                {
                    v = function.Item2.Invoke(_value);
                }
            }
        }

        if (!calcRun)
        {
            v = (float)(_calcFunction?.Invoke(null, [v])?? v);
        }
        GD.Print($"{Name} calculated : {v}");
        return v;
    }
}