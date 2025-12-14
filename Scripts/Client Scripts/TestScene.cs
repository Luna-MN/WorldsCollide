using Godot;
using System;
using System.Collections.Generic;

public partial class TestScene : Node2D
{
	private Node2D Server;
	public override void _EnterTree()
	{
		try
		{
			Server = new Server();
			GD.Print("I am server");
		}
		catch (Exception e)
		{
			Server = new ServerConnect();
			GD.Print("I am client");
		}
	}

	public override void _Ready()
	{
		AddChild(Server);
		if (Server is ServerConnect)
		{
			GameManager.camera = GetNode<Camera2D>("Camera2D");
		}
	}



}
