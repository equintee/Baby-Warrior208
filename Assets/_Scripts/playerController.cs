using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct unitStats
{
    public int health;
    public int manaCost;
    public GameObject unitPrefab;
}


public class playerController : MonoBehaviour
{
    public float movementSpeed;
    public float rotationSpeed;
    public List<unitStats> unitStats;
    public Transform playerUnits;
    public Transform playerBridgeExit;
    public Joystick joystick;
    public GameObject manaBar;
    public TextMeshProUGUI goldText;
    public int manaCost;
    private unitMatcher unitMatcher;

    private Animator animator;
    private Transform playerModel;
    private Rigidbody rb;
    private int playerMana = 50;
    private int playerGold = 0;


    private float borderMaxX = 30f;
    private float borderMinX = 12f;
    private float borderMaxZ = 9f;
    private float borderMinZ = -9f;



    void Start()
    {
        playerModel = transform.GetChild(0);
        animator = playerModel.GetComponent<Animator>();
        rb = transform.GetComponent<Rigidbody>();
        unitMatcher = FindObjectOfType<unitMatcher>();

        updateGoldText();
        updateManaBar();

    }

    
    void FixedUpdate()
    {
        playerMovement();
    }

    private void playerMovement()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
                animator.SetTrigger("run");
            if (touch.phase == TouchPhase.Ended)
                animator.SetTrigger("idle");
        }

        //Fixable
        Vector3 rotation = new Vector3(0, (Mathf.Atan2(joystick.Vertical, joystick.Horizontal) * 180 / Mathf.PI * -1), 0);
        playerModel.eulerAngles = rotation;

        Vector3 verticalMovement = Vector3.left * movementSpeed * Time.fixedDeltaTime * joystick.Vertical;
        Vector3 horizontalMovement = Vector3.forward * movementSpeed * Time.fixedDeltaTime * joystick.Horizontal;

        rb.MovePosition(verticalMovement + horizontalMovement + transform.position);

       /* if (rb.position.x > borderMaxX)
            rb.position = new Vector3(borderMaxX, 0, rb.position.z);
        if (borderMinX > rb.position.x)
            rb.position = new Vector3(borderMinX, 0, rb.position.z);

        if (rb.position.z > borderMaxZ)
            rb.position = new Vector3(rb.position.x, 0, borderMaxZ);
        if (borderMinZ > rb.position.z)
            rb.position = new Vector3(rb.position.x, 0, borderMinZ);*/
    }

    
    private void setUpdate()
    {
        this.enabled = !this.enabled;
    }

    public async void spawnSkeletons(Vector3 spawnPosition, int unitLevel)
    {
        //Disable playerMovement
        setUpdate();
        while(playerMana >= unitStats[unitLevel].manaCost)
        {
            updateMana(-unitStats[unitLevel].manaCost);
            animator.SetTrigger("spawnSkeleton");
            await Task.Delay(System.TimeSpan.FromSeconds(1f));
            GameObject spawnedSkeleton = Instantiate(unitStats[unitLevel].unitPrefab, spawnPosition, Quaternion.identity, playerUnits.transform);
            spawnedSkeleton.tag = "playerUnit";
            unitMatcher.addSkeletonToList(unitMatcher.playerUnitsList, spawnedSkeleton);
            //spawnedSkeleton.GetComponent<unitController>().moveToBridgeExit(playerBridgeExit);
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
            animator.SetTrigger("run");


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
        float[] borders = new float[4]; //X,Z
                
        borders[0] = borderMinX;
        borders[1] = borderMaxX;
        borders[2] = borderMinZ;
        borders[3] = borderMaxX;

        return borders;
    }

    public void updateGold(int value)
    {
        playerGold += value;
        updateGoldText();
    }

    private void updateGoldText()
    {
        goldText.text = playerGold.ToString();
    }
}
