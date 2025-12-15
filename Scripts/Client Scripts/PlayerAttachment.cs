using Godot;
using System;
using System.Linq;

public partial class PlayerAttachment : Node2D
{
    #region Input Handling
        public SkillOnScreen TB1, TB2, TB3, TB4, TB5;
        private Character player;
        [Export] public InputSync inputSync;
        public override void _Ready()
        {
            player = GetParent<Character>();
            if (GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
            {   
                //GameManager.player = this;
                TB1 = GetNode<SkillOnScreen>("/root/Node2D2/Camera2D/CanvasLayer/Control/HBoxContainer/1");
                TB1.SetIcon();
                TB2 = GetNode<SkillOnScreen>("/root/Node2D2/Camera2D/CanvasLayer/Control/HBoxContainer/2");
                TB2.SetIcon();
                TB3 = GetNode<SkillOnScreen>("/root/Node2D2/Camera2D/CanvasLayer/Control/HBoxContainer/3");
                TB3.SetIcon();
                TB4 = GetNode<SkillOnScreen>("/root/Node2D2/Camera2D/CanvasLayer/Control/HBoxContainer/4");
                TB4.SetIcon();
                TB5 = GetNode<SkillOnScreen>("/root/Node2D2/Camera2D/CanvasLayer/Control/HBoxContainer/5");
                TB5.SetIcon(true);
                player.healthBar.Visible = false;
            }
        }
    public override void _UnhandledInput(InputEvent @event)
    {
        if (GetMultiplayerAuthority() != Multiplayer.GetUniqueId()) return;
        if (Input.IsActionJustPressed("skill_1"))
        {
            if(player.skills[player.selectedSkillIndexes[0]].IsPassive) return;
            player.Skill1();
            SetUI(TB1);
        }
        if (Input.IsActionJustPressed("skill_2"))
        {
            if (player.skills[player.selectedSkillIndexes[1]].IsPassive) return;
            player.Skill2();
            SetUI(TB2);
        }

        if (Input.IsActionJustPressed("skill_3"))
        {
            if (player.skills[player.selectedSkillIndexes[2]].IsPassive)
            {
                GD.Print("3 is passive");
                return;
            }
            player.Skill3();
            SetUI(TB3);
        }

        if (Input.IsActionJustPressed("skill_4"))
        {
            if (player.skills[player.selectedSkillIndexes[3]].IsPassive) return;
            player.Skill4();
            SetUI(TB4);
        }

        if (Input.IsActionJustPressed("skill_5"))
        {
            player.Skill5();
            SetUI(TB5);
        }
        if (Input.IsActionJustReleased("skill_1"))
        {
            ResetUI(TB1);
        }
        if (Input.IsActionJustReleased("skill_2"))
        {
            ResetUI(TB2);
        }

        if (Input.IsActionJustReleased("skill_3"))
        {
            ResetUI(TB3);
        }
        if (Input.IsActionJustReleased("skill_4"))
        {
            ResetUI(TB4);
        }
        if (Input.IsActionJustReleased("skill_5"))
        {
            ResetUI(TB5);
        }

        if (@event is InputEventMouseButton Button)
        {
            if (Button.ButtonIndex == MouseButton.Left)
            {
                if (Button.Pressed)
                {
                    if (player.PrimaryEquipment.Count > 0)
                    {
                        foreach (var wep in player.PrimaryEquipment)
                        {
                            var attack = wep.Attack(player, true, wep.Attacks[0], PrimaryWeapon.EventAction.click);
                            if (attack)
                            {
                                player.LeftClick(wep, inputSync.mousePosition);
                            }
                        }

                    }
                }
                else
                {
                    if (player.PrimaryEquipment.Count > 0)
                    {
                        foreach (var wep in player.PrimaryEquipment)
                        {
                            var attack = wep.Attack(player, true, wep.Attacks[0], PrimaryWeapon.EventAction.unclick);
                            if (attack)
                            {
                                player.LeftClick(wep, inputSync.mousePosition);
                            }
                        }
                    }
                }

            }

            if (Button.ButtonIndex == MouseButton.Right)
            {
                if (Button.Pressed)
                {
                    if (player.SecondaryEquipment.Count > 0)
                    {
                        foreach (var wep in player.SecondaryEquipment)
                        {
                            if (wep.Attacks.Length > 1)
                            {
                                var attack = player.SecondaryEquipment.First().Attack(player, false, wep.Attacks[1], PrimaryWeapon.EventAction.click);
                                if (attack)
                                {
                                    player.RightClick(wep, inputSync.mousePosition);
                                }
                            }
                            else
                            {
                                var attack = player.SecondaryEquipment.First().Attack(player, false, wep.Attacks[0], PrimaryWeapon.EventAction.click);
                                if (attack)
                                {
                                    player.RightClick(wep, inputSync.mousePosition);
                                }
                            }

                        }
                    }

                }
                else
                {
                    foreach (var wep in player.SecondaryEquipment)
                    {
                        if (wep.Attacks.Length > 1)
                        {
                            var attack = wep.Attack(player, false, wep.Attacks[1], PrimaryWeapon.EventAction.unclick);
                            if (attack)
                            {
                                player.RightClick(wep, inputSync.mousePosition);
                            }
                        }
                        else
                        {
                            var attack = wep.Attack(player, false, wep.Attacks[0], PrimaryWeapon.EventAction.unclick);
                            if (attack)
                            {
                                player.RightClick(wep, inputSync.mousePosition);
                            }
                        }
                    }
                }
            }
        }
    }
    public override void _Process(double delta)
    {
        if (GameManager.UIOpen)
        {
            return;
        }
        base._Process(delta);
        if (GetMultiplayerAuthority() != Multiplayer.GetUniqueId()) return;
        if (inputSync.moveInput.X < 0)
        {
            player.Sprite.FlipH = true;
        }
        else if (inputSync.moveInput.X > 0)
        {
            player.Sprite.FlipH = false;
        }
        if (player.WepSprite.SpriteFrames != null && player.PrimaryEquipment.Count > 0)
        {
            player.WepSprite.Position = player.Sprite.FlipH ? new Vector2(-player.GunPos.X, player.GunPos.Y) : player.GunPos;
            
            player.WepSprite.LookAt(inputSync.mousePosition);
            if (player.WepSprite.RotationDegrees > 360)
            {
                player.WepSprite.RotationDegrees -= 360;
            }

            if (player.WepSprite.RotationDegrees < 0)
            {
                player.WepSprite.RotationDegrees += 360;
            }

            if (player.WepSprite.RotationDegrees > 90 && player.WepSprite.RotationDegrees < 270)
            {
                player.WepSprite.FlipV = true;
                player.ShootPosition.Position = player.PrimaryEquipment[0].ShootPos;
            }
            else
            {
                player.WepSprite.FlipV = false;
                player.ShootPosition.Position = player.PrimaryEquipment[0].FlippedPos;
            }
        }
        if (player.OffHandSprite.SpriteFrames != null && player.SecondaryEquipment.Count > 0)
        {
            player.OffHandSprite.Position = player.Sprite.FlipH ? new Vector2(-player.OffHandPos.X, player.OffHandPos.Y) : player.OffHandPos;
            
            player.OffHandSprite.LookAt(inputSync.mousePosition);
            if (player.OffHandSprite.RotationDegrees > 360)
            {
                player.OffHandSprite.RotationDegrees -= 360;
            }

            if (player.OffHandSprite.RotationDegrees < 0)
            {
                player.OffHandSprite.RotationDegrees += 360;
            }

            if (player.OffHandSprite.RotationDegrees > 90 && player.OffHandSprite.RotationDegrees < 270)
            {
                player.OffHandSprite.FlipV = true;
                player.OffHandShootPosition.Position = player.SecondaryEquipment[0].ShootPos;
            }
            else
            {
                player.OffHandSprite.FlipV = false;
                player.OffHandShootPosition.Position = player.SecondaryEquipment[0].FlippedPos;
            }
        }



        foreach (var wep in player.PrimaryEquipment)
        {
            if (wep.Attacks[0].Event)
            {
                if (wep.Attacks[0].Available)
                {
                    var attack = wep.Attack(player, true, wep.Attacks[0], PrimaryWeapon.EventAction.fire);
                    if (attack)
                    {
                        player.LeftClick(wep, inputSync.mousePosition);
                    } 
                }
            }
        }
        
        foreach (var wep in player.SecondaryEquipment)
        {
            if (wep.Attacks.Length > 1)
            {
                if (wep.Attacks[1].Event)
                {
                    if (wep.Attacks[1].Available)
                    {
                        var attack = wep.Attack(player, false, wep.Attacks[1], PrimaryWeapon.EventAction.fire);
                        if (attack)
                        {
                            player.RightClick(wep, inputSync.mousePosition);
                        }
                    }
                }
            }
            else if (wep.Attacks[0].Event)
            {
                if (wep.Attacks[0].Available)
                {
                    var attack = wep.Attack(player, false, wep.Attacks[0], PrimaryWeapon.EventAction.fire);
                    if (attack)
                    {
                        player.RightClick(wep, inputSync.mousePosition);
                    } 
                }
            }
        }
    }
    public void Move(float delta)
    {
        // Godot: up is negative Y
        Vector2 input = inputSync.moveInput.Normalized();
        if (input != Vector2.Zero)
        {
            player.Position += input.Normalized() * player.stats[StatMaths.StatNum.speed] * delta;
            player.OnMoveToggle(true);
        }
        else
        {
            if (player.PassiveMoveTimers != null && player.PassiveMoveTimers.Count > 0)
            {
                player.PassiveMoveTimers.ForEach(x => x.Stop());
            }
            player.OnMoveToggle(false);
        }
    }
    public override void _PhysicsProcess(double delta)
    {
        if (Multiplayer.IsServer())
        {
            Move((float)delta);
        }
        else if (inputSync != null && Multiplayer.GetUniqueId() == Convert.ToInt32(player.Name))
        {
            inputSync.mousePosition = GetGlobalMousePosition();
        }
        player.TargetPosition = inputSync.mousePosition;
    }
    protected virtual void SetUI(Button button)
    {
        button.Modulate = new Color(0, 0, 1);
    }
    protected virtual void ResetUI(Button button)
    {
        button.Modulate = new Color(1, 1, 1);
    }
    #endregion
}
