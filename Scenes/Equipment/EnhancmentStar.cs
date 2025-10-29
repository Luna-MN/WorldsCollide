using Godot;
using System;

public partial class EnhancmentStar : TextureRect
{
    public ConstillationSlotData data;
    public bool mouseOver;
    public bool dragging;
    public Vector2 offset;
    public override void _Ready()
    {
        MouseEntered += () =>
        {
            mouseOver = true;
        };
        MouseExited += () =>
        {
            mouseOver = false;
        };
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.Pressed && mouseOver)
            {
                dragging = true;
                offset = GetGlobalMousePosition() - GlobalPosition;
            }
            else
            {
                dragging = false;
            }
        }
    }

    public override void _Process(double delta)
    {
        if (dragging)
        {
            GlobalPosition = GetGlobalMousePosition() - offset;
        }
    }
}
