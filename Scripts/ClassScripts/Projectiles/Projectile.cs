using Godot;
using System;
[GlobalClass]
public partial class Projectile : Node2D
{
    public Vector2 MoveDirection;
    [Export]
    public long Id;
    [Export]
    public Area2D Area;
    
    [ExportGroup("Properties")]
    [Export]
    public float Damage = 10.0f;
    [Export]
    public float Speed = 400f;
    [Export]
    public float RangeTime = 2f;

    public float damageDone;
    public float amountOfTimes = 1;
    protected Timer timer;
    private bool crit;
    
    public override void _Ready()
    {
        // Bullet timeout
        timer = new Timer()
        {
            WaitTime = RangeTime,
            OneShot = true,
            Autostart = true,
            Name = "Timer"
        };
        timer.Timeout += () =>
        {
            if(!Multiplayer.IsServer()) return;
            QueueFree();
        };
        AddChild(timer);
        LookAt(GlobalPosition + MoveDirection);
        
        //On Bullet Hit
        Area.BodyEntered += OnBulletHit;
    }
    
    public override void _EnterTree()
    {
        SetMultiplayerAuthority(1);
    }

    protected virtual void OnBulletHit(Node2D Body)
    {
        if(Name.ToString().Contains(Body.Name.ToString())) return;
        if(!Multiplayer.IsServer()) return;

        if (ServerManager.NodeDictionary[(int)Id] != null && ServerManager.NodeDictionary[(int)Id] is Character bulletOwner)
        {
            bulletOwner.CallOnHit(Body, this, Damage);
            if (crit)
            {
                bulletOwner.CallOnCrit(Body);
            }
        }

        var modifiedDamage = Damage;
        if (Body is Character hitChar)
        {
            hitChar.TakeDamage(Damage * amountOfTimes, (int)Id);
            modifiedDamage = Damage * hitChar.characterStats[StatMaths.StatNum.armour];
        }
        ((Character)Body).DamageText(modifiedDamage, amountOfTimes);
    }

    public void obtainStats(Character c)
    {
        Damage *= c.characterStats[StatMaths.StatNum.damageIncrease];
        var rng = new RandomNumberGenerator();
        if (rng.Randf() < c.characterStats[StatMaths.StatNum.critChance])
        {
            Damage *= c.characterStats[StatMaths.StatNum.critDamage];
            crit = true;
        }
    }
}
