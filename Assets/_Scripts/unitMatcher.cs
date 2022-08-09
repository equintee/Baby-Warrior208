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

        setTargetForAllUnits(playerUnitsList);
        setTargetForAllUnits(enemyUnitsList);

    }

    private void Update()
    {
        removeDeadSkeletonsFromList();

        foreach (GameObject skeleton in playerUnitsList.ToArray())
            setTarget(skeleton);

        foreach (GameObject skeleton in enemyUnitsList.ToArray())
            setTarget(skeleton);
    }

    private Collider[] hitList;
    public void setTarget(GameObject skeleton)
    {
        unitController skeletonController = skeleton.GetComponent<unitController>();
        List<GameObject> playerUnitsListTemp = new List<GameObject>(playerUnitsList);
        List<GameObject> enemyUnitsListTemp = new List<GameObject>(enemyUnitsList);
        if (skeleton.CompareTag("playerUnit"))
        {
            if (enemyUnitsList.Count > 0)
            {
                hitList = Physics.OverlapSphere(skeleton.transform.position, 20);
                foreach (Collider collider in hitList)
                {
                    if (enemyUnitsListTemp.Contains(collider.gameObject))
                    {
                        enemyUnitsList.Remove(collider.gameObject);
                        skeletonController.setTarget(collider.transform);
                        skeletonController.GetComponent<unitController>().isTargetBoss = false;
                        break;
                    }

                }
            }

            if (enemyUnitsList.Count == 0 && enemyUnits.childCount != 0)
            {
                skeletonController.setTarget(enemyUnits.GetChild(Random.Range(0, enemyUnits.childCount)));
                skeletonController.GetComponent<unitController>().isTargetBoss = false;
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
                hitList = Physics.OverlapSphere(skeleton.transform.position, 20);
                foreach (Collider collider in hitList)
                {
                    if (playerUnitsListTemp.Contains(collider.gameObject))
                    {
                        playerUnitsList.Remove(collider.gameObject);
                        skeletonController.setTarget(collider.transform);
                        skeletonController.GetComponent<unitController>().isTargetBoss = false;
                        break;
                    }
                }
            }

            if (playerUnitsList.Count == 0 && playerUnits.childCount != 0)
            {
                skeletonController.setTarget(playerUnits.GetChild(Random.Range(0, playerUnits.childCount)));
                skeletonController.GetComponent<unitController>().isTargetBoss = false;
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
}
