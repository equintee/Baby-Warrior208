using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class endingPointTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GetComponent<BoxCollider>().enabled = false;
        FindObjectOfType<levelController>().endGame(true);
    }
}
