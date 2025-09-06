using Godot;
using System;

public partial class ClassRpc : Node2D
{
    #region Class1

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
