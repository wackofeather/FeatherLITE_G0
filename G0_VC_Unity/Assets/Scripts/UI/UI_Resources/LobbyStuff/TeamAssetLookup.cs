using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TeamAssetLookup", menuName = "Scriptable Objects/TeamAssetLookup")]
public class TeamAssetLookup : ScriptableObject
{
    public List<TeamAssets> TeamAssets;
    //public 
    //public Dictionary<string, string> MapLookupDict;


    public Dictionary<int, TeamAssets> GetMapLookUp()
    {
        Dictionary<int, TeamAssets> TeamAssets_Dict = new Dictionary<int, TeamAssets>();
        foreach (TeamAssets assets in TeamAssets)
        {
            TeamAssets_Dict.Add(assets.teamId, assets);
        }
        return TeamAssets_Dict;
    }
}
