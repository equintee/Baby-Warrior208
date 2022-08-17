using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class playerSpawnPoint : MonoBehaviour
{
    public int spawnerLevel;
    public int goldCost;
    public GameObject spawnCostIndicator; 
    public SphereCollider hitbox;
    public BoxCollider spawnTrigger;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        playerController playerController = other.GetComponent<playerController>();
        playerController.spawnSkeletons(transform.position + new Vector3(0,-1,0), spawnerLevel);

    }

}
