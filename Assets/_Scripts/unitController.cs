using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unitController : MonoBehaviour
{
    private float moveSpeed;
    private Rigidbody rb;

    private Transform target;
    private void Start()
    {
        moveSpeed = FindObjectOfType<unitMatcher>().unitSpeed;
        Debug.Log(moveSpeed);
        rb = GetComponent<Rigidbody>();
    }

    Tweener lerper;
    public void setTarget(Transform target)
    {
        this.target = target;
    }

    private bool isMovingToTarget = true;
    private void Update()
    {

        if (target != null && isMovingToTarget)
        {
            transform.DOLookAt(target.position, 0f);
            rb.position = Vector3.MoveTowards(rb.position, target.position, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(rb.position, target.position) < 1f)
                isMovingToTarget = false;
        }

    }
}
