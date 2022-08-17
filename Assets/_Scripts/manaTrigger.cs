using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class manaTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        
        DOTween.Kill(transform.GetChild(0));
        if (other.transform.CompareTag("Player"))
        {
            FindObjectOfType<manaSpawner>().counter--;
            other.GetComponent<playerController>().updateMana(10);
        }

        Destroy(gameObject);
    }
}

