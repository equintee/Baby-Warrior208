using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class buyPoint : MonoBehaviour
{
    public int cost;
    public GameObject spawnerPrefab;

    private TextMeshProUGUI costText;

    private void Awake()
    {
        costText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        costText.text = cost.ToString();
    }

    private float deltaTime = 0f;
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        deltaTime += Time.deltaTime;
        if (deltaTime < 0.2f)
            return;
        playerController playerController = other.GetComponent<playerController>();
        if(playerController.getGold() > 1 && cost != 0)
        {
            updateCost(-1);
            playerController.updateGold(-1);
            deltaTime = 0f;
        }

        if(cost == 0)
        {
            GameObject spawner = Instantiate(spawnerPrefab, transform.position, Quaternion.Euler(new Vector3(0, -90, 0)), playerController.playerSpawnersParent);
            FindObjectOfType<unitMatcher>().addSpawnerToList(spawner);
            spawner.transform.position = transform.position - new Vector3(0, 5, 0);
            if (spawner.transform.position.z > 0)
                spawner.transform.GetChild(1).position = transform.position - new Vector3(3, 0, 0);
            else if (spawner.transform.position.z <= 0)
                spawner.transform.GetChild(1).position = transform.position + new Vector3(3, 0, 0);

            spawner.transform.DOMove(spawner.transform.position + new Vector3(0, 5, 0), 1f).SetEase(Ease.Linear);
            Destroy(gameObject);
        }

            
    }


    private void updateCost(int value)
    {
        cost -= value;
        costText.text = cost.ToString();

    }
}
