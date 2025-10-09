using Godot;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

public partial class TerrainTileMap : TileMapLayer
{
	[ExportGroup("Tile Maps")]
	[Export] private TileMapLayer calculationTileMap;
	[Export] private TileMapLayer displayTileMap1;
	[Export] private TileMapLayer displayTileMap2;
	[Export] private TileMapLayer displayTileMap3;
	[Export] private TileMapLayer displayTileMap4;
	[ExportGroup("More Shit")]
	private readonly Vector2I[] internalToDisplay = [new (0,0), new (0,-1), new (-1,-1),  new (-1,0)];
	private readonly Vector2I[] displayToInternal = [new (0,0), new (1,0), new (0,1), new (1,1)];

	//t is transparent, w is wall - though doesn't truly matter
	//view t as white and w and pink on the template
	//read top to bottom left to right
	private readonly Dictionary<Tuple<string, string, string, string>, Vector2I> InternalNeighborsToDisplayAtlasCoords = new ()
	{
		{new Tuple<string, string, string, string>("t", "t", "t", "t"), new Vector2I(0, 3)},
		{new Tuple<string, string, string, string>("t", "t", "t", "w"), new Vector2I(1, 3)},
		{new Tuple<string, string, string, string>("t", "t", "w", "t"), new Vector2I(0, 0)},
		{new Tuple<string, string, string, string>("t", "t", "w", "w"), new Vector2I(3, 0)},
		{new Tuple<string, string, string, string>("t", "w", "t", "t"), new Vector2I(0, 2)},
		{new Tuple<string, string, string, string>("t", "w", "t", "w"), new Vector2I(1, 0)},
		{new Tuple<string, string, string, string>("t", "w", "w", "t"), new Vector2I(2, 3)},
		{new Tuple<string, string, string, string>("t", "w", "w", "w"), new Vector2I(1, 1)},
		{new Tuple<string, string, string, string>("w", "t", "t", "t"), new Vector2I(3, 3)},
		{new Tuple<string, string, string, string>("w", "t", "t", "w"), new Vector2I(0, 1)},
		{new Tuple<string, string, string, string>("w", "t", "w", "t"), new Vector2I(3, 2)},
		{new Tuple<string, string, string, string>("w", "t", "w", "w"), new Vector2I(2, 0)},
		{new Tuple<string, string, string, string>("w", "w", "t", "t"), new Vector2I(1, 2)},
		{new Tuple<string, string, string, string>("w", "w", "t", "w"), new Vector2I(2, 2)},
		{new Tuple<string, string, string, string>("w", "w", "w", "t"), new Vector2I(3, 1)},
		{new Tuple<string, string, string, string>("w", "w", "w", "w"), new Vector2I(2, 1)}
	};
	
	private Dictionary<string, int> TextNameToID = new()
	{
		{ "PinkT", 0 },
		{ "BlackT", 1 },
		{ "PinkWhite", 2 },
		{ "BlackWhite", 3 },
		{ "WhiteT", 4 },
		{ "GreenT", 5 },
	};

	private TileMapLayer getTileMapLayer(int i)
	{
		switch (i)
		{
			case 0:
				return calculationTileMap??this;
			case 1:
				return displayTileMap1;
			case 2:
				return displayTileMap2;
			case 3:
				return displayTileMap3;
			case 4:
				return displayTileMap4;
			default:
				return null;
		}
	}

	public override void _Ready()
	{
		foreach (var coord in getTileMapLayer(0).GetUsedCells())
		{
			// GD.Print(coord);
			setInternalTiles(coord);
		}
		// setInternalTiles(new Vector2I(4, -5));
		
	}

	//Give an Internal Coord - will modify 4 display tiles
	public void setInternalTiles(Vector2I coord)
	{
		//if coord changes need to edit coord around to coord -1 -1
		// GD.Print("internal tile " + coord);
		foreach (var mod in internalToDisplay)
		{
			setDisplayTile (coord + mod);
		}
	}

