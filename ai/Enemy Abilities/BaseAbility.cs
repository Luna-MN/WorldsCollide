using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class BaseAbility : Resource
{
    [Export]
    public int Priority;
    public virtual int UpdatePriority(Character enemy)
    {
        return 100;
    }
    public virtual bool Active(List<Character> Targets)
    {
        return false;
    }
}
