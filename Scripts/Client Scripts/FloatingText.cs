using Godot;
using System;

public partial class FloatingText : Marker2D
{
    [Export]
    public Label text;
    
    private Vector2 Vel = Vector2.Zero;
    public override async void _Ready()
    {
        var tween = CreateTween();
        var rng = new RandomNumberGenerator();
        var SideMovement = rng.RandiRange(60, 100) - 40;
        Vel = new Vector2(SideMovement, 25);
        tween.TweenProperty(this, "scale", new Vector2(2, 2), 0.5f).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
        tween.TweenProperty(this, "scale", new Vector2(0.5f, 0.5f), 0.5f).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
        await ToSignal(tween, Tween.SignalName.Finished); 
        QueueFree();
    }

    public override void _Process(double delta)
    {
        Position -= Vel * (float)delta;
    }
}
