using Godot;
using System;

public partial class ClassRpc : Node2D
{
    #region Class1
    [ExportGroup("Class 1")]
    
    [Export]
    public PackedScene Class1_Bullet;
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void Class1_LeftClick(int id)
    {
        var Character = ServerManager.NodeDictionary[id];
        var mousePos = Character.inputSync.mousePosition;
        var bullet = Class1_Bullet.Instantiate<Class1_Bullet>();
        bullet.MoveDirection = mousePos.Normalized();
        bullet.GlobalPosition = Character.GlobalPosition;
        ServerManager.spawner.AddChild(bullet, true);
    }
    
    #endregion

    #region Class2

    

    #endregion
}
