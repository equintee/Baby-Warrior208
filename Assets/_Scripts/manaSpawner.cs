using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class manaSpawner : MonoBehaviour
{
    public GameObject manaPrefab;


    private float upperBoundX;
    private float lowerBoundX;
    private float upperBoundZ;
    private float lowerBoundZ;

    void Start()
    {
        //Assign border coordinates
        playerController playerController = FindObjectOfType<playerController>();
        float[] borders = playerController.getBorders();

        upperBoundX = borders[0];
        upperBoundZ = borders[1];

        lowerBoundX = -1 * upperBoundX;
        lowerBoundZ = -1 * upperBoundZ;

        spawnMana();
    }

    private void spawnMana()
    {
        Vector3 spawnPoint = new Vector3(Random.Range(lowerBoundX, upperBoundX), 0.5f, Random.Range(lowerBoundZ, upperBoundZ));
        Transform spawnedMana = Instantiate(manaPrefab, spawnPoint, Quaternion.identity, transform).transform.GetChild(0);
        spawnedMana.DOMoveY(spawnedMana.position.y + 1, 0.5f).SetEase(Ease.OutQuart).SetLoops(-1, LoopType.Yoyo);
        spawnedMana.DORotate(new Vector3(90, 360, 0), 2f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
        Invoke("spawnMana", 1f);
    }
}
