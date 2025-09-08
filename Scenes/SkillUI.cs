using Godot;
using System;

[Tool]
[GlobalClass]
public partial class SkillUI : Panel
{

    [Export]
    public String SkillName
    {
        get => GetButton().Text;
        set => GetButton().Text = value;
    }

    public Button GetButton()
    {
        return GetNode<Button>("Button");
    }
}
