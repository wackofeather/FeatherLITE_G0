using UnityEngine;

[CreateAssetMenu(fileName = "TeamAssets", menuName = "Scriptable Objects/TeamAssets")]
public class TeamAssets : ScriptableObject
{
    public GameObject teamSwitchButton;
    public GameObject teamPlayerThumbnail;
    public int teamId;
}
