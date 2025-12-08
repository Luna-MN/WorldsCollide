using Godot;
using System;

public partial class AwaitVarTime : BTAction
{
    public override Status _Tick(double delta)
    {
        Wait();
        return Status.Success;
    }

    public async void Wait()
    {
        Timer timer = new Timer()
        {
            WaitTime = (float)Blackboard.GetVar("Await_var"),
            Autostart = true,
            OneShot = true
        };
        var parent = (Node2D)Blackboard.GetVar("Parent_var");
        parent.AddChild(timer);
        await ToSignal(timer, "timeout");
        timer.QueueFree();
        Blackboard.SetVar("Await_var", 0);
    }
}
