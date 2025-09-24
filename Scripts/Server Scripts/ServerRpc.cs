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

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void UpdatePlayerEquipment(int id, int[] equipmentIds, Godot.Collections.Dictionary<int, int[]> enhancmentIds, int[] InvIds)
    {
        GD.Print(string.Join(", ", equipmentIds));
        GD.Print(string.Join(", ", InvIds));
        GD.Print(string.Join(", ", enhancmentIds.Select(x => equipmentIds[x.Key] + " (" + string.Join("& ", x.Value) + ")")));
        var equipment = equipmentIds.Select(x => x != -1 ? ServerManager.EquipmentRpcs.equipment[x].Duplicate() as BaseEquipment : null).ToArray();
        foreach (var equipmentPair in enhancmentIds)
        {
            if (equipmentIds[equipmentPair.Key] == -1) continue;
            var enhancments = equipmentPair.Value.Select(x => ServerManager.EquipmentRpcs.Enhancments[x]).ToArray();
            equipment[equipmentPair.Key].enhancements = enhancments;
        }
        var Inv = InvIds.Select(x => x != -1 ? ServerManager.EquipmentRpcs.equipment[x].Duplicate() as BaseEquipment : null).ToArray();
        
        var player = ServerManager.NodeDictionary[id];
        foreach (EquipmentSlot equipmentSlot in player.EquipmentSlots)
        {
            var index = player.EquipmentSlots.ToList().IndexOf(equipmentSlot);
            if (index == -1) continue;
            equipmentSlot.EquippedEquipment = equipment[index];
        }
        player.inventory.AllEquipment = Inv;
        player.equipAll();
    }
    #endregion
}