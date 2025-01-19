using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BumpColliderChecker : MonoBehaviour
{
    [SerializeField] PlayerStateMachine player;
    [HideInInspector] public List<Collider> player_hits;

    private void Update()
    {
        transform.rotation = Quaternion.Euler(0, player.yRotation, 0);
    }

    private void OnTriggerEnter(Collider col)
    {
        player_hits.Add(col);
        player.CurrentPlayerState.OnBumpPlayer(col);
    }

    private void OnTriggerExit(Collider col)
    {
        player_hits.Remove(col);
    }
}
