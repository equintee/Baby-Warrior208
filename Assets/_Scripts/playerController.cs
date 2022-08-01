using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class playerController : MonoBehaviour
{
    public float movementSpeed;
    public float rotationSpeed;
    public Joystick joystick;
    public Transform playField;
    public GameObject skeleton;
    public Transform playerUnits;
    public GameObject manaBar;

    private Vector3 horizontalMovement;
    private Vector3 verticalMovement;
    private Animator animator;
    private Touch touch;
    private Transform playerModel;
    private Rigidbody rb;
    private int playerMana = 50;


    void Start()
    {
        playerModel = transform.GetChild(0);
        animator = playerModel.GetComponent<Animator>();
        rb = transform.GetComponent<Rigidbody>();
    }

    
    void FixedUpdate()
    {
        playerMovement();
    }

    private void playerMovement()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
                animator.SetTrigger("run");
            if (touch.phase == TouchPhase.Ended)
                animator.SetTrigger("idle");
        }

        //Fixable
        Vector3 rotation = new Vector3(0, (Mathf.Atan2(joystick.Vertical, joystick.Horizontal) * 180 / Mathf.PI * -1) + 90, 0);
        playerModel.eulerAngles = rotation;

        verticalMovement = Vector3.forward * movementSpeed * Time.fixedDeltaTime * joystick.Vertical;
        horizontalMovement = Vector3.right * movementSpeed * Time.fixedDeltaTime * joystick.Horizontal;

        rb.MovePosition(verticalMovement + horizontalMovement + transform.position);
    }

    
    private void setUpdate()
    {
        this.enabled = !this.enabled;
    }


    private float skeletonSpawnTime;
    public async void spawnSkeletons()
    {
        if (Time.realtimeSinceStartup < skeletonSpawnTime + 1f)
            return;
        skeletonSpawnTime = Time.realtimeSinceStartup;
        setUpdate();
        animator.SetTrigger("spawnSkeleton");
        await Task.Delay(System.TimeSpan.FromSeconds(1f));
        Instantiate(skeleton, playField.GetComponent<BoxCollider>().ClosestPoint(transform.position), Quaternion.identity, playerUnits.transform);
        Invoke("setUpdate", 1f);
    }


    private Tween manaBarTween;
    private void updateManaBar()
    {
        if (manaBarTween != null)
            DOTween.Kill(manaBarTween);

        float targetManaBarValue = playerMana * 0.01f;
        manaBarTween = DOTween.To(() => manaBar.GetComponent<Image>().fillAmount, x => manaBar.GetComponent<Image>().fillAmount = x, targetManaBarValue, 0.5f).SetEase(Ease.Linear).OnComplete(() => manaBarTween = null);

    }

    public void updateMana(int value)
    {
        playerMana += value;
        updateManaBar();
    }
}
