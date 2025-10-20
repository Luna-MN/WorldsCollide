using Godot;
using System;

[GlobalClass]
public partial class SkillButton : Button
{
    public int skillIndex;

    [Export]
    public String SkillName;
    [Export]
    public new TextureRect Icon;
    [Export]
    public new Label Text;

    public void SetSkill(SkillButton skill)
    {
        skillIndex = skill.skillIndex;
        Text.Text = skill.SkillName;
        Icon.Texture = skill.Icon.Texture;
    }

    public override void _Ready()
    {
    }
}
