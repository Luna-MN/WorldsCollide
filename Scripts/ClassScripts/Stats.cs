using Godot;
using System;
[GlobalClass]
public partial class Stats : Resource
{
    private event Action onStatCalc;
    [ExportGroup("Stats")] 
    [Export] private float _speed = 200f;
    [Export] private float _currentHealth = 100f;
    [Export] private float _maxHealth = 100f;
    [Export] private float _armour = 0.0f;
    [Export] private float _damageIncrease = 1.1f;
    [Export] private float _itemFind = 1.0f;
    [Export] private float _critChance = 0.5f;
    [Export] private float _critDamage = 1.25f;
    
    #region getter/setter
    public float Speed
    {
        get => _speed;
        set => _speed = value;
    }
    public float CurrentHealth
    {
        get => _currentHealth;
        set => _currentHealth = value;
    }
    public float MaxHealth
    {
        get => _maxHealth;
        set => _maxHealth = value;
    }
    public float Armour
    {
        get => _armour;
        set => _armour = value;
    }
    public float DamageReductionMultiplier => ArmourFormula();

    public float DamageIncrease
    {
        get => _damageIncrease;
        set => _damageIncrease = value;
    }
    public float ItemFind
    {
        get => _itemFind;
        set => _itemFind = value;
    }
    public float CritChance
    {
        get => _critChance;
        set => _critChance = value;
    }
    public float CritDamage
    {
        get => _critDamage;
        set => _critDamage = value;
    }
    #endregion
    
    #region Formulas
    private float ArmourFormula()
    {
        return 100.0f / (100.0f + _armour);
    }
    #endregion
    
    
}
