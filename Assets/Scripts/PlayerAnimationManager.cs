using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void setWalking(bool isWalking)
    {
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
        
    }
}
