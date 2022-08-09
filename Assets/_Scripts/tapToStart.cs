using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tapToStart : MonoBehaviour
{
    [SerializeField] public unitMatcher unitMatcher;
    [SerializeField] public enemyController enemyController;
    [SerializeField] public playerController playerController;
    [SerializeField] public manaSpawner manaSpawner;
    

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0)
        {
            unitMatcher.enabled = true;
            enemyController.enabled = true;
            playerController.enabled = true;
            manaSpawner.enabled = true;
            this.enabled = false;
            gameObject.SetActive(false);
        }
    }
}
