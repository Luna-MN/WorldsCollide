using Godot;
using Godot.NativeInterop;

public static class StatMaths
{
    //static formula function
    //functions obtained by invoking on name - careful changing any names
    //[stat name]Calc = calculation on value
    //[stat name]Validation = validation on setting a value
    
    
    /// <summary>
    /// Default Fallback Function - just returns the value straight
    /// </summary>
    /// <param name="v">Value of the stat</param>
    /// <returns>Value of the stat</returns>
    public static float defaultFallBack(float v)
    {
        return v;
    }
    
    /// <summary>
    /// Speed Calculation function - does nothing right now
    /// </summary>
    /// <param name="v">The speed stat</param>
    /// <returns>The speed value</returns>
    public static float speedCalc(float v)
    {
        GD.Print("speedCalc");
        return v;
    }
    
    /// <summary>
    /// Insures speed is positive and not extreme in either direction
    /// </summary>
    /// <param name="v">The value speed is being set to</param>
    /// <returns>A speed Between 100 and 1000</returns>
    public static float speedValidation(float v)
    {
        if (v < 100)
            return 100;
        if (v > 1000)
            return 1000;
        return v;

    }
    
    /// <summary>
    /// Armour Calculation
    /// </summary>
    /// <param name="v">The armour stat value</param>
    /// <returns>The multiplicative relation between the listed damage and the damage taken because of armour</returns>
    public static float armourCalc(float v)
    {
        GD.Print("armourCalc");
        v = 100 / (100 + v);
        return v;
    }
    
    /// <summary>
    /// Ensure listed armour does not go below 0
    /// </summary>
    /// <param name="v">The value armour is being set to</param>
    /// <returns>Max of <paramref name="v"/> and  0</returns>
    public static float armourValidation(float v)
    {
        v = float.Max(v, 0);
        return v;
    }
}