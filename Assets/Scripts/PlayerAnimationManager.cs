using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimationManager : MonoBehaviour
{
    public Transform aimTarget;
    public playerMovement movementScript;
    public Camera mainCamera;
    private Animator animator;
    public bool isWalking;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void setWalking(bool Walking)
    {
        isWalking = Walking;
        animator.SetBool(Animator.StringToHash("isWalking"), isWalking);
    }

    public void setFaceLeft(bool isLeft)
    {
        animator.SetBool(Animator.StringToHash("facingLeft"), isLeft);
    }

    public bool getFaceLeft()
    {
        return animator.GetBool(Animator.StringToHash("facingLeft"));
    }

    public void jump()
    {
        animator.SetBool(Animator.StringToHash("jump"), true);
    }

    public void takeHit()
    {
        animator.SetBool(Animator.StringToHash("takeHit"), true);
    }

    public void setGrounded(bool isGrounded)
    {
        animator.SetBool(Animator.StringToHash("onGround"), isGrounded);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = movementScript.mousePos;
        aimTarget.position = new Vector3(pos.x, pos.y, -3);
        if (!isWalking && !animator.GetBool("jump"))
        {
            movementScript.turnLeft(pos.x < transform.position.x);
        }
    }
}
