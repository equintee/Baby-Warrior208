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
    public int manaCost;
    private unitMatcher unitMatcher;

    private Vector3 horizontalMovement;
    private Vector3 verticalMovement;
    private Animator animator;
    private Touch touch;
    private Transform playerModel;
    private Rigidbody rb;
    private int playerMana = 50;


    private float borderX = 9.5f;
    private float borderZ = 9.5f;



    void Start()
    {
        playerModel = transform.GetChild(0);
        animator = playerModel.GetComponent<Animator>();
        rb = transform.GetComponent<Rigidbody>();
        unitMatcher = FindObjectOfType<unitMatcher>();
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

        if (rb.position.x > borderX)
            rb.position = new Vector3(borderX, 0, rb.position.z);
        if (-1 * borderX > rb.position.x)
            rb.position = new Vector3(-1 * borderX, 0, rb.position.z);

        if (rb.position.z > borderZ)
            rb.position = new Vector3(rb.position.x, 0, borderZ);
        if (-1 * borderZ > rb.position.z)
            rb.position = new Vector3(rb.position.x, 0, borderZ * -1);
    }

    
    private void setUpdate()
    {
        this.enabled = !this.enabled;
    }

    public async void spawnSkeletons(Vector3 spawnPosition)
    {
        //Disable playerMovement
        setUpdate();

        while(playerMana >= manaCost)
        {
            updateMana(-manaCost);
            animator.SetTrigger("spawnSkeleton");
            await Task.Delay(System.TimeSpan.FromSeconds(1f));
            GameObject spawnedSkeleton = Instantiate(skeleton, spawnPosition, Quaternion.identity, playerUnits.transform);
            unitMatcher.playerSkeletons.Add(spawnedSkeleton);

        }
        
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

    public float[] getBorders()
    {
        float[] borders = new float[2]; //X,Z
        borders[0] = borderX;
        borders[1] = borderZ;

        return borders;
    }

}
