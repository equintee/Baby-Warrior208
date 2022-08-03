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
        /*DOTween.Kill(transform);
        Vector3 waypoint = target.GetComponent<BoxCollider>().ClosestPoint(transform.position);

        transform.DOLookAt(waypoint, 0f);
        lerper = rb.DOMove(waypoint, moveSpeed).SetSpeedBased().OnUpdate(() =>
        {
            lerper.ChangeEndValue(target.GetComponent<BoxCollider>().ClosestPoint(transform.position), true).SetSpeedBased();
        }).OnComplete(() => DOTween.Kill(lerper));*/

        this.target = target;
    }

    private void Update()
    {
        if (target != null)
            rb.position = Vector3.MoveTowards(rb.position, target.position, moveSpeed * Time.deltaTime);
    }
}
