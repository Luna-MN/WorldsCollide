using Godot;
using System;

public partial class FloatingText : Marker2D
{
    [Export]
    public Label text;

    public override async void _Ready()
    {
        var tween = CreateTween();

        tween.TweenProperty(this, "scale", new Vector2(2, 2), 0.5f).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
        tween.TweenProperty(this, "scale", new Vector2(0.5f, 0.5f), 0.5f).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
        await ToSignal(tween, Tween.SignalName.Finished); 
        QueueFree();
    }
}
