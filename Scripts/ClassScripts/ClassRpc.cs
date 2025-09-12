using Godot;
using System;

public partial class ClassRpc : Node2D
{
    [ExportGroup("Class1")]
    [Export]
    public PackedScene Shuriken;
    
    #region Class1
    
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable)]
        public void Class1_ShurikenThrow(int id)
        {
            var Character = ServerManager.NodeDictionary[id];
            var mousePos = Character.inputSync.mousePosition;
            var bullet1 = Shuriken.Instantiate<Gun_Bullet>();
            var bullet2 = Shuriken.Instantiate<Gun_Bullet>();
            var bullet3 = Shuriken.Instantiate<Gun_Bullet>();
            bullet1.MoveDirection = mousePos - Character.GlobalPosition;
            var orth = bullet1.MoveDirection.Orthogonal();
            orth = orth.Normalized();
            bullet2.MoveDirection = mousePos - Character.GlobalPosition + orth * 25;
            bullet3.MoveDirection = mousePos - Character.GlobalPosition - orth *25;
            bullet1.GlobalPosition = Character.GlobalPosition;
            bullet2.GlobalPosition = Character.GlobalPosition;
            bullet3.GlobalPosition = Character.GlobalPosition;
            bullet1.Id = id;
            bullet2.Id = id;
            bullet3.Id = id;
            bullet1.Name = id + "_Bullet1";
            bullet2.Name = id + "_Bullet2";
            bullet3.Name = id + "_Bullet3";
            bullet1.obtainStats(Character);
            bullet2.obtainStats(Character);
            bullet3.obtainStats(Character);
            ServerManager.spawner.AddChild(bullet1, true);
            ServerManager.spawner.AddChild(bullet2, true);
            ServerManager.spawner.AddChild(bullet3, true);
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