	//give Display Coord - will modify 1 display tile but look at 4 internals
	public void setDisplayTile(Vector2I coord)
	{
		string[] neighbours = new string[4];
		for (int i = 0; i < displayToInternal.Length; i++)
		{
			var internalCheckCoord = coord + displayToInternal[i];
			var tile = getTileMapLayer(0).GetCellTileData(internalCheckCoord);
			if (tile == null) 
				neighbours[i] = "T";
			else 
				neighbours[i] = tile.GetCustomData("Name").ToString();
			
		}
		setDisplayTileAtlasInfo(coord, neighbours);
	}

	// set display tiles
	private void setDisplayTileAtlasInfo(Vector2I coord, string[] neighbours)
	{
		
		if (neighbours.Length != 4) throw new Exception("Invalid number of neighbors");
		TileInfo[] tiles = colourGroupsToAtlasID(neighbours);
		// if (tiles.Length > 1) GD.Print(coord);
		for (int i = 0; i < tiles.Length; i++)
		{
			TileMapLayer tml = getTileMapLayer(i + 1);
			//deal with alternative tiles
			tml.SetCell(coord, tiles[i].atlasID, tiles[i].atlasCoord);
		}
	}
	
	private TileInfo[] colourGroupsToAtlasID(string[] neighbours)
	{
		string[] names;
		int id;
		var td = new TileInfo();
		switch (neighbours.Distinct().Count())
		{
			//if no neighbours
			case 0: throw new Exception("Invalid neighbors");
			//if 1 neighbour return tilename + transparent atlas
			case 1:
				names = [neighbours.FirstOrDefault(), "T"];
				td.atlasID = namePairToAtlasID(ref names);
				td.atlasCoord = getAtlasCoord(neighbours, names);
				return [td];
			//if 2 neighbours first try find bespoke tilename + tilename atlas
			//otherwise find the two tilename + transparent
			case 2:
				//check for bespoke solution
				names = neighbours.Distinct().ToArray();
				id = namePairToAtlasID(ref names);
				if (id != -1)
				{
					td.atlasID = id;
					td.atlasCoord = getAtlasCoord(neighbours, names);
					return [td];
				}
				goto default;
				//if not find solution with transparent
			default:
				names = neighbours.Distinct().ToArray();
				// GD.Print(names);
				List<TileInfo> tis = new();
				for(int i = 0; i < names.Length; i++)
				{
					// GD.Print(names[i]);
					string[] n = new string[4];
					neighbours.CopyTo(n, 0);
					for(int j = 0; j < n.Length; j++)
					{
						if (n[j] != names[i])
						{
							n[j] = "T";
						}
					}
					// GD.Print(n);
					tis.AddRange(colourGroupsToAtlasID(n));
				}
				return tis.ToArray();
				
		}
	}
	
	private int namePairToAtlasID(ref string[] names)
	{
		if  (names.Length != 2) throw new Exception("Invalid name pair");
		if (TextNameToID.TryGetValue(names.First() + names.Last(), out var AtlasID))
		{
			return AtlasID;
		}
		if (TextNameToID.TryGetValue(names.Last() + names.First(), out AtlasID))
		{
			var temp = names.Last();
			names[1] = names[0];
			names[0] = temp;
			return AtlasID;
		}

		return -1;
	}
	private Vector2I getAtlasCoord(string[] neighbours, string[] names)
	{
		if (names.Length != 2) return new Vector2I(0,0);
		if (neighbours.Length != 4) return new Vector2I(0,0);
		string[] newNeighbours = new string[neighbours.Length];
		for (int i = 0; i < neighbours.Length; i++)
		{
			if (names[0] == neighbours[i])
			{
				newNeighbours[i] = "w";
			} else if (names[1] == neighbours[i])
			{
				newNeighbours[i] = "t";
			}
			else
			{
				newNeighbours[i] = "t";
			}
		}
		return InternalNeighborsToDisplayAtlasCoords[new Tuple<string,string,string,string>(newNeighbours[0], newNeighbours[1], newNeighbours[2], newNeighbours[3])];
	}
}

class TileInfo
{
	public int atlasID;
	public Vector2I atlasCoord;
}
