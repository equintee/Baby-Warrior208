using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class playerController : MonoBehaviour
{
    public float movementSpeed;
    public float rotationSpeed;
    public Joystick joystick;
    public Transform playField;
    public GameObject skeleton;
    public Transform playerUnits;

    private Vector3 horizontalMovement;
    private Vector3 verticalMovement;
    private Animator animator;
    private Touch touch;
    private Transform playerModel;
    private Rigidbody rb;

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

    public async void spawnSkeletons()
    {
        setUpdate();
        animator.SetTrigger("spawnSkeleton");
        await Task.Delay(System.TimeSpan.FromSeconds(1f));
        Instantiate(skeleton, playField.GetComponent<BoxCollider>().ClosestPoint(transform.position), Quaternion.identity, playerUnits.transform);
        Invoke("setUpdate", 1f);
    }
}
