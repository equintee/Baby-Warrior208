using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unitMatcher : MonoBehaviour
{
    public float unitSpeed;
    public int goldPerUnit;
    public GameObject player;
    public GameObject enemy;
    public Transform playerUnits;
    public Transform enemyUnits;
    public Transform corpse;


    [HideInInspector] public List<GameObject> playerUnitsList;
    [HideInInspector] public List<GameObject> enemyUnitsList;



    void Awake()
    {
        playerUnitsList = new List<GameObject>();
        enemyUnitsList = new List<GameObject>();

        foreach (Transform skeleton in playerUnits)
            playerUnitsList.Add(skeleton.gameObject);

        foreach (Transform skeleton in enemyUnits)
            enemyUnitsList.Add(skeleton.gameObject);

        foreach (Transform enemyUnit in enemyUnits)
            enemyUnit.tag = "enemyUnit";

    }

    private void Update()
    {
        removeDeadSkeletonsFromList();

        foreach (GameObject skeleton in playerUnitsList.ToArray())
            setTarget(skeleton);

        foreach (GameObject skeleton in enemyUnitsList.ToArray())
            setTarget(skeleton);
    }

    public void setTarget(GameObject skeleton)
    {
        unitController skeletonController = skeleton.GetComponent<unitController>();

        if (skeletonController.isTargetNullOrBoss() == false)
            return;


        if (skeleton.CompareTag("playerUnit"))
        {
            if (enemyUnitsList.Count > 0)
            {
                skeletonController.setTarget(findClosestEnemy(enemyUnitsList.ToArray(), skeleton));
            }

            if (enemyUnits.childCount == 0)
            {
                skeletonController.setTarget(enemy.transform);
                skeletonController.GetComponent<unitController>().isTargetBoss = true;
            }
        }

        if (skeleton.CompareTag("enemyUnit"))
        {
            if (playerUnitsList.Count > 0)
            {
                skeletonController.setTarget(findClosestEnemy(playerUnitsList.ToArray(), skeleton));
            }

            if (playerUnits.childCount == 0)
            {
                skeletonController.setTarget(player.transform);
                skeletonController.GetComponent<unitController>().isTargetBoss = true;
            }
        }

        skeletonController.enabled = true;

    }


    public void setTargetForAllUnits(List<GameObject> skeletonList)
    {
        foreach (GameObject skeleton in skeletonList)
            setTarget(skeleton);
    }

    public void addSkeletonToList(List<GameObject> list, GameObject skeleton)
    {
        if (!list.Contains(skeleton))
            list.Add(skeleton);
    }

    public void removeDeadSkeletonsFromList()
    {
        foreach (GameObject skeleton in playerUnitsList.ToArray())
            if (skeleton.GetComponent<unitController>().isAlive == false)
                playerUnitsList.Remove(skeleton);

        foreach (GameObject skeleton in enemyUnitsList.ToArray())
            if (skeleton.GetComponent<unitController>().isAlive == false)
                playerUnitsList.Remove(skeleton);
    }

    public void moveSkeletonToCorpse(GameObject skeleton)
    {
        skeleton.transform.parent = corpse;
    }

    private Transform findClosestEnemy(GameObject[] enemyArray, GameObject skeleton)
    {
        float minDistance = 999999f;
        Transform closestEnemy = null;
        foreach(GameObject opponent in enemyArray)
        {
            float distance = Vector3.Distance(skeleton.transform.position, opponent.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestEnemy = opponent.transform;
            }
        }
        return closestEnemy;
    }
}
