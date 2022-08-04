using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class unitController : MonoBehaviour
{
    public int hp;
    public int damage;

    private float moveSpeed;
    private Rigidbody rb;
    private Animator animator;
    [SerializeField]private Transform target;
    private bool isAlive = true;
    private unitMatcher unitMatcher;
    private void Start()
    {
        moveSpeed = FindObjectOfType<unitMatcher>().unitSpeed;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        unitMatcher = transform.parent.parent.GetComponent<unitMatcher>();
    }

    public void setTarget(Transform target)
    {
        this.target = target;
        this.enabled = true;
    }

    private bool isMovingToTarget = true;
    private void Update()
    {

        if (target == null)
        {
            if (transform.CompareTag("playerUnit"))
                unitMatcher.addSkeletonToList(unitMatcher.playerUnitsList, gameObject);
            if (transform.CompareTag("enemyUnit"))
                unitMatcher.addSkeletonToList(unitMatcher.enemyUnitsList, gameObject);

            this.enabled = false;
            return;
        }
        if (isMovingToTarget)
        {
            transform.DOLookAt(target.position, 0f);
            rb.position = Vector3.MoveTowards(rb.position, target.position, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(rb.position, target.position) < 1f)
            {
                animateHit(damage);
            }
                
        }

    }

    private async void animateHit(int damage)
    {
        this.enabled = false;
        isMovingToTarget = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        transform.DOLookAt(target.position, 0f);
        animator.SetTrigger("attack");
        await Task.Delay(System.TimeSpan.FromSeconds(1.5f));

        target.GetComponent<unitController>().decrementHp(damage);
        bool isTargetDead = target.GetComponent<unitController>().animateGetHit();
        target = isTargetDead ? null : target;
        isMovingToTarget = true;

        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        this.enabled = true;

    }

    public void decrementHp(int damage)
    {
        hp -= damage;
    }

    public bool animateGetHit()
    {
        isMovingToTarget = false;
        if (hp > 0)
        {
            animator.SetTrigger("getHit");
        }
        else if (isAlive)
            return animateDeath();

        return false;

    }

    public bool animateDeath()
    {
        isAlive = false;

        if (transform.CompareTag("playerUnit"))
            unitMatcher.playerUnitsList.Remove(gameObject);
        if (transform.CompareTag("enemyUnit"))
            unitMatcher.enemyUnitsList.Remove(gameObject);

        GetComponent<BoxCollider>().enabled = false;
        animator.SetTrigger("death");
        this.enabled = false;
        Invoke("destroyUnit", 3f);
        return true;
    }

    public void destroyUnit()
    {
        DOTween.Kill(rb);
        Destroy(gameObject);
    }


}
