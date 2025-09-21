using System.Collections.Generic;
using System.Linq;
using Godot;


public partial class ServerRpc : Node2D
{
    [Export]
    public PackedScene Player;
    
    #region Server RPCs
    // Server -> Create a player with a given Id
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void CreateNewPlayer(int id)
    {
        if(id == GameManager.LocalID) return;
        var newPlayer = Player.Instantiate<Player>();
        newPlayer.Name = id.ToString();
        
        ServerManager.spawner.AddChild(newPlayer, true);
        ServerManager.NodeDictionary.Add(id, newPlayer);
        GD.Print("New player created" + id);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void UpdatePlayerEquipment(int id, int[] equipmentIds, int[] InvIds)
    {
        GD.Print(string.Join(", ", equipmentIds));
        GD.Print(string.Join(", ", InvIds));
        var equipment = equipmentIds.Select(x => x != -1 ? ServerManager.EquipmentRpcs.equipment[x] : null).ToArray();
        var Inv = InvIds.Select(x => x != -1 ? ServerManager.EquipmentRpcs.equipment[x] : null).ToArray();
        
        var player = ServerManager.NodeDictionary[id];
        foreach (EquipmentSlot equipmentSlot in player.EquipmentSlots)
        {
            var index = player.EquipmentSlots.ToList().IndexOf(equipmentSlot);
            equipmentSlot.EquippedEquipment = equipment[index];
        }
        player.inventory.AllEquipment = Inv;
        player.equipAll();
    }
    #endregion
}