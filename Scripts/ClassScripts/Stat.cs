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
    // Might be useful, depends on how complicated we make formulas
    //private float _cachedResult;
    // Priority-ed list - main calculation runs at priority 10
    private List<(string, Func<float, float>, int)> _funcs = new ();
    private MethodInfo _calcFunction;
    private MethodInfo _validationFunction;
    
    /// <summary>
    /// Gets Sets Name of the stat - primarily for Godot Resource Handler
    /// </summary>
    [Export]
    public string Name
    {
        get => _name;
        set => setName(value);
    }
    
    /// <summary>
    /// Gets the Calculation value of the stat - allows setting
    /// </summary>
    public float CalcValue
    {
        get => calculation();
        set => _value = (float)(_validationFunction?.Invoke(null, [value])?? _value);
    }
    
    /// <summary>
    /// Returns the display value of the stat - just the stat value straight
    /// Allows setting
    /// </summary>
    [Export]
    public float DisplayValue
    {
        get => _value;
        set => _value = (float)(_validationFunction?.Invoke(null, [value]) ?? _value);
    }
    
    
    [ExportGroup("Value Clamp")]
    //I don't know - you're gonna have to fill this one in Matt
    [Export]
    public float MinValue;
    
    //I don't know - you're gonna have to fill this one in Matt
    [Export]
    public float MaxValue;
    
    //I don't know - you're gonna have to fill this one in Matt
    [Export(PropertyHint.GroupEnable)]
    public bool ClampValue;
    
    
    /// <summary>
    /// Default constructor for Godot's Resource Handler
    /// </summary>
    private Stat() : this("defaultStat", 0.0f) { }
    /// <summary>
    /// Constructor with paramters
    /// </summary>
    /// <param name="name">Name to create with</param>
    /// <param name="startingValue">Initial Value of stat</param>
    public Stat(string name, float startingValue)
    {
        if (name == "defaultStat" || name == "")
        {
            name = ResourceName;
        }
        setName(name);
        CalcValue = startingValue;
    }
    
    /// <summary>
    /// Sets the _name and finds the Validation and Calculation Functions off of that name
    /// </summary>
    /// <param name="name">New Name</param>
    private void setName(string name)
    {
        _name = name;
        // GD.Print($"Stat name: {_name}");
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
    
    /// <summary>
    /// Add a function to the stream in the correct location
    /// </summary>
    /// <param name="name">Function Name</param>
    /// <param name="func">The Function</param>
    /// <param name="priority">The priority</param>
    /// <exception cref="Exception">Error if the priority is 10</exception>
    /// <exception cref="Exception">Error if the name already exists</exception>
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
    
    /// <summary>
    /// Remove the function based on name
    /// </summary>
    /// <param name="name">Function Name</param>
    public void removeFunc(string name)
    {
        _funcs.Remove(_funcs.Find(tuple => tuple.Item1 == name));
    }
    
    /// <summary>
    /// Loop through the list and apply the functions in order to the value starting at the lowest priority
    /// The calculation from Stat Maths runs at priority 10
    /// </summary>
    /// <returns>The result of the stream</returns>
    private float calculation()
    {
        // GD.Print($"{Name} calculating : {_calcFunction?.Name} : {_validationFunction?.Name}");
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
                    v = function.Item2.Invoke(v);
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
                    v = function.Item2.Invoke(v);
                }
            }
        }

        if (!calcRun)
        {
            v = (float)(_calcFunction?.Invoke(null, [v])?? v);
        }
        // GD.Print($"{Name} calculated : {v}");
        return v;
    }
}