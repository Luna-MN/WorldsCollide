using Godot;
using System;

public partial class TerrainChanger : Node2D
{
    [Export] private TerrainTileMap TileMap;
    private RandomNumberGenerator rng;
    public override void _Ready()
    {
        base._Ready();
        rng = new RandomNumberGenerator();
        Timer t = new Timer()            
        {
            WaitTime = 2,
            OneShot = false,
            Autostart = true,
            Name = "TileChangerTimer"
        };
        t.Timeout += () =>
        {
            TileMap.SetInternalTile(new Vector2I(rng.RandiRange(-8, 8), rng.RandiRange(-8, 8)),1, new Vector2I(rng.RandiRange(0, 1), rng.RandiRange(0, 1)));
        };
        this.AddChild(t);
    }
}
