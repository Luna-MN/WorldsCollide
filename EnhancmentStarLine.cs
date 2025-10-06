using Godot;
using System;

public partial class EnhancmentStarLine : Line2D
{
    public EnhancmentStar startStar;
    public EnhancmentStar endStar;

    public override void _Process(double delta)
    {
        if(startStar == null || endStar == null) return;
        AddPoint(startStar.GlobalPosition);
        AddPoint(endStar.GlobalPosition);
    }
}
