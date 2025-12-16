using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class BasicAiTest : Node2D
{
    [Export] private Area2D PlayerSightRange;
    [Export] private Area2D AgroRange;
    [Export] private BaseAbility[] Abilities;
    public List<Character> playersInSightRange = [];
    public List<Character> playersInAgro = [];
    public Character enemy;
    private readonly RandomNumberGenerator rng = new();

    [Export] public float RoamRadius { get; set; } = 300f; 
    [Export] public float RoamMinRadius { get; set; } = 0f;

    [Export] public bool HasAttacks;
    [Export] public bool HasHealing;
    public override void _Ready()
    {
        PlayerSightRange.BodyEntered += OnPlayerSightBodyEntered;
        PlayerSightRange.BodyExited += OnPlayerSightBodyExited;
        AgroRange.BodyEntered += OnAgroBodyEntered;
        AgroRange.BodyExited += OnAgroBodyExited;
        enemy = GetParent<Character>();
        rng.Randomize();

    }

    #region Checks
        public bool IsServer()
        {
            return Multiplayer.IsServer();    
        }
        public bool CheckPlayersInRange()
        {
            return playersInSightRange.Count > 0;
        }
        public bool CheckAgro()
        {
            return playersInAgro.Count > 0;
        }
        /// <summary>
        /// this is how we dictate what the enemy is doing if it is agro'd
        /// </summary>
        /// <returns>
        /// <param name="WhatAreWeDoing_var">1 = Healing/Defence,
        /// 2 = Attacks, anything else = auto attack</param>
        /// </returns>
        public virtual float WhatAreWeDoing()
        {
            // create a dictionary, EnemySkillClass vs Probability, the class contains a function that recalculates the probability and the function to do the thing
            if(!HasAttacks && !HasHealing) return 0f;
            
            var r = rng.RandiRange(1, 100);
            
            if(enemy.stats[StatMaths.StatNum.maxHealth] / enemy.stats[StatMaths.StatNum.currentHealth] < 0.5f && HasHealing)
            {
                if(r <= 50) return 1f;
            }

            if (r <= 50 && HasAttacks) return 2f;
            
            return 0f;
        }
    #endregion
    #region Non-Agro Actions
        public virtual float Roam()
        {
            // Uniform random point within a ring [RoamMinRadius, RoamRadius]
            float min = Mathf.Max(0f, RoamMinRadius);
            float max = Mathf.Max(min, RoamRadius);

            float angle = rng.RandfRange(0f, Mathf.Tau);
            float u = rng.Randf();
            float r = Mathf.Sqrt(Mathf.Lerp(min * min, max * max, u)); // uniform area distribution

            Vector2 offset = Vector2.FromAngle(angle) * r;
            enemy.GetNode<EnemyAttachment>("EnemyAttachment").targetPos = enemy.GlobalPosition + offset;
            
            float distance = offset.Length();
            GD.Print(offset.Length());
            return distance / enemy.stats[StatMaths.StatNum.speed];
        }
        
        public virtual float Idle()
        {
            return 5f;
        }
    #endregion
    #region AutoAttack
        public float AutoAttack()
        {
            return 30f;
        }
    #endregion
    #region Attacking
    
    #endregion
    #region Healing
    
    #endregion
    #region Area Handlers
        private void OnPlayerSightBodyEntered(Node body)
        {
            if (body is Character c)
            {
                if ((c.alleigence.HasFlag(Flags.Alleigence.Player)) && (c.alleigence.HasFlag(Flags.Alleigence.Minion))) return;
                playersInSightRange.Add((Character)body);
            }
        }
        private void OnPlayerSightBodyExited(Node body)
        {
            if (body is Character c)
            {
                if ((c.alleigence.HasFlag(Flags.Alleigence.Player)) && (c.alleigence.HasFlag(Flags.Alleigence.Minion))) return;
                if (playersInSightRange.Contains((Character)body))
                {
                    playersInSightRange.Remove((Character)body);
                }
            }
        }
        private void OnAgroBodyEntered(Node body)
        {
            if (body is Character c)
            {
                if (c.alleigence.HasFlag(Flags.Alleigence.Player))
                {
                    playersInAgro.Add(c);
                }
                else if (c.alleigence.HasFlag(Flags.Alleigence.Minion))
                {
                    playersInAgro.Add(c.GetNode<MinionAttachment>("MinionControl").summoner);
                }
            }
        }

        private void OnAgroBodyExited(Node body)
        {
            if (body is Character c)
            {
                if (c.alleigence.HasFlag(Flags.Alleigence.Player) && playersInAgro.Contains(c))
                {
                    playersInAgro.Remove(c);
                }
                else if (c.alleigence.HasFlag(Flags.Alleigence.Minion) && playersInAgro.Contains(c.GetNode<MinionAttachment>("MinionControl").summoner))
                {
                    playersInAgro.Remove(c.GetNode<MinionAttachment>("MinionControl").summoner);
                }
            }
        }
        #endregion
}
