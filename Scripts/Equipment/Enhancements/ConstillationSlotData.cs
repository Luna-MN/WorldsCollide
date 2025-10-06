using Godot;
using System;
[GlobalClass]
public partial class ConstillationSlotData : Resource
{
    [Export] public BaseEnhancement Star;
    [Export] public int[] connectionIndexes;
}
