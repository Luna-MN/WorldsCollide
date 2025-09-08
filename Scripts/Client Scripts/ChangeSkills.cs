using Godot;
using System;

public partial class ChangeSkills : Button
{
    [Export]
    public PackedScene SkillUI;
    public SkillSelection SkillPanel;
    public override void _Ready()
    {
        ButtonDown += onButtonPressed;
    }

    private void onButtonPressed()
    {
        if (SkillPanel != null) return;
        SkillPanel = SkillUI.Instantiate<SkillSelection>();
        GetParent().AddChild(SkillPanel);
    }
}
