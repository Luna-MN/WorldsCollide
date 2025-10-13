
using Godot;
using System;

[GlobalClass]
public partial class MovableObject : Panel
{
    [Export] public Control MoveArea;
    [Export] public Control ScaleArea;
    [Export] public Button CloseButton;
    [Export] public Vector2 MinSize = new Vector2(50, 50);
    [Export] public Vector2 MaxSize = new Vector2(1000, 1000);
    public bool mouseInMove;
    public bool mouseInScale;
    public bool mouseInScaleStore;
    public bool firstFrame = true;
    private bool isDragging;
    private bool isScaling;
    private Vector2 dragOffset;
    private Vector2 scaleOffset;
    private UiController uiController;
    public override void _Ready()
    {
        // Prevent spawning on top of other MovableObjects
        AdjustPositionToAvoidOverlap();

        MoveArea.MouseEntered += () => { mouseInMove = true; };
        MoveArea.MouseExited += () =>
        {
            if (!isDragging)
            {
                mouseInMove = false;
            }
        };
        ScaleArea.MouseEntered += () => { mouseInScale = true; };
        ScaleArea.MouseExited += () =>
        {
            if (!isScaling)
            {
                mouseInScale = false;
            }
            else
            {
                mouseInScaleStore = true;
            }
        };
        CloseButton.ButtonDown += () => { Close(); };
    }
    private void AdjustPositionToAvoidOverlap()
    {
        uiController = GetParent<UiController>();
        if (uiController?.Objects == null) return;

        // Start from the center of the screen
        var viewportSize = GetViewportRect().Size;
        var startPosition = (viewportSize - Size) / 2;
    
        var currentRect = new Rect2(startPosition, Size);
        const int offset = 30; // Offset distance when overlapping
        int attempts = 0;
        const int maxAttempts = 20;

        while (attempts < maxAttempts)
        {
            bool overlapping = false;
        
            foreach (var obj in uiController.Objects)
            {
                if (obj == this) continue;
            
                var otherRect = new Rect2(obj.GlobalPosition, obj.Size);
                if (currentRect.Intersects(otherRect))
                {
                    overlapping = true;
                    // Move to the right and down
                    currentRect.Position += new Vector2(offset, 0);
                    break;
                }
            }
        
            if (!overlapping)
            {
                GlobalPosition = currentRect.Position;
                break;
            }
        
            attempts++;
        }
        uiController.Objects.Add(this);
    }


    /// <summary>
    /// This will be called when the UI object is closed, use it to clean up and then call base
    /// </summary>
    public virtual void Close()
    {
        QueueFree();
    }
    public override void _Process(double delta)
    {
        //stop scaling when leave and stop scaling
        if (mouseInScaleStore && !isScaling)
        {
            mouseInScaleStore = false;
            mouseInScale = false;
        }

        // Handle dragging - only start on just pressed
        if (mouseInMove && !isScaling)
        {
            if (Input.IsActionJustPressed("left_click") && !isDragging)
            {
                // Start the drag
                isDragging = true;
                var globalMousePos = GetGlobalMousePosition();
                dragOffset = globalMousePos - GlobalPosition;
            }

            // Continue dragging if already started
            if (isDragging && Input.IsMouseButtonPressed(MouseButton.Left))
            {
                var newGlobalMousePos = GetGlobalMousePosition();
                GlobalPosition = newGlobalMousePos - dragOffset;
            }
        }

        if (!Input.IsMouseButtonPressed(MouseButton.Left))
        {
            isDragging = false;
        }

        // Handle scaling - only start on just pressed
        if (mouseInScale && !isDragging)
        {
            if (Input.IsActionJustPressed("left_click") && !isScaling)
            {
                isScaling = true;
                scaleOffset = Size - GetLocalMousePosition();
            }

            // Continue scaling if already started
            if (isScaling && Input.IsMouseButtonPressed(MouseButton.Left))
            {
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
        }

        if (!Input.IsMouseButtonPressed(MouseButton.Left))
        {
            isScaling = false;
        }
    }

    public override void _ExitTree()
    {
        uiController.Objects.Remove(this);
    }
}