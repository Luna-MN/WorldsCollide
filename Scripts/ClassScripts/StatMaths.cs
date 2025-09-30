using Godot;
using Godot.NativeInterop;

public static class StatMaths
{
    //static formula function
    //functions obtained by invoking on name - careful changing any names
    //[stat name]Calc = calculation on value
    //[stat name]Validation = validation on setting a value
    //defaultFallBack returns the value put in. - it will use this if the function it's looking for can't be found
    public static float defaultFallBack(float v)
    {
        return v;
    }
    public static float speedCalc(float v)
    {
        GD.Print("speedCalc");
        return v;
    }
    //ensure speed is positive and not too fast
    public static float speedValidation(float v)
    {
        if (v < 100)
            return 100;
        if (v > 1000)
            return 1000;
        return v;

    }
    public static float armourCalc(float v)
    {
        GD.Print("armourCalc");
        v = 100 / (100 + v);
        return v;
    }

    public static float armourValidation(float v)
    {
        v = float.Max(v, 0);
        return v;
    }
}