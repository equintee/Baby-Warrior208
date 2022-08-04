using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unitMatcher : MonoBehaviour
{
    public float unitSpeed;
    public GameObject player;
    public GameObject enemy;

    [HideInInspector] public List<GameObject> playerUnitsList;
    [HideInInspector] public List<GameObject> enemyUnitsList;

    public Transform playerUnits;
    public Transform enemyUnits;


    void Awake()
    {
        playerUnitsList = new List<GameObject>();
        enemyUnitsList = new List<GameObject>();

        foreach (Transform skeleton in playerUnits)
            playerUnitsList.Add(skeleton.gameObject);

        foreach (Transform skeleton in enemyUnits)
            enemyUnitsList.Add(skeleton.gameObject);

        setTargetForAllUnits(playerUnitsList);
        setTargetForAllUnits(enemyUnitsList);

    }

    private void Update()
    {
        foreach (GameObject skeleton in playerUnitsList)
            setTarget(skeleton);

        foreach (GameObject skeleton in enemyUnitsList)
            setTarget(skeleton);
    }

    private Collider[] hitList;
    public void setTarget(GameObject skeleton)
    {
        unitController skeletonController = skeleton.GetComponent<unitController>();

        if (skeleton.CompareTag("playerUnit"))
        {
            if (enemyUnitsList.Count > 0)
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
            if (playerUnitsList.Count > 0)
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
            }

            if (playerUnitsList.Count == 0 && playerUnits.childCount != 0)
            {
                skeletonController.setTarget(playerUnits.GetChild(Random.Range(0, playerUnits.childCount)));
                return;
            }

            if (playerUnits.childCount == 0)
            {
                skeletonController.setTarget(player.transform);
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

}
