using Godot;
using System;

public partial class ClassRpc : Node2D
{
    #region Class1
    
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable)]
        public void Class1_Skill1(int id)
        {
            
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable)]
        public void Class1_Skill2(int id)
        {
            
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
