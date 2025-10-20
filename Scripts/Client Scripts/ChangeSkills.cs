using Godot;
using System;

public partial class ChangeSkills : Button
{
    [Export]
    public PackedScene SkillUI;
    [Export]
    public UiController UiController;

    public override void _Ready()
    {
        ButtonDown += onButtonPressed;
    }

    private void onButtonPressed()
    {
        if (UiController.SkillSelection != null) return;
        UiController.SkillSelection = SkillUI.Instantiate<SkillSelection>();
        UiController.AddChild(UiController.SkillSelection);
    }
}
