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
            bullet.MoveDirection = mousePos - Character.GlobalPosition;
            bullet.GlobalPosition = Character.GlobalPosition;
            bullet.Id = id;
            ServerManager.spawner.AddChild(bullet, true);
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void Class1_Skill1()
        {
            
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void Class1_Skill2()
        {
            
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void Class1_Skill3()
        {
            
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void Class1_Skill4()
        {
            
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void Class1_Skill5()
        {
            
        }
    #endregion

    #region Class2

    

    #endregion
}
