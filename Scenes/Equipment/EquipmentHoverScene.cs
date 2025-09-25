using Godot;
using System;

public partial class EquipmentHoverScene : Panel
{
    [Export] public RichTextLabel ItemName;
    [Export] public RichTextLabel ItemDescription;
    [Export] public Sprite2D ItemIcon;

    public override void _Process(double delta)
    {
        // Get the mouse position
        Vector2 mousePos = GetGlobalMousePosition();
        
        // Get the window size
        Vector2 windowSize = GetViewportRect().Size;
        
        // Get the size of this panel
        Vector2 panelSize = Size;
        
        // Clamp the position to stay within the screen bounds
        Vector2 clampedPosition = new Vector2(
            Mathf.Clamp(mousePos.X, 0, windowSize.X - panelSize.X),
            Mathf.Clamp(mousePos.Y, 0, windowSize.Y - panelSize.Y)
        );
        
        // Set the global position with the clamped values
        GlobalPosition = clampedPosition;
    }

}
