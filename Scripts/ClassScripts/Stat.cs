using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

[GlobalClass]
public partial class Stat : Resource
{
    [Export]
    private string _name;
    private float _value = 0.0f;
    //might be useful, depends on how complicated we make formulas
    //private float _cachedResult;
    
    //priority-ed list - main calculation runs at priority 10
    private List<(string, Func<float, float>, int)> _funcs = new ();
    private MethodInfo _calcFunction;
    private MethodInfo _setValidationFunction;
    public string Name
    {
        get => _name;
    }
    //value indexer returns the calculated value
    //allows setting
    public float Value
    {
        get => calculation();
        set => _value = (float)(_setValidationFunction?.Invoke(null, [value])?? _value);
    }
    //display value indexer returns the value straight
    //allows setting
    [Export]
    public float DisplayValue
    {
        get => _value;
        set => _value = (float)(_setValidationFunction?.Invoke(null, [value]) ?? _value);
    }

    //default constructor for godot
    private Stat()
    {
        _name = "Default Crappy Stupid Stat";
        _calcFunction = typeof(StatMaths).GetMethod("defaultFallBack", BindingFlags.Static | BindingFlags.Public);
        _setValidationFunction = typeof(StatMaths).GetMethod("defaultFallBack", BindingFlags.Static | BindingFlags.Public);
        Value = 0.0f;
    }
    //constructor for use
    public Stat(string name, float startingValue)
    {
        StatConstructor(name, startingValue);
    }
    public void StatConstructor(string name, float startingValue)
    {
        _name = name;
        try
        {
            _setValidationFunction = typeof(StatMaths).GetMethod(name + "Vaildation", BindingFlags.Static | BindingFlags.Public);
            if (_setValidationFunction == null)
                _setValidationFunction = typeof(StatMaths).GetMethod("defaultFallBack", BindingFlags.Static | BindingFlags.Public);
            
        }
        catch (Exception e)
        {
            _setValidationFunction = typeof(StatMaths).GetMethod("defaultFallBack", BindingFlags.Static | BindingFlags.Public);
            
        }
        try
        {
            _calcFunction = typeof(StatMaths).GetMethod(name + "Calc", BindingFlags.Static | BindingFlags.Public);
            if (_calcFunction == null)
                _calcFunction = typeof(StatMaths).GetMethod("defaultFallBack", BindingFlags.Static | BindingFlags.Public);
        }
        catch (Exception e)
        {
            _calcFunction = typeof(StatMaths).GetMethod("defaultFallBack", BindingFlags.Static | BindingFlags.Public);
        }
        Value = startingValue;
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
        float v = _value;
        bool calcRun = false;
        if (_funcs.Count != 0)
        {
            foreach (var valueTuple in _funcs)
            {
                if (valueTuple.Item3 < 10)
                {
                    v = valueTuple.Item2.Invoke(_value);
                }
                else if (valueTuple.Item3 > 10 && !calcRun)
                {
                    v = (float)(_calcFunction?.Invoke(null, [v]) ?? v);
                    calcRun = true;
                }
                else
                {
                    v = valueTuple.Item2.Invoke(_value);
                }
            }
        }

        if (!calcRun)
        {
            v = (float)(_calcFunction?.Invoke(null, [v])?? v);
        }
        return v;
    }
}