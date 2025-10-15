
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
    public Vector2 SpawnPosition = Vector2.Inf;
    public override void _Ready()
    {
        if (SpawnPosition == Vector2.Inf)
        {
            SpawnPosition = GetViewportRect().Size / 2;
        }
        // Prevent spawning on top of other MovableObjects
        JustifyPosition(SpawnPosition);

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
    private void JustifyPosition(Vector2 idealPosition)
    {
        uiController = GetParent<UiController>();
        if (uiController?.Objects == null) return;

        // Start from the center of the screen
        var viewportSize = GetViewportRect().Size;
        
        // If no overlap at ideal position, use it
        var idealRect = new Rect2(idealPosition, Size);
        bool hasOverlap = false;
        
        foreach (var obj in uiController.Objects)
        {
            if (obj == this) continue;
            
            var otherRect = new Rect2(obj.GlobalPosition, obj.Size);
            if (idealRect.Intersects(otherRect))
            {
                hasOverlap = true;
                break;
            }
        }
        
        if (!hasOverlap)
        {
            GlobalPosition = idealPosition;
            uiController.Objects.Add(this);
            return;
        }
        
        // Find closest non-overlapping position using spiral pattern
        Vector2 bestPosition = idealPosition;
        float bestDistance = float.MaxValue;
        const int stepSize = 10; // How fine the search grid is
        const int maxRadius = 500; // Maximum search distance from center
        
        for (int radius = stepSize; radius <= maxRadius; radius += stepSize)
        {
            // Check positions in a square pattern around the ideal position
            for (int x = -radius; x <= radius; x += stepSize)
            {
                for (int y = -radius; y <= radius; y += stepSize)
                {
                    // Only check positions on the perimeter of current radius
                    if (Mathf.Abs(x) != radius && Mathf.Abs(y) != radius) continue;
                    
                    var testPosition = idealPosition + new Vector2(x, y);
                    var testRect = new Rect2(testPosition, Size);
                    
                    // Check if position is within viewport
                    if (testPosition.X < 0 || testPosition.Y < 0 || 
                        testPosition.X + Size.X > viewportSize.X || 
                        testPosition.Y + Size.Y > viewportSize.Y)
                        continue;
                    
                    // Check for overlaps
                    bool overlapping = false;
                    foreach (var obj in uiController.Objects)
                    {
                        if (obj == this) continue;
                        
                        var otherRect = new Rect2(obj.GlobalPosition, obj.Size);
                        if (testRect.Intersects(otherRect))
                        {
                            overlapping = true;
                            break;
                        }
                    }
                    
                    if (!overlapping)
                    {
                        float distance = testPosition.DistanceTo(idealPosition);
                        if (distance < bestDistance)
                        {
                            bestDistance = distance;
                            bestPosition = testPosition;
                        }
                    }
                }
            }
        }
        
        GlobalPosition = bestPosition;
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
        if (Input.IsActionJustPressed("left_click"))
        {
            GetParent().MoveChild(this, uiController.Objects.Count - 1);
        }
        
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