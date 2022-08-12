using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerSpawnPoint : MonoBehaviour
{
    public int spawnerLevel;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if(true)
        {
            
        }
        else
            other.gameObject.GetComponent<playerController>().spawnSkeletons(transform.position, spawnerLevel);

    }
}
