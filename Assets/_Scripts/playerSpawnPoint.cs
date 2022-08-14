using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerSpawnPoint : MonoBehaviour
{
    public int spawnerLevel;
    public int goldCost;
    public BoxCollider hitbox;
    public BoxCollider spawnTrigger;

    private bool isEnabled;
    private void Awake()
    {
        isEnabled = gameObject.GetComponent<MeshRenderer>().enabled;
        Debug.Log(isEnabled);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        playerController playerController = other.GetComponent<playerController>();
        if (!isEnabled)
        {
            if (playerController.getGold() >= goldCost)
            {
                playerController.updateGold(-goldCost);
                animateUpgrade();
            }
                
        }
        else
            playerController.spawnSkeletons(transform.position, spawnerLevel);

    }

    public async void animateUpgrade()
    {
        GetComponent<MeshRenderer>().enabled = true;
        transform.position = transform.position + new Vector3(0, -3.5f, 0);
        await transform.DOMoveY(transform.position.y + 3.5f, 0.5f).SetEase(Ease.Linear).AsyncWaitForCompletion();
        spawnTrigger.enabled = false;
        spawnTrigger.enabled = true;
        //particle.Play()
        isEnabled = true;

        
    }
}
