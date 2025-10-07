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
        
        // Calculate the required bounds to contain both points
        Vector2 minBounds = new Vector2(
            Mathf.Min(0, Mathf.Min(start.X, end.X)),
            Mathf.Min(0, Mathf.Min(start.Y, end.Y))
        );
        
        Vector2 maxBounds = new Vector2(
            Mathf.Max(Size.X, Mathf.Max(start.X, end.X)),
            Mathf.Max(Size.Y, Mathf.Max(start.Y, end.Y))
        );
        
        // Calculate required dimensions
        Vector2 requiredSize = maxBounds - minBounds;
        
        // Use the larger dimension to make it a square
        float squareSize = Mathf.Max(requiredSize.X, requiredSize.Y);
        Vector2 newSize = new Vector2(squareSize, squareSize);
        
        // Calculate position offset (center the content in the square)
        Vector2 positionOffset = -minBounds;
        
        // Update size and position if needed
        if (newSize != Size)
        {
            Position += positionOffset;
            Size = newSize;
        }

        // calculate the normalized UV positions of the end and start of the line 
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
