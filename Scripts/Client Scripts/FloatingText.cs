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
    public Character character;
    public bool moving = true;
    public Timer GoneTimer;
    private Vector2 Vel = Vector2.Zero;
    public float RunningTotal;
    public override async void _Ready()
    {
        Vel = new Vector2(0, 25);
        if (character.RunningTotal == null)
        {
            character.RunningTotal = this;
            RunningTotal = value * multiplier;
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
            // Vel = new Vector2(SideMovement, 25);

            tween.TweenProperty(this, "scale", new Vector2(2f, 2f), 0.5f).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
            tween.TweenProperty(this, "scale", new Vector2(1f, 1f), 0.5f).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
            await ToSignal(tween, Tween.SignalName.Finished); 
            // if there is no text already above the character tween linearly out and back in, then combine the mult and value into real value


            var MultTween = CreateTween();
            var ValueTween = CreateTween();
    
            // Store original positions
            var originalValuePos = valueText.Position;
            var originalMultPos = multiplierText.Position;
    
            // Move the texts away from each other
            MultTween.TweenProperty(multiplierText, "position", originalMultPos + new Vector2(20f, 0f), 0.5f).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
            ValueTween.TweenProperty(valueText, "position", originalValuePos - new Vector2(20f, 0f), 0.5f).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
            
    
            // Move them back to center (combine)
            MultTween.TweenProperty(multiplierText, "position", new Vector2(5, originalMultPos.Y), 0.1f).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.In);
            ValueTween.TweenProperty(valueText, "position", new Vector2(0, originalValuePos.Y), 0.1f).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.In);
    
            await ToSignal(MultTween, Tween.SignalName.Finished);
            moving = false;
            multiplierText.Hide();
            // Now you could hide both texts and show the combined result
            valueText.Hide();
            NoMultText.Text = RunningTotal.ToString();
            NoMultText.Show();
            
            GoneTimer = new Timer()
            {
                WaitTime = 2,
                OneShot = true,
                Autostart = true,
                Name = "GoneTimer"
            };
            GoneTimer.Timeout += () =>
            {
                character.RunningTotal = null;
                QueueFree();
            };
            AddChild(GoneTimer);
        }
        // else queue free and add the value * mult to the character text that already exists
        else
        {
            NoMultText.Show();
            valueText.Hide();
            multiplierText.Hide();
            var newTotal = value * multiplier;
            NoMultText.Text = newTotal.ToString();
            var currentTotal = character.RunningTotal.RunningTotal;
            character.RunningTotal.RunningTotal = currentTotal + newTotal;
            character.RunningTotal.NoMultText.Text = character.RunningTotal.RunningTotal.ToString();
            character.RunningTotal.GoneTimer?.Stop();
            character.RunningTotal.GoneTimer?.Start();
            GlobalPosition = character.RunningTotal.GlobalPosition;
            
            var tween = CreateTween();

            tween.TweenProperty(this, "scale", new Vector2(2f, 2f), 0.5f).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
            tween.TweenProperty(this, "scale", new Vector2(1f, 1f), 0.5f).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
            await ToSignal(tween, Tween.SignalName.Finished);
            QueueFree();

        }
    }

    public override void _Process(double delta)
    {
        if (!moving) return;
        if (moving)
        {
            Position -= Vel * (float)delta;
        }
    }
}
