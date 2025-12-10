using Godot;
using System;
[Tool]
public partial class BasicAiTest : Node2D
{
    public float Roam()
    {

        return 10f;
    }

    public float Idle()
    {
        return 5f;
    }

    public bool CheckPlayersInRange()
    {
        return true;
    }

    public bool CheckAgro()
    {
        return false;
    }
}
