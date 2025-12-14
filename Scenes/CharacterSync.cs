using Godot;
using System;

public partial class CharacterSync : MultiplayerSynchronizer
{
    [Export]
    public float currentHealth = 0.0f;
    [Export]
    public float maxHealth = 0.0f;
}
