using Godot;
using Godot.NativeInterop;

public static class StatMaths
{
    public static float defaultFallBack(float value)
    {
        return value;
    }
    public static float speedCalc(float v)
    {
        return v;
    }
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