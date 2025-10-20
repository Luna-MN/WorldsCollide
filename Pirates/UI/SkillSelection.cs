using Godot;
using System;
using System.Linq;

public partial class SkillSelection : MovableObject
{
    [Export]
    private PackedScene skillUIPanel;
    [Export]
    private SkillButton[] SkillButtons;
    [Export]
    private GridContainer SkillGrid;
    private SkillButton selectedSkill;
    private SkillButton pressedSkillButton;
    public override void _Ready()
    {
        base._Ready();
        foreach (var skill in GameManager.player.skills)
        {
            // create a skill panel for each skill
            var skillUI = skillUIPanel.Instantiate<SkillButton>();
            skillUI.skillIndex = GameManager.player.skills.ToList().IndexOf(skill);
            SkillGrid.AddChild(skillUI);
            skillUI.SkillName = skill.Name;
            if (skill.Icon != null)
            {
                skillUI.Icon.Texture = skill.Icon;
            }

            skillUI.ButtonDown += () =>
            {
                // if there already is a selected skill change it back to the starting set of it
                if (selectedSkill != null)
                {
                    selectedSkill.SelfModulate = new Color(1, 1, 1);
                }
                // if we have a skill button pressed then set it to this
                if (pressedSkillButton != null)
                {
                    pressedSkillButton.SetSkill(skillUI);
                    pressedSkillButton.SelfModulate = new Color(1, 1,1);
                    pressedSkillButton = null;
                }
                // if we have this skill selected, deselect it
                else if (selectedSkill != null && selectedSkill == skillUI)
                {
                    selectedSkill.SelfModulate = new Color(1, 1, 1);   
                    selectedSkill = null;
                }
                // select the skill if nothing is selected
                else
                {
                    selectedSkill = skillUI;
                    selectedSkill.SelfModulate = Colors.LightBlue;
                }
            };
        }

        foreach (var skillButton in SkillButtons)
        {
            skillButton.ButtonDown += () =>
            {
                // if a button is pressed then set it back to normal
                if (pressedSkillButton != null)
                {
                    pressedSkillButton.SelfModulate = new Color(1, 1, 1);
                }
                // if a skill is pressed then set this to it and return to normal
                if (selectedSkill != null)
                {
                    skillButton.SetSkill(selectedSkill);
                    selectedSkill.SelfModulate = new Color(1, 1, 1);
                    selectedSkill = null;
                }
                // if a this is already selected, deselect it
                else if (pressedSkillButton != null && pressedSkillButton != skillButton)
                {
                    pressedSkillButton.SelfModulate = new Color(1, 1, 1);
                    pressedSkillButton = null;
                }
                // if nothing is selected, select this
                else
                {
                    pressedSkillButton = skillButton;
                    pressedSkillButton.SelfModulate = Colors.LightBlue;
                }
            };
        }
    }
    public override void Close()
    {
        uiController.SkillSelection = null;
        base.Close();
    }
}
