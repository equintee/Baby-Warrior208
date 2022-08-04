using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class unitController : MonoBehaviour
{
    private float moveSpeed;
    private Rigidbody rb;
    private Animator animator;
    private Transform target;
    public int hp;
    public int damage;
    private void Start()
    {
        moveSpeed = FindObjectOfType<unitMatcher>().unitSpeed;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    public void setTarget(Transform target)
    {
        this.target = target;
    }

    private bool isMovingToTarget = true;
    private void Update()
    {

        if (isMovingToTarget)
        {
            transform.DOLookAt(target.position, 0f);
            rb.position = Vector3.MoveTowards(rb.position, target.position, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(rb.position, target.position) < 1f)
            {
                animateHit();
            }
                
        }

    }

    private async void animateHit()
    {
        isMovingToTarget = false;
        rb.constraints = RigidbodyConstraints.FreezePosition;
        animator.SetTrigger("attack");
        await Task.Delay(System.TimeSpan.FromSeconds(1.5f));

        target.GetComponent<unitController>().decrementHp(damage);
        target.GetComponent<unitController>().animateGetHit();
        isMovingToTarget = true;

    }

    public void decrementHp(int damage)
    {
        hp -= damage;
    }

    public async void animateGetHit()
    {
        isMovingToTarget = false;
        if (hp > 0)
        {
            animator.SetTrigger("getHit");
            
        }
        else
            animateDeath();
    }

    public void animateDeath()
    {
        animator.SetTrigger("death");
        this.enabled = false;
        Invoke("destroyUnit()", 3f);
    }

    public void destroyUnit()
    {
        DOTween.Kill(rb);
        Destroy(gameObject);
    }
}
