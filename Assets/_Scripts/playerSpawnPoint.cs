using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class playerSpawnPoint : MonoBehaviour
{
    private int spawnerLevel = 0;
    private float deltaTime = 0f;
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        deltaTime += Time.deltaTime;
        if (deltaTime > 0.5f)
        {
            playerController playerController = other.GetComponent<playerController>();
            playerController.spawnBaby(transform.parent.position + new Vector3(0, -1, 0), spawnerLevel);
            deltaTime = 0f;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        deltaTime = 0f;
    }

}
