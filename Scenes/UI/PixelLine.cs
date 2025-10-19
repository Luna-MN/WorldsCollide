using Godot;
using System;

[Tool]
[GlobalClass]
public partial class PixelLine : ColorRect
{
    // Existing exports (kept for compatibility)
    [Export] public Vector2 start;
    [Export] public Vector2 end;
    [Export] public Color color = Colors.White;
    [Export] public float thickness = 1.0f;

    // use normalized endpoints so the line stays in place when the Control resizes.
    [Export] public bool UseNormalizedEndpoints { get; set; } = true;

    // 0..1 coordinates within this control's rect
    [Export(PropertyHint.Range, "0,1,0.001")]
    public Vector2 StartUV { get; set; } = new Vector2(0f, 0.5f);

    [Export(PropertyHint.Range, "0,1,0.001")]
    public Vector2 EndUV { get; set; } = new Vector2(1f, 0.5f);

    private ShaderMaterial _shader;

    public override void _Ready()
    {
        _shader = Material as ShaderMaterial;
    }

    public override void _Process(double delta)
    {
        // Ensure we have a shader material to configure
        if (_shader == null)
        {
            _shader = Material as ShaderMaterial;
            if (_shader == null)
                return;
        }

        var size = Size;
        if (size.X <= 0 || size.Y <= 0)
            return;

        // IMPORTANT: Do not modify Position or Size here.
        // Let the layout/anchors handle the Control's size/position.

        // Compute normalized endpoints (0..1)
        Vector2 normalizedStart = UseNormalizedEndpoints ? StartUV : start / size;
        Vector2 normalizedEnd   = UseNormalizedEndpoints ? EndUV   : end   / size;
        
        // Clamp to safe 0..1 range
        normalizedStart = new Vector2(Mathf.Clamp(normalizedStart.X, 0f, 1f), Mathf.Clamp(normalizedStart.Y, 0f, 1f));
        normalizedEnd   = new Vector2(Mathf.Clamp(normalizedEnd.X,   0f, 1f), Mathf.Clamp(normalizedEnd.Y,   0f, 1f));

        // If your shader expects a single "node_scale" float, pick a consistent axis.
        // Use min axis to keep thickness uniform regardless of aspect ratio.
        float axis = Mathf.Min(size.X, size.Y);

        // Account for global canvas scaling so thickness remains visually constant
        // even if the node (or its parents) is scaled.
        Transform2D gt = GetGlobalTransformWithCanvas();
        float sx = gt.X.Length();
        float sy = gt.Y.Length();
        float canvasAxis = Mathf.Min(size.X * sx, size.Y * sy);

        // Send parameters to the shader
        _shader.SetShaderParameter("point_a", normalizedStart);
        _shader.SetShaderParameter("point_b", normalizedEnd);
        _shader.SetShaderParameter("color", color);
        _shader.SetShaderParameter("thickness", thickness);
        _shader.SetShaderParameter("node_scale", canvasAxis > 0 ? canvasAxis : axis);
    }
}