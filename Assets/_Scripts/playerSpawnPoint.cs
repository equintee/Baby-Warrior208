using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerSpawnPoint : MonoBehaviour
{
    public GameObject skeletonSpawnButton;
    private void OnTriggerEnter(Collider other)
    {
        skeletonSpawnButton.SetActive(true);
    }
    private void OnTriggerExit(Collider other)
    {
        skeletonSpawnButton.SetActive(false);
    }
}
