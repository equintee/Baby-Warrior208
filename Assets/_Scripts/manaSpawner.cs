using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class manaSpawner : MonoBehaviour
{
    public int spawnInterval;
    public int maximumSpawnCount;

    public GameObject manaPrefab;

    public int counter = 0;
    private float upperBoundX;
    private float lowerBoundX;
    private float upperBoundZ;
    private float lowerBoundZ;

    void Start()
    {
        //Assign border coordinates
        playerController playerController = FindObjectOfType<playerController>();
        float[] borders = playerController.getBorders();

        lowerBoundX = borders[0];
        upperBoundX = borders[1];
        lowerBoundZ = borders[2];
        upperBoundX = borders[3];

        spawnMana();
    }

    private float deltaTime = 0f;
    private void Update()
    {
        deltaTime += Time.deltaTime;
        if (deltaTime >= spawnInterval)
            spawnMana();
    }

    private Collider[] hit;
    private void spawnMana()
    {
        deltaTime = 0f;
        if (counter == maximumSpawnCount)
            return;
        Vector3 spawnPoint = new Vector3(Random.Range(lowerBoundX, upperBoundX), 0.5f, Random.Range(lowerBoundZ, upperBoundZ));
        while (Physics.OverlapSphere(spawnPoint, 1f, layerMask: Physics.AllLayers ,queryTriggerInteraction: QueryTriggerInteraction.Collide).Length > 0)
            spawnPoint = new Vector3(Random.Range(lowerBoundX, upperBoundX), 0.5f, Random.Range(lowerBoundZ, upperBoundZ));

        counter++;
        Instantiate(manaPrefab, spawnPoint, Quaternion.identity, transform).transform.GetChild(0);
    }
}
