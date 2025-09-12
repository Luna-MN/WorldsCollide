using Godot;
using System;

public partial class ClassRpc : Node2D
{
    [ExportGroup("Class1")]
    [Export]
    public PackedScene Shuriken;
    
    #region Class1
    
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable)]
        public void Class1_Shuriken(int id)
        {
            var Character = ServerManager.NodeDictionary[id];
            var mousePos = Character.inputSync.mousePosition;
            var bullet = Shuriken.Instantiate<Class1_Shuriken>();
            bullet.MoveDirection = mousePos - Character.GlobalPosition;
            bullet.GlobalPosition = Character.GlobalPosition;
            bullet.Id = id;
            bullet.Name = id + "_Bullet";
            bullet.obtainStats(Character);
            ServerManager.spawner.AddChild(bullet, true);
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable)]
        public void Class1_Blink(int id)
        {
            var Character = ServerManager.NodeDictionary[id];
            var mousePos = Character.inputSync.mousePosition;
            var MoveDirection = mousePos - Character.GlobalPosition;
            Character.GlobalPosition += MoveDirection.Normalized() * 500;
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable)]
        public void Class1_Skill3(int id)
        {
            
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable)]
        public void Class1_Skill4(int id)
        {
            
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable)]
        public void Class1_Skill5(int id)
        {
            
        }
    #endregion

    #region Class2

    

    #endregion
}
