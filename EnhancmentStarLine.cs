using Godot;
using System.Collections.Generic;

public partial class ConstellationLines : Control
{
    public List<EnhancmentStar> stars = new();
    public List<(int start, int end)> connections = new();
    
    public override void _Ready()
    {
        // Create a material with no filtering for pixelated effect
        SetTextureFilter(TextureFilterEnum.Nearest);
    }

    public override void _Draw()
    {
        foreach (var (start, end) in connections)
        {
            if (start < stars.Count && end < stars.Count)
            {
                var startPos = stars[start].Position + stars[start].Size / 2; // Center of star
                var endPos = stars[end].Position + stars[end].Size / 2;
                
                DrawLine(startPos, endPos, Colors.White, 2.0f);
            }
        }
    }
}