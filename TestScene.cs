using Godot;
using System;

public partial class TestScene : Node2D
{

    public override void _Ready()
    {
        Multiplayer.ConnectedToServer += TestStuff;
    }

    private void TestStuff()
    {
        GetNode<Player>("Player").IsPlayer = true;
        GetNode<Player>("Player").Name = "Player " + Multiplayer.GetUniqueId();
    }
}
