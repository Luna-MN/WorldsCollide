
using Godot;
using System;

[GlobalClass]
public partial class MovableObject : Panel
{
    [Export]
    public Control MoveArea;
    [Export]
    public Control ScaleArea;
    [Export]
    public Vector2 MinSize = new Vector2(50, 50);
    [Export]
    public Vector2 MaxSize = new Vector2(1000, 1000);
    public bool mouseInMove;
    public bool mouseInScale;
    private bool isDragging;
    private bool isScaling;
    private Vector2 dragOffset;
    private Vector2 scaleOffset;

    public override void _Ready()
    {
        MoveArea.MouseEntered += () =>
        {
            mouseInMove = true;
        };
        MoveArea.MouseExited += () =>
        {
            if (!isDragging)
            {
                mouseInMove = false;
            }
        };
        ScaleArea.MouseEntered += () =>
        {
            mouseInScale = true;
        };
        ScaleArea.MouseExited += () =>
        {
            if (!isScaling)
            {
                mouseInScale = false;
            }
        };
    }

    public override void _Process(double delta)
    {
        // Handle dragging
        if (mouseInMove && Input.IsMouseButtonPressed(MouseButton.Left) && !isScaling)
        {
            if (!isDragging)
            {
                // Start the drag
                isDragging = true;
                var globalMousePos = GetGlobalMousePosition();
                dragOffset = globalMousePos - GlobalPosition;
            }
            // If we are already dragging
            var newGlobalMousePos = GetGlobalMousePosition();
            GlobalPosition = newGlobalMousePos - dragOffset;
        }
        else
        {
            isDragging = false;
        }

        // Handle scaling
        if (mouseInScale && Input.IsMouseButtonPressed(MouseButton.Left) && !isDragging)
        {
            if (!isScaling)
            {
                isScaling = true;

                scaleOffset = Size - GetLocalMousePosition();
            }
            
            // Get mouse position relative to this panel's position
            var mousePos = GetLocalMousePosition();
            var localMousePos = mousePos + scaleOffset;
            
            // The new size should be where the mouse is relative to the top-left corner
            var newSize = localMousePos;
            
            // Apply minimum size constraints
            newSize.X = Mathf.Max(newSize.X, MinSize.X);
            newSize.Y = Mathf.Max(newSize.Y, MinSize.Y);
            // Apply maximum size constraints
            newSize.X = Mathf.Min(newSize.X, MaxSize.X);
            newSize.Y = Mathf.Min(newSize.Y, MaxSize.Y);
            
            Size = newSize;
        }
        else
        {
            isScaling = false;
        }
    }
}