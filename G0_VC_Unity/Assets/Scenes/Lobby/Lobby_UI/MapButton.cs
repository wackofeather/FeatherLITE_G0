using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MapButton : NetworkBehaviour
{
    public NetworkVariable<int> voteCount = new NetworkVariable<int>();
    public MapData mapData;
    public SortingGroup sortingGroup;

    
    public void OnHoverEnter()
    {
        gameObject.transform.localScale *= 1.1f;
        sortingGroup.sortingOrder = -1;
    }
    public void OnHoverExit()
    {
        gameObject.transform.localScale /= 1.1f;
        sortingGroup.sortingOrder = 0;
    }

    public void OnVoteChanged(Toggle toggle)
    {
        if (toggle.isOn)
        {

            voteCount.Value += 1;
        }
        else
        {
            voteCount.Value -= 1;
        }
    }

}
