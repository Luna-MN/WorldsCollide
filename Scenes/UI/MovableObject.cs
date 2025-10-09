using Godot;
using System;
[GlobalClass]
public partial class MovableObject : Panel
{
    [Export]
    public Area2D MoveArea;
    public bool mouseIn;
    private bool isDragging;
    private Vector2 dragOffset;

    public override void _Ready()
    {
        MoveArea.MouseEntered += () =>
        {
            mouseIn = true;
        };
        MoveArea.MouseExited += () =>
        {
            if (!isDragging)
            {
                mouseIn = false;
            }
        };
    }

    public override void _Process(double delta)
    {
        if (mouseIn && Input.IsMouseButtonPressed(MouseButton.Left))
        {
            if (!isDragging)
            {
                // Start dragging - store the offset between mouse and panel position
                isDragging = true;
                var globalMousePos = GetGlobalMousePosition();
                dragOffset = globalMousePos - GlobalPosition;
            }
            
            // Update position while maintaining the offset
            var newGlobalMousePos = GetGlobalMousePosition();
            GlobalPosition = newGlobalMousePos - dragOffset;
        }
        else
        {
            isDragging = false;
        }
    }

}
