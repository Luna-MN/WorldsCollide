using Godot;
using System;
[Tool]
[GlobalClass]
public partial class PixelLine : ColorRect
{
    [Export]
    public Vector2 start;
    [Export]
    public Vector2 end;
    [Export]
    public Color color;
    [Export]
    public float thickness;
    public override void _Process(double delta)
    {
        Vector2 normalizedStart = start / Size;
        Vector2 normalizedEnd = end / Size;

        var shader = (ShaderMaterial)Material;
        shader.SetShaderParameter("point_a", normalizedStart);
        shader.SetShaderParameter("point_b", normalizedEnd);

        shader.SetShaderParameter("color", color);
        shader.SetShaderParameter("thickness", thickness);
        shader.SetShaderParameter("node_scale", Size.X);

    }
}
