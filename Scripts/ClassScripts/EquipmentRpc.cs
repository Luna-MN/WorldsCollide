using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class EquipmentRpc : Node2D
{
    [ExportGroup("Generic")]
    [Export]
    public BaseEnhancement[] Enhancments;
    [Export]
    public BaseEquipment[] equipment;
    
    [ExportGroup("Ranged Bullets")]
    [Export]
    public PackedScene Basic_Bullet;
    
    [ExportGroup("Swords")]
    [Export]
    public PackedScene Melee_Slash;
    // on ready dynamically load all equipment and enhancments into the arrays

    [Rpc(MultiplayerApi.RpcMode.Authority)]
    public void AddEquipmentToInv(int[] EnhancmentIndexes, int equipmentId, int ItemId)
    {
        GD.Print(equipmentId);
        GD.Print(string.Join(", ", EnhancmentIndexes));
        var currentEquipment = GameManager.player.inventory.AllEquipment;
        Array.Resize(ref currentEquipment, currentEquipment.Length + 1);
        var eq = equipment[equipmentId].Duplicate() as BaseEquipment;
        eq.enhancements = EnhancmentIndexes.Select(x => Enhancments[x]).ToArray();
        eq.ItemId = ItemId;
        currentEquipment[currentEquipment.Length - 1] = eq;
        GameManager.player.inventory.AllEquipment = currentEquipment;
    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable)]
    public void Gun_LeftClick(int id)
    {
        var Character = ServerManager.NodeDictionary[id];
        var mousePos = Character.inputSync.mousePosition;
        var bullet = Basic_Bullet.Instantiate<BasicRangedProjectile>();
        bullet.MoveDirection = mousePos - Character.GlobalPosition;
        bullet.GlobalPosition = Character.ShootPosition.GlobalPosition;
        bullet.Id = id;
        bullet.Name = id + "_Bullet";
        bullet.obtainStats(Character);
        ServerManager.spawner.AddChild(bullet, true);
    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable)]
    public void SlashingSword_LeftClick(int id)
    {
        var Character = ServerManager.NodeDictionary[id];
        var mousePos = Character.inputSync.mousePosition;
        var bullet = Melee_Slash.Instantiate<Melee_Slash>();
        bullet.MoveDirection = mousePos - Character.GlobalPosition;
        bullet.GlobalPosition = Character.ShootPosition.GlobalPosition;
        bullet.Id = id;
        bullet.Name = id + "_Bullet";
        bullet.character = Character;
        bullet.obtainStats(Character);
        ServerManager.spawner.AddChild(bullet, true);
    }
    
}
