using System;
using Godot;
using Godot.NativeInterop;

public static class StatMaths
{
    //static formula function
    //functions obtained by invoking on name - careful changing any names
    //[stat name]Calc = calculation on value
    //[stat name]Validation = validation on setting a value


    public enum OriginType
    {
        Character,
        Projectile,
        Equipment,
    }

    public enum StatNum
    {
        defaultFallback,
        armour,
        critChance,
        critDamage,
        currentHealth,
        damageIncrease,
        itemFind,
        maxHealth,
        speed,
        damage,
        range,
        attackSpeed,
    }
    
    /// <summary>
    /// Default Fallback Functions - just returns the value straight
    /// </summary>
    /// <param name="v">Value of the stat</param>
    /// <returns>Value of the stat</returns>
    public static float defaultFallBackCalc(float v)
    {
        return v;
    }
    
    public static float defaultFallBackValidation(float v, OriginType origin)
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
        // GD.Print("speedCalc");
        return v;
    }
    
    /// <summary>
    /// Insures speed is positive and not extreme in either direction
    /// </summary>
    /// <param name="v">The value speed is being set to</param>
    /// <param name="origin">The origin of the stats class</param>
    /// <returns>A speed Between 100 and 1000</returns>
    public static float speedValidation(float v, OriginType origin)
    {
        v = Math.Clamp(v, 100, 1000);
        return v;
    }
    
    /// <summary>
    /// Armour Calculation
    /// </summary>
    /// <param name="v">The armour stat value</param>
    /// <returns>The multiplicative relation between the listed damage and the damage taken because of armour</returns>
    public static float armourCalc(float v)
    {
        // GD.Print("armourCalc");
        v = 100 / (100 + v);
        return v;
    }
    
    /// <summary>
    /// Ensure listed armour does not go below 0
    /// </summary>
    /// <param name="v">The value armour is being set to</param>
    /// <param name="origin">The origin of the stats class</param>
    /// <returns>Max of <paramref name="v"/> and  0</returns>
    public static float armourValidation(float v, OriginType origin)
    {
        v = float.Max(v, 0);
        return v;
    }
}