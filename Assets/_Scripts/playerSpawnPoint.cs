using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class playerSpawnPoint : MonoBehaviour
{
    public bool isEnabled;
    public int spawnerLevel;
    public int goldCost;
    public GameObject spawnCostIndicator; 
    public SphereCollider hitbox;
    public BoxCollider spawnTrigger;
    private void Awake()
    {
        hitbox.enabled = isEnabled;

        foreach (Transform child in transform)
            child.GetComponent<MeshRenderer>().enabled = isEnabled;

        if (!isEnabled)
        {
            spawnCostIndicator = Instantiate(spawnCostIndicator, new Vector3(transform.position.x, 6.5f, transform.position.z), Quaternion.Euler(90, -90, 0), transform.parent);
            spawnCostIndicator.GetComponentInChildren<TextMeshProUGUI>().text = goldCost.ToString();
        }
            

        
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
        isEnabled = true;
        Destroy(spawnCostIndicator);
        hitbox.enabled = isEnabled;

        foreach (Transform child in transform)
            child.GetComponent<MeshRenderer>().enabled = isEnabled;

        transform.position = transform.position + new Vector3(0, -4.5f, 0);
        await transform.DOMoveY(transform.position.y + 4.5f, 0.5f).SetEase(Ease.Linear).AsyncWaitForCompletion();

        spawnTrigger.enabled = false;
        spawnTrigger.enabled = true;
        //particle.Play()
        
    }
}
