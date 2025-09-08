using Godot;
using System;
using System.Linq;

public partial class SkillSelection : Panel
{
    [Export]
    public PackedScene skillUIPanel;
    [Export]
    private GridContainer SkillGrid;
    [Export]
    private Button ApplyButton;
    [Export]
    public SkillUI[] SkillsSelected;
    public SkillUIPanel[] SkillPanels;
    public SkillUIPanel PressedSkill = null;
    public SkillUI pressedSelectSkill;
    public override void _Ready()
    {
        foreach (var skill in GameManager.player.skills)
        {
            var createdSkill = skillUIPanel.Instantiate<SkillUIPanel>();
            createdSkill.SkillName = skill.Name;
            createdSkill.Icon = skill.Icon;
            createdSkill.skillIndex = GameManager.player.skills.ToList().IndexOf(skill);
            createdSkill.GetButton().ButtonDown += () =>
            {
                if (pressedSelectSkill != null)
                {
                    pressedSelectSkill.selectedSkill = createdSkill;
                    pressedSelectSkill.Icon = createdSkill.Icon;
                    createdSkill.Modulate = new Color(1, 1, 1);
                    pressedSelectSkill.UpdateButton();
                    pressedSelectSkill = null;
                }
                else
                {
                    createdSkill.Modulate = new Color(0, 0, 1);
                    PressedSkill = createdSkill;
                }
            };
            SkillGrid.AddChild(createdSkill);
        }
        foreach (SkillUI skill in SkillsSelected)
        {
            skill.GetButton().ButtonDown += () =>
            {
                if (PressedSkill != null)
                {
                    PressedSkill.Modulate = new Color(1, 1, 1);
                    skill.Icon = PressedSkill.Icon;
                    skill.selectedSkill = PressedSkill;
                    skill.UpdateButton();
                    PressedSkill = null;
                }
                else
                {
                    pressedSelectSkill = skill;
                }
            };
        }

        ApplyButton.ButtonDown += () =>
        {
            foreach (var skill in SkillsSelected)
            {

                GameManager.player.selectedSkillIndexes[SkillsSelected.ToList().IndexOf(skill)] = skill.selectedSkill.skillIndex;
            }
            GameManager.player.
            QueueFree();
        };
    }
}
