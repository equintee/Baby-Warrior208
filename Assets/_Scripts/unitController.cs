using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class unitController : MonoBehaviour
{
    public int hp;
    public int damage;
    [HideInInspector] public bool isTargetBoss = false;
    [HideInInspector] public bool isAlive = true;
    [HideInInspector] public bool isHitting = false;

    private float moveSpeed;
    private Rigidbody rb;
    private Animator animator;
    [SerializeField]private Transform target;
    
    private unitMatcher unitMatcher;
    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        moveSpeed = FindObjectOfType<unitMatcher>().unitSpeed;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        unitMatcher = transform.parent.parent.GetComponent<unitMatcher>();

        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = moveSpeed;
    }
    public void setTarget(Transform target)
    {
        this.target = target;
        moveToTarget();
    }

    private bool isMovingToTarget = true;
    private void Update()
    {
        Debug.Log(navMeshAgent.isStopped);
        if (!target)
            return;

        if(isTargetReached())
        {
            animateHit(damage);
        }
            

    }

    private async void animateHit(int damage)
    {
        this.enabled = false;
        isMovingToTarget = false;
        isHitting = true;
        navMeshAgent.isStopped = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        transform.DOLookAt(target.position, 0f);
        animator.SetTrigger("attack");
        await Task.Delay(System.TimeSpan.FromSeconds(1f));
        navMeshAgent.isStopped = false;
        target.GetComponent<unitController>().decrementHp(damage);
        bool isTargetDead = target.GetComponent<unitController>().animateGetHit();
        target = isTargetDead ? null : target;
        isMovingToTarget = true;
        isHitting = false;
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
        navMeshAgent.isStopped = true;
        unitMatcher.moveSkeletonToCorpse(gameObject);

        if (transform.CompareTag("playerUnit"))
            unitMatcher.playerUnitsList.Remove(gameObject);
        if (transform.CompareTag("enemyUnit"))
        {
            unitMatcher.enemyUnitsList.Remove(gameObject);
            FindObjectOfType<playerController>().updateGold(unitMatcher.goldPerUnit);
        }
            

        GetComponent<NavMeshAgent>().enabled = false;
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

    public bool isTargetNullOrBoss()
    {
        return !target || isTargetBoss;
    }

    public void moveToBridgeExit(Transform bridgeExit)
    {
        transform.DOLookAt(bridgeExit.transform.position, 0f);
        transform.DOMove(bridgeExit.transform.position, moveSpeed).SetSpeedBased().SetEase(Ease.Linear).OnComplete(() =>
        {
            unitMatcher.addSkeletonToList(unitMatcher.playerUnitsList, gameObject);
            this.enabled = true;
        });
    }

    public void moveToTarget()
    {
        navMeshAgent.SetDestination(target.position);
    }

    public bool isTargetReached()
    {
        return Vector3.Distance(transform.position, target.position) <= 2f;
    }
}
