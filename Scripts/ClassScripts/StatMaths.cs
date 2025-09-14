using Godot;
using Godot.NativeInterop;

public static class StatMaths
{
    //static formula function
    //functions obtained by invoking on name - careful changing any names
    //[stat name]Calc = calculation on value
    //[stat name]Vaildation = validation on setting a value
    //defaultFallBack returns the value put in. - it will use this if the function it's looking for can't be found
    public static float defaultFallBack(float value)
    {
        return value;
    }
    public static float speedCalc(float v)
    {
        return v;
    }
    //ensure speed is positive and not too fast
    public static float speedVaildation(float value)
    {
        float v = (float)value;
        if (v < 100)
            return 100;
        if (v > 1000)
            return 1000;
        return v;

    }
}