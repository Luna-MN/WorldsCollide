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
