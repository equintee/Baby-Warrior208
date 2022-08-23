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
    public List<unitStats> unitStats;
    public Transform playerUnits;
    public Transform playerSpawnersParent;
    public Joystick joystick;
    public TextMeshProUGUI manaText;
    public TextMeshProUGUI goldText;
    private unitMatcher unitMatcher;

    [HideInInspector] public List<GameObject> playerSpawners;
    private Animator animator;
    private Transform playerModel;
    private Rigidbody rb;
    private int playerMana = 0;
    private int playerGold = 0;

    void Awake()
    {
        playerModel = transform.GetChild(0);
        animator = playerModel.GetComponent<Animator>();
        rb = transform.GetComponent<Rigidbody>();
        unitMatcher = FindObjectOfType<unitMatcher>();
        playerSpawners = new List<GameObject>();
        foreach (Transform spawnerTransform in playerSpawnersParent)
        {
            playerSpawners.Add(spawnerTransform.gameObject);
        }

        updateGoldText();
        updateManaBar();

    }

    
    void FixedUpdate()
    {
        playerMovement();
    }

    private bool isMoving = false;
    private void playerMovement()
    {
        if(!isMoving && Vector3.Magnitude(new Vector3(joystick.Horizontal, joystick.Vertical, 0)) > 0){
            isMoving = true;
            animator.SetTrigger("run");
        }
        else if(isMoving && Vector3.Magnitude(new Vector3(joystick.Horizontal, joystick.Vertical, 0)) == 0)
        {
            isMoving = false;
            animator.SetTrigger("idle");
        }

        //Fixable
        Vector3 rotation = new Vector3(0, (Mathf.Atan2(joystick.Vertical, joystick.Horizontal) * 180 / Mathf.PI * -1) - 90, 0);
        playerModel.eulerAngles = rotation;

        Vector3 verticalMovement = Vector3.back * movementSpeed * Time.fixedDeltaTime * joystick.Vertical;
        Vector3 horizontalMovement = Vector3.left * movementSpeed * Time.fixedDeltaTime * joystick.Horizontal;

        Vector3 waypoint = verticalMovement + horizontalMovement + transform.position;
        if(Physics.Raycast(waypoint, Vector3.down))
            rb.MovePosition(verticalMovement + horizontalMovement + transform.position);

    }

    
    private void setUpdate()
    {
        this.enabled = !this.enabled;
    }

    public async void spawnSkeletons(Vector3 spawnPosition, int unitLevel)
    {
        if(playerMana >= unitStats[unitLevel].manaCost)
        {
            updateMana(-unitStats[unitLevel].manaCost);
            GameObject spawnedSkeleton = Instantiate(unitStats[unitLevel].unitPrefab, spawnPosition, Quaternion.identity, playerUnits.transform);
            spawnedSkeleton.tag = "playerUnit";
            unitMatcher.addSkeletonToList(unitMatcher.playerUnitsList, spawnedSkeleton);
        }
    }

    private void updateManaBar()
    {
        manaText.text = playerMana.ToString();
    }

    public void updateMana(int value)
    {
        playerMana += value;
        updateManaBar();
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

    public int getGold()
    {
        return playerGold;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("enemyMana"))
            FindObjectOfType<enemyController>().playerStoleMana(other.gameObject);
    }

}
