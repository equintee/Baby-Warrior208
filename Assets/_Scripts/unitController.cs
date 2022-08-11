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
    [HideInInspector] public bool isTargetSpawner = false;
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

    private void Update()
    {

        if (!target)
            return;

        if(isTargetReached())
        {
            animateHit(damage);
        }
            

    }

    private async void animateHit(int damage)
    {
        isHitting = true;
     
        navMeshAgent.ResetPath();
        navMeshAgent.velocity = Vector3.zero;

        rb.constraints = RigidbodyConstraints.FreezeAll;
        transform.DOLookAt(target.position, 0f);
        animator.SetTrigger("attack");
        await Task.Delay(System.TimeSpan.FromSeconds(1.250f));
        
        if(target && (target.CompareTag("enemyUnit") || target.CompareTag("playerUnit")))
        {
            target.GetComponent<unitController>().decrementHp(damage);
            bool isTargetDead = target.GetComponent<unitController>().animateGetHit();
            target = isTargetDead ? null : target;
        }
        else if(target.CompareTag("enemySpawner"))
        {
            target.gameObject.SetActive(false);
        }
        
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        isHitting = false;

    }

    public void decrementHp(int damage)
    {
        hp -= damage;
    }

    public bool animateGetHit()
    {
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
        return !target || isTargetSpawner;
    }

    public void moveToTarget()
    {
        navMeshAgent.SetDestination(target.position);
    }

    public bool isTargetReached()
    {
        return Vector3.Distance(transform.position, target.position) <= 2f;
    }

    public void resetPath()
    {
        navMeshAgent.ResetPath();
    }
}
