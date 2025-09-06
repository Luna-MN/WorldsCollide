using Godot;
using System;

public partial class EquipmentRpc : Node2D
{
    [ExportGroup("Ranged Bullets")]
    [Export]
    public PackedScene Basic_Bullet;
        
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable)]
    public void Gun_LeftClick(int id)
    {
        var Character = ServerManager.NodeDictionary[id];
        var mousePos = Character.inputSync.mousePosition;
        var bullet = Basic_Bullet.Instantiate<Gun_Bullet>();
        bullet.MoveDirection = mousePos - Character.GlobalPosition;
        bullet.GlobalPosition = Character.GlobalPosition;
        bullet.Id = id;
        ServerManager.spawner.AddChild(bullet, true);
    }
}
