using Godot;
using System;
[Tool]
public partial class SkillUIPanel : SkillUI
{
    [Export]
    public Texture2D Icon
    {
        get => GetButton().Icon;
        set => GetButton().Icon = value;
    }
}
