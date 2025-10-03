using Godot;
using System;

public partial class FloatingText : Marker2D
{
    [Export]
    public Label NoMultText;
    [Export]
    public Label valueText;
    [Export]
    public Label multiplierText;
    
    public float value;
    public float multiplier = 1;
    
    
    private Vector2 Vel = Vector2.Zero;
    public override async void _Ready()
    {
        if (multiplier == 1)
        {
            NoMultText.Show();
            valueText.Hide();
            multiplierText.Hide();
            NoMultText.Text = value.ToString();
        }
        else
        {
            NoMultText.Hide();
            valueText.Show();
            multiplierText.Show();
            valueText.Text = value.ToString();
            multiplierText.Text = "x" + multiplier;
            
            var valueTextSize = valueText.GetThemeFont("font").GetStringSize(valueText.Text, HorizontalAlignment.Left, -1, valueText.GetThemeFontSize("font_size"));
            var multiplierTextSize = multiplierText.GetThemeFont("font").GetStringSize(multiplierText.Text, HorizontalAlignment.Left, -1, multiplierText.GetThemeFontSize("font_size"));
            
            // Calculate total width of both labels
            var totalWidth = valueTextSize.X + multiplierTextSize.X;
        
            // Position value text to the left of center
            valueText.Position = new Vector2(-totalWidth / 2, valueText.Position.Y);
        
            // Position multiplier text right after value text
            multiplierText.Position = new Vector2(-totalWidth / 2 + valueTextSize.X, multiplierText.Position.Y);

        }
        var tween = CreateTween();
        var rng = new RandomNumberGenerator();
        var SideMovement = rng.RandiRange(-40, 40);
        Vel = new Vector2(SideMovement, 25);
        tween.TweenProperty(this, "scale", new Vector2(2, 2), 0.5f).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
        tween.TweenProperty(this, "scale", new Vector2(0.5f, 0.5f), 0.5f).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
        await ToSignal(tween, Tween.SignalName.Finished); 
        // if there is no text already above the character tween linearly out and back in, then combine the mult and value into real value
        // else queue free and add the value * mult to the character text that already exists
        QueueFree();
    }

    public override void _Process(double delta)
    {
        Position -= Vel * (float)delta;
    }
}
