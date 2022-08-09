using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemySpawnPoint : MonoBehaviour
{
    public int spawnerLevel;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
            other.GetComponent<enemyController>().spawnSkeletons(transform.position, spawnerLevel);
    }
}
