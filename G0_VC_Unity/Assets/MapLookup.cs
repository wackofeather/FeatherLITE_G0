using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "MapLookup", menuName = "Scriptable Objects/MapLookup")]
public class MapLookup : ScriptableObject//, ISerializationCallbackReceiver
{
    public List<MapData> MapDatas;
    //public Dictionary<string, string> MapLookupDict;


    public Dictionary<string, string> GetMapLookUp()
    {
        Dictionary<string, string> MapLookupDict = new Dictionary<string, string>();
        foreach (MapData data in MapDatas)
        {
            MapLookupDict.Add(data.MapName, data.MapSceneName);
        }
        return MapLookupDict;
    }
/*    public void OnBeforeSerialize()
    {

    }

    public void OnAfterDeserialize()
    {
        foreach (MapData data in MapDatas)
        {

            if (MapLookupDict.ContainsKey(data.MapName)) MapLookupDict[data.MapName] = data.MapSceneName;
            else MapLookupDict.Add(data.MapName, data.MapSceneName);
        }
    }*/
}