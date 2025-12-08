using Godot;

public partial class AwaitVarTime : BTAction
{
    [Export]
    public BBFloat AwaitVar;
    private double waitSeconds;
    private double elapsed;

    public override void _Enter()
    {
        waitSeconds = (float)AwaitVar.GetValue(SceneRoot, Blackboard, 0.0);
    }

    public override Status _Tick(double delta)
    {
        if (waitSeconds <= 0.0)
        {
            return Status.Success;
        }

        elapsed += delta;
        if (elapsed >= waitSeconds)
        {
            Blackboard.SetVar("Await_var", 0.0);
            return Status.Success;
        }

        return Status.Running;
    }

    public override void _Exit()
    {
        elapsed = 0.0;
    }
}