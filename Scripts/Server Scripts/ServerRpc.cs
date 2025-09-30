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
    public void UpdatePlayerEquipment(int id, int[] equipmentIds, int[] InvIds)
    {
        var player = ServerManager.NodeDictionary[id];
        GD.Print(string.Join(", ", equipmentIds));
        GD.Print(string.Join(", ", InvIds));
        var equipment = new List<BaseEquipment>();
        var inv = new List<BaseEquipment>();
        var allEquipment = player.inventory.AllEquipment.ToList();
        allEquipment.AddRange(player.EquipmentSlots.Where(x => x.EquippedEquipment != null).Select(x => x.EquippedEquipment));
        var allEquipmentIds = allEquipment.Select(x => x.ItemId).ToList();
        foreach (var equipId in equipmentIds)
        {
            if(equipId == -1) continue;
            var equip = allEquipment.Find(x => x.ItemId == equipId);
            if (allEquipmentIds.Contains(equipId))
            {
                player.EquipmentSlots[equipmentIds.ToList().IndexOf(equipId)].EquippedEquipment = equip;
            }
        }
        List<BaseEquipment> Inven = new();
        foreach (var invId in InvIds)
        {
            if (allEquipmentIds.Contains(invId))
            {
                Inven.Add(allEquipment.Find(x => x.ItemId == invId));
            }
        }
        player.inventory.AllEquipment = Inven.ToArray();
        player.equipAll();
    }
    #endregion
}