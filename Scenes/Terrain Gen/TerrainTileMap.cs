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
	private bool ignoreRemake = false;

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
	
	private Dictionary<string, int> TextNameToID = new();
	
	/// <summary>
	/// Gets the corresponding tilemap layer. 0 gets the calculation layer.
	/// </summary>
	/// <param name="i">number of layer to get</param>
	/// <returns></returns>
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
	
	/// <summary>
	/// Sets up the Atlas ID dictionary using Atlas Name. Runs update all and sets up changed listening.
	/// </summary>
	public override void _Ready()
	{
		var j = getTileMapLayer(1).TileSet.GetSourceCount();
		for (int i = 0; i < j; i++)
		{
			var id = getTileMapLayer(1).TileSet.GetSourceId(i);
			var name = getTileMapLayer(1).TileSet.GetSource(id).GetName();
			TextNameToID.Add(name, id);
			GD.Print($"{name} : {id}");
		}

		updateAll();
		Changed += limitUpdates;
		// updateInternalTile(new Vector2I(4, -5));
	}
	
	///<summary>
	///Called on change to limit number of times remaking the grid
	///</summary>
	private void limitUpdates()
	{
		//call update all deferred so it can update bulk changes at once
		ignoreRemake = true;
		CallDeferred("UpdateAll");
	}
	
	///<summary>
	///Update all used Cells
	///</summary>
	private void updateAll()
	{
		//clear all other layers
		int i = 1;
		TileMapLayer tml = getTileMapLayer(i);
		while (tml != null)
		{
			tml.Clear();
			i++;
			tml = getTileMapLayer(i);
		}
		
		foreach (var coord in getTileMapLayer(0).GetUsedCells())
		{
			// GD.Print(coord);
			updateInternalTile(coord);
		}
		GD.Print("Updated");
		ignoreRemake = false;
	}
	
	///<summary>
	///Updates the 4 display layer cells based on Calculation Layer
	///</summary>
	///<param name="coord"> Coordinate on Calculation layer</param>
	public void updateInternalTile(Vector2I coord)
	{
		//if coord changes need to edit coord around to coord -1 -1
		foreach (var mod in internalToDisplay)
		{
			setDisplayTile (coord + mod);
		}
	}
	
	///<summary>
	///Updates a single display layer cell
	/// </summary>
	/// <param name="coord"> Coordinate on Display layer</param>
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
			// use the name custom data layer to determine which tile is on calculation layer
				neighbours[i] = tile.GetCustomData("Name").ToString();
			
		}
		setDisplayTileAtlasInfo(coord, neighbours);
	}
	
	///<summary>
	///Sets the Atlas Info of all display layers to match the neighbours supplied
	///</summary>
	/// <param name="coord"> Coordinate on Display layer</param>
	/// <param name="neighbours"> The neighbours using calculation layer names. Length 4</param>
	private void setDisplayTileAtlasInfo(Vector2I coord, string[] neighbours)
	{
		// error if not 4 in length
		if (neighbours.Length != 4) return;
		TileInfo[] tiles = colourGroupsToAtlasID(neighbours);
		// if (tiles.Length > 1) GD.Print(coord);
		for (int i = 0; i < tiles.Length; i++)
		{
			TileMapLayer tml = getTileMapLayer(i + 1);
			//deal with alternative tiles
			tml.SetCell(coord, tiles[i].atlasID, tiles[i].atlasCoord);
		}
	}
	
	///<summary>
	///Recursive function to get the Tile Info of all tiles needed to match the calculation neighbours in the display layer.
	///</summary>
	///<param name="neighbours">The neighbours using calculation layer names. Length 4</param>
	/// <returns>Array of (max 4) tiles needed to create the neighbour relation across the 4 display layers</returns>
	private TileInfo[] colourGroupsToAtlasID(string[] neighbours)
	{
		string[] names;
		int id;
		var td = new TileInfo();
		switch (neighbours.Distinct().Count())
		{
			//no distinct neighbours is an error - ignore
			case 0: throw new Exception("Invalid neighbors");
			//if 1 neighbour return tilename + transparent atlas
			case 1:
				names = [neighbours.FirstOrDefault(), "T"];
				td.atlasID = namePairToAtlasID(ref names);
				td.atlasCoord = getAtlasCoord(neighbours, names);
				return [td];
			//if 2 neighbours first try find bespoke tilename + tilename atlas
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
				//if not find solution with transparent
				goto default;
			//Default is to find tilename + transparent atlas for all neighbours
			default:
				names = neighbours.Distinct().ToArray();
				List<TileInfo> tis = new();
				for(int i = 0; i < names.Length; i++)
				{
					string[] n = new string[4];
					neighbours.CopyTo(n, 0);
					for(int j = 0; j < n.Length; j++)
					{
						if (n[j] != names[i])
						{
							n[j] = "T";
						}
					}
					tis.AddRange(colourGroupsToAtlasID(n));
				}
				return tis.ToArray();
				
		}
	}
	
	///<summary>
	///Function to search for an Atlas that contains a merge of the two input names
	/// </summary>
	/// <param name="names"> Names of the two calculation tiles to check. Array will be reordered to match the Atlas Vector Dictionary above. Length 2</param>
	/// <returns>Atlas ID. -1 Means there is no Atlas for the inputs</returns>
	private int namePairToAtlasID(ref string[] names)
	{
		// error if not 2 in length
		if  (names.Length != 2) return -1;
		if (TextNameToID.TryGetValue(names.First() + names.Last(), out var AtlasID))
		{
			return AtlasID;
		}
		// reorder the input array if Atlas is built the other way around
		if (TextNameToID.TryGetValue(names.Last() + names.First(), out AtlasID))
		{
			var temp = names.Last();
			names[1] = names[0];
			names[0] = temp;
			return AtlasID;
		}
		// -1 means it doesn't exist
		return -1;
	}
	
	///<summary>
	///Function to search for Atlas coordinates needed
	///</summary>
	/// <param name="neighbours"> The full calculation neighbour relation of the display tile</param>
	/// <param name="names"> The two calculation names which are expected to merge on this Atlas. Ordered such that [0] is Pink and [1] is transparent (on the template)</param>
	/// <returns>Atlas Coordinates</returns>
	private Vector2I getAtlasCoord(string[] neighbours, string[] names)
	{
		// error if array's wrong length
		if (names.Length != 2) return new Vector2I(0,0);
		if (neighbours.Length != 4) return new Vector2I(0,0);
		//rewrite neighbours how the dictionary looks for it
		string[] newNeighbours = new string[neighbours.Length];
		for (int i = 0; i < neighbours.Length; i++)
		{
			//Pink (template)
			if (names[0] == neighbours[i])
			{
				newNeighbours[i] = "w"; 
			//White (template)
			} else if (names[1] == neighbours[i])
			{
				newNeighbours[i] = "t";
			//Left separate in case a larger template is added.
			} else
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
