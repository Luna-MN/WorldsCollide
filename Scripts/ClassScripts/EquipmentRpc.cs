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
    [Export]
    public PackedScene Arrow;
    [ExportGroup("Swords")]
    [Export]
    public PackedScene Melee_Slash;
    // on ready dynamically load all equipment and enhancments into the arrays
    public override void _Ready()
    {
        var equipmentDir = DirAccess.Open("res://Resources/Equipment/");
        if (equipmentDir != null)
        {
            equipmentDir.ListDirBegin();
            var fileName = equipmentDir.GetNext();
        
            while (!string.IsNullOrEmpty(fileName))
            {
                if (!fileName.StartsWith(".") && fileName.EndsWith(".tres"))
                {
                    var fullPath = "res://Resources/Equipment/" + fileName;
                    var loadedEquipment = ResourceLoader.Load(fullPath) as BaseEquipment;
                    if (loadedEquipment != null)
                    {
                        var exists = equipment.ToList().FindIndex(x => x.ResourceName == loadedEquipment.ResourceName);
                        if (exists == -1)
                        {
                            Array.Resize(ref equipment, equipment.Length + 1);
                            equipment[equipment.Length - 1] = loadedEquipment;
                        }
                    }
                }
            
                fileName = equipmentDir.GetNext();
            }
        
            equipmentDir.ListDirEnd();
        }
        
        var EnhancmentsDir = DirAccess.Open("res://Resources/Enhancments/");
        if (EnhancmentsDir != null)
        {
            EnhancmentsDir.ListDirBegin();
            var fileName = EnhancmentsDir.GetNext();
        
            while (!string.IsNullOrEmpty(fileName))
            {
                if (!fileName.StartsWith(".") && fileName.EndsWith(".tres"))
                {
                    var fullPath = "res://Resources/Enhancments/" + fileName;
                    var enhancement = ResourceLoader.Load(fullPath) as BaseEnhancement;
                    if (enhancement != null)
                    {
                        var exists = Enhancments.ToList().FindIndex(x => x.ResourceName == enhancement.ResourceName);
                        if (exists == -1)
                        {
                            Array.Resize(ref Enhancments, Enhancments.Length + 1);
                            Enhancments[equipment.Length - 1] = enhancement;
                        }
                    }
                }
            
                fileName = EnhancmentsDir.GetNext();
            }
        
            EnhancmentsDir.ListDirEnd();
        }
    }
    
    [Rpc(MultiplayerApi.RpcMode.Authority)]
    public void AddEquipmentToInv(int[] EnhancmentIndexes, int equipmentId, int ItemId, int Rarity)
    {
        GD.Print(equipmentId);
        GD.Print(string.Join(", ", EnhancmentIndexes));
        var currentEquipment = GameManager.player.inventory.AllEquipment;
        Array.Resize(ref currentEquipment, currentEquipment.Length + 1);
        var eq = equipment[equipmentId].Duplicate() as BaseEquipment;
        eq.enhancements = EnhancmentIndexes.Select(x => Enhancments[x]).ToArray();
        eq.ItemId = ItemId;
        eq.Rarity = (EquipmentGenerator.Rarity)Rarity;
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

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferChannel = 1,
        TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable)]
    public void Bow_LeftClick(int id)
    {
        var Character = ServerManager.NodeDictionary[id];
        var mousePos = Character.inputSync.mousePosition;
        var arrow = Arrow.Instantiate<BasicRangedProjectile>();
        arrow.MoveDirection = mousePos - Character.GlobalPosition;
        arrow.GlobalPosition = Character.ShootPosition.GlobalPosition;
        arrow.Id = id;
        arrow.Name = id + "_Arrow";
        arrow.deleteOnHit = false;
        arrow.obtainStats(Character);
        ServerManager.spawner.AddChild(arrow, true);
    }
}
