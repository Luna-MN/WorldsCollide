using Godot;
using System;

public partial class TerrainChanger : Node2D
{
    [Export] private TerrainTileMap TileMap;
    private RandomNumberGenerator rng;
    public override void _Ready()
    {
        if (!Multiplayer.IsServer())
        {
            return;
        }
        rng = new RandomNumberGenerator();
        Timer t = new Timer()
        {
            WaitTime = 0.5,
            OneShot = false,
            Autostart = true,
            Name = "TileChangerTimer"
        };
        t.Timeout += () =>
        {
            if (!Multiplayer.IsServer())
            {
                t.QueueFree();
                return;
            }
            // TileMap.SetInternalTile(new Vector2I(rng.RandiRange(-8, 8), rng.RandiRange(-8, 8)),1, new Vector2I(rng.RandiRange(0, 1), rng.RandiRange(0, 1)));
            TileMap.SetInternalTiles([new Vector2I(rng.RandiRange(-8, 8), rng.RandiRange(-8, 8)), new Vector2I(rng.RandiRange(-8, 8), rng.RandiRange(-8, 8))], [1, 1], 
                [new Vector2I(rng.RandiRange(0, 1), rng.RandiRange(0, 1)), new Vector2I(rng.RandiRange(0, 1), rng.RandiRange(0, 1))], [0, 0]);
        };
        this.AddChild(t);
    }
}
