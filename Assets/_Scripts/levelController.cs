using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelController : MonoBehaviour
{
    [SerializeField] public unitMatcher unitMatcher;
    [SerializeField] public enemySpawner enemyController;
    [SerializeField] public playerController playerController;
    [SerializeField] public manaSpawner manaSpawner;


    public GameObject tapToStart;

    void Update()
    {
        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            unitMatcher.enabled = true;
            enemyController.enabled = true;
            playerController.enabled = true;
            manaSpawner.enabled = true;
            this.enabled = false;
            tapToStart.SetActive(false);
        }
    }
}
