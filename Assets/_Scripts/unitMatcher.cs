using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unitMatcher : MonoBehaviour
{
    public float unitSpeed;
    public GameObject player;
    public GameObject enemy;

    [HideInInspector]public List<GameObject> playerUnitsList;
    [HideInInspector]public List<GameObject> enemyUnitsList;

    public Transform playerUnits;
    public Transform enemyUnits;

    
    void Start()
    {
        playerUnitsList = new List<GameObject>();
        enemyUnitsList = new List<GameObject>();

    }


    private Collider[] hitList;
    public void setTarget(GameObject skeleton, bool isEnemy = false)
    {
        unitController skeletonController = skeleton.GetComponent<unitController>();
        
        if (skeleton.CompareTag("playerUnit"))
        {
            if(enemyUnitsList.Count > 0)
            {
                hitList = Physics.OverlapSphere(skeleton.transform.position, 20);
                foreach (Collider collider in hitList)
                {
                    if (enemyUnitsList.Contains(collider.gameObject))
                    {
                        enemyUnitsList.Remove(collider.gameObject);
                        skeletonController.setTarget(collider.transform);
                        return;
                    }
                }
            }

            if (enemyUnitsList.Count == 0 && enemyUnits.childCount != 0)
            {
                skeletonController.setTarget(enemyUnits.GetChild(Random.Range(0, enemyUnits.childCount)));
                return;
            }

            if (enemyUnits.childCount == 0)
            {
                skeletonController.setTarget(enemy.transform);
                return;
            }
        }

        if (skeleton.CompareTag("enemyUnit"))
        {
            if(playerUnitsList.Count > 0)
            {
                hitList = Physics.OverlapSphere(skeleton.transform.position, 20);
                foreach (Collider collider in hitList)
                {
                    if (playerUnitsList.Contains(collider.gameObject))
                    {
                        playerUnitsList.Remove(collider.gameObject);
                        skeletonController.setTarget(collider.transform);
                        return;
                    }
                }

                if(playerUnitsList.Count == 0 && playerUnits.childCount != 0)
                {
                    skeletonController.setTarget(playerUnits.GetChild(Random.Range(0, playerUnits.childCount)));
                    return;
                }

                if(playerUnits.childCount == 0)
                {
                    skeletonController.setTarget(player.transform);
                }

            }
        }

        skeletonController.enabled = true;

    }


    public void setTargetForAllUnits(List<GameObject> skeletonList)
    {
        foreach (GameObject skeleton in skeletonList)
            setTarget(skeleton);
    }
    
}
