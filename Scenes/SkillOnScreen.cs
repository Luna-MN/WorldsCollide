using Godot;
using System;

public partial class SkillOnScreen : Button
{
    public void SetIcon(bool isUlt = false)
    {
        if (!isUlt)
        {
            var sel = GameManager.player.selectedSkillIndexes[Convert.ToInt32(Name) - 1];
            Icon = GameManager.player.skills[sel].Icon;
        }
        else
        {
            Icon = GameManager.player.UltSkill.Icon;
        }
    }
}
