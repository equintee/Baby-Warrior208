using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unitMatcher : MonoBehaviour
{
    public float unitSpeed;
    public GameObject enemy;

    public List<GameObject> playerSkeletons;
    public List<GameObject> enemySkeletons;
    void Start()
    {
        playerSkeletons = new List<GameObject>();
        enemySkeletons = new List<GameObject>();

        foreach (Transform playerSkeleton in transform.GetChild(0))
            playerSkeletons.Add(playerSkeleton.gameObject);

        foreach (Transform enemySkeleton in transform.GetChild(1))
            enemySkeletons.Add(enemySkeleton.gameObject);

        InvokeRepeating("setTargets", 2f, 10);

    }


    private RaycastHit hit;
    public void setTargets()
    {
        if(enemySkeletons.Count == 0)
        {
            foreach(GameObject playerSkeleton in playerSkeletons)
            {
                playerSkeleton.GetComponent<unitController>().setTarget(enemy.transform);
            }
            return;
        }
        else
        {
            foreach(GameObject playerSkeleton in playerSkeletons)
            {
                if(Physics.SphereCast(playerSkeleton.transform.position, 10, transform.forward, out hit))
                {
                    playerSkeleton.GetComponent<unitController>().setTarget(hit.collider.transform);
                    hit.collider.transform.GetComponent<unitController>().setTarget(playerSkeleton.transform);
                }
            }
        }

    }
    
}
