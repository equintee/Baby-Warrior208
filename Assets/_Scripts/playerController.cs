using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class playerController : MonoBehaviour
{
    public float movementSpeed;
    public float rotationSpeed;
    public Joystick joystick;

    private Vector3 horizontalMovement;
    private Vector3 verticalMovement;
    private Animator animator;

    private Touch touch;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    
    void Update()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
                animator.SetTrigger("run");
            if (touch.phase == TouchPhase.Ended)
                animator.SetTrigger("idle");
        }

        float yRotation = joystick.Horizontal * rotationSpeed * Time.deltaTime;

        transform.eulerAngles += new Vector3(0, yRotation, 0);

        verticalMovement = Vector3.forward * movementSpeed * Time.deltaTime * joystick.Vertical;
        horizontalMovement = Vector3.right * movementSpeed * Time.deltaTime * joystick.Horizontal;
        transform.Translate(verticalMovement + horizontalMovement);
    }
}
