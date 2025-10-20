using Godot;
using System;


[GlobalClass]
public partial class SkillUI : Panel
{
    
    public SkillUIPanel selectedSkill = null;

    [Export]
    public String SkillName;


    [Export] 
    public Texture2D Icon;

    public void SetSkill(SkillUIPanel skill)
    {
        selectedSkill = skill;
        SkillName = skill.SkillName;
        Icon = skill.Icon;
        UpdateButton();
    }
    public Button GetButton()
    {
        return GetNode<Button>("Button");
    }

    public override void _Ready()
    {
        UpdateButton();
    }

    public void UpdateButton()
    {
        GetButton().Icon = Icon;
        GetButton().Text = SkillName;
    }
}
