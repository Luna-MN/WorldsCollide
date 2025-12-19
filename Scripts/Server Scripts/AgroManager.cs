using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
/// <summary>
/// The Agro Managment for an enemy or a minion, these are stored in a list in the player for each enemy the player has agro over
/// </summary>
public partial class AgroManager : Node2D
{
    public List<CharacterAgro> charactersAgros;
    private int OrderType = 0; // 0 = Agro, 1 = DPS, 2 = HPS
    /// <summary>
    /// Returns the highest agro in Agro
    /// </summary>
    /// <returns>Main Tank</returns>
    private Character MainTankAgro()
    {
        if (OrderType != 0)
        { 
            charactersAgros = charactersAgros.OrderBy(x => x.Agro).ToList();
            OrderType = 0;
        }
        return charactersAgros.First().character;
    }
    /// <summary>
    /// Return second in agro
    /// </summary>
    /// <returns>Off Tank</returns>
    private Character OffTankAgro()
    {
        if (OrderType != 0)
        { 
            charactersAgros = charactersAgros.OrderBy(x => x.Agro).ToList();
            OrderType = 0;
        }
        return charactersAgros[1].character;
    }
}
