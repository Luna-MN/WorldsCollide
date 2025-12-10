using Godot;
using System;
using System.Linq;

public partial class ClassRpc : Node2D
{
    [ExportGroup("Class1")]
    [Export]
    public PackedScene Shuriken;
    [Export]
    public PackedScene CircleShuriken;
    
    #region Class1
    
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable)]
        public void Class1_ShurikenThrow(string id)
        {
            var Character = ServerManager.NodeDictionary[id];
            var mousePos = Character.inputSync.mousePosition;
            var bullet1 = Shuriken.Instantiate<BasicRangedProjectile>();
            var bullet2 = Shuriken.Instantiate<BasicRangedProjectile>();
            var bullet3 = Shuriken.Instantiate<BasicRangedProjectile>();
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
        public void Class1_Blink(string id)
        {
            var Character = ServerManager.NodeDictionary[id];
            var mousePos = Character.inputSync.mousePosition;
            var MoveDirection = mousePos - Character.GlobalPosition;
            Character.GlobalPosition += MoveDirection.Normalized() * 500;
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable)]
        public void Class1_CircleShuriken(string id)
        {
            var Character = ServerManager.NodeDictionary[id];
            var mousePos = Character.inputSync.mousePosition;
            var bullet1 = CircleShuriken.Instantiate<Circling_Projectile>();
            bullet1.GlobalPosition = Character.GlobalPosition;
            bullet1.angle = 0;
            bullet1.character = Character;
            bullet1.GlobalPosition = Character.GlobalPosition + Vector2.FromAngle((float)bullet1.angle) * bullet1.distance;
            bullet1.Id = id;
            bullet1.Name = id + "_Bullet1";
            bullet1.obtainStats(Character);
            ServerManager.spawner.AddChild(bullet1, true);
            var bullet2 = CircleShuriken.Instantiate<Circling_Projectile>();
            bullet2.GlobalPosition = Character.GlobalPosition;
            bullet2.angle = 2*double.Pi*2/3;
            bullet2.character = Character;
            bullet2.GlobalPosition = Character.GlobalPosition + Vector2.FromAngle((float)bullet2.angle) * bullet2.distance;
            bullet2.Id = id;
            bullet2.Name = id + "_Bullet2";
            bullet2.obtainStats(Character);
            ServerManager.spawner.AddChild(bullet2, true);
            var bullet3 = CircleShuriken.Instantiate<Circling_Projectile>();
            bullet3.GlobalPosition = Character.GlobalPosition;
            bullet3.angle = 2*double.Pi*1/3;
            bullet3.character = Character;
            bullet3.GlobalPosition = Character.GlobalPosition + Vector2.FromAngle((float)bullet3.angle) * bullet3.distance;
            bullet3.Id = id;
            bullet3.Name = id + "_Bullet3";
            bullet3.obtainStats(Character);
            ServerManager.spawner.AddChild(bullet3, true);
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

    #region Gunslinger
        [ExportGroup("Gunslinger")]
        [Export]
        public PackedScene CapTrap;
        [Export]
        public PackedScene FloorFire;
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable)]
        public void Gunslinger_Skill1(string id)
        {
            var character = (Gunslinger)ServerManager.NodeDictionary[id];
            foreach (var c in character.charactersIn)
            {
                if (c == character) continue;
                if (character.healingShots == false && c is not Enemy)
                {
                    continue;
                }
                ServerManager.EquipmentRpcs.RpcId(1, character.PrimaryEquipment[0].WeaponName+"_LeftClick", character.Name.ToString(), character.PrimaryEquipment[0].ItemId, c.GlobalPosition);
            }
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable)]
        public void Gunslinger_Skill2(string id)
        {
            var cap = CapTrap.Instantiate<CapTrap>();
            var player = ServerManager.NodeDictionary[id];
            cap.summoner = player;
            cap.GlobalPosition = player.inputSync.mousePosition;
            cap.EquipmentSlots[0].EquippedEquipment = player.PrimaryEquipment[0];
            while (true)
            {
                var capID = id + "_" + new Random().Next(1000000000);
                if (!ServerManager.NodeDictionary.ContainsKey(capID))
                {
                    cap.ID = capID;
                    cap.Name = cap.ID;
                    ServerManager.NodeDictionary[cap.ID] = cap;
                    break;
                }
            }
            cap.equipAll();
            ServerManager.spawner.AddChild(cap, true);
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable)]
        public void Gunslinger_Skill3(string id)
        {
            var character = (Gunslinger)ServerManager.NodeDictionary[id];
            var handler = (Node2D b,Projectile p,float d) => Gunslinger_OnHitSkill3(id, b, p, d);
            character.OnHitSkill += handler;
            var timer = new Timer()
            {
                Autostart = true,
                OneShot = true,
                WaitTime = 10f,
                Name = "Skill3Timer"
            };
            timer.Timeout += () =>
            {
                character.OnHitSkill -= handler;
                timer.QueueFree();
            };
            character.AddChild(timer);
        }
        public void Gunslinger_OnHitSkill3(string Id, Node2D Body, Projectile proj, float damage)
        {
            // create floor fire on an enemy
            var createdFire = FloorFire.Instantiate<FloorFire>();
            createdFire.GlobalPosition = Body.GlobalPosition;
            createdFire.Name = Id + "_" + new Random().Next(100000);
            ServerManager.spawner.CallDeferred("add_child", createdFire, true);
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable)]
        public void Gunslinger_Skill4(string id)
        {
            var character = (Gunslinger)ServerManager.NodeDictionary[id];
            var statCalc = (float f) =>
            {
                // change to be based on a stat or something
                return (float)(f * 1.01);
            };
            character.characterStats.addFunc(id, StatMaths.StatNum.attackSpeed, "Skill4", statCalc, 6);
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable)]
        public void Gunslinger_Skill5(string id)
        {
            var character = (Gunslinger)ServerManager.NodeDictionary[id];
            foreach (var primaryWeaponAttack in character.PrimaryEquipment.SelectMany(primaryWeapon => primaryWeapon.Attacks))
            {
                primaryWeaponAttack.RicochetCount += 1;
            }
            foreach (var secondaryEquipmentAttack in character.SecondaryEquipment.SelectMany(secondaryEquipment => secondaryEquipment.Attacks))
            {
                secondaryEquipmentAttack.RicochetCount += 1;
            }
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable)]
        public void Gunslinger_Skill6(string id)
        {
            var character = (Gunslinger)ServerManager.NodeDictionary[id];
            if (Multiplayer.IsServer() && character.healingShots == false)
            {
                character.healingShots = true;
                ServerManager.ClassRpcs.Rpc("Gunslinger_Skill6", id);
            }
            else
            {
                character.healingShots = false;
            }
        }
    #endregion
}
