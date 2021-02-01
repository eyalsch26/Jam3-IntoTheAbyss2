using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimationManager : MonoBehaviour
{
    public Transform aimTarget;
    public playerMovement movementScript;
    public SkinnedMeshRenderer mesh;
    public GameObject gunObj;
    public GameObject canistersObj;
    public Material hitMaterial;
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
        StartCoroutine(HitFlicker());
    }

    public void setGrounded(bool isGrounded)
    {
        animator.SetBool(Animator.StringToHash("onGround"), isGrounded);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = movementScript.mousePos;
        //aimTarget.position = new Vector3(pos.x, pos.y, pos.z - 2);
        aimTarget.position = pos;
        if (!isWalking && !animator.GetBool("jump"))
        {
            movementScript.turnLeft(pos.x < transform.position.x);
        }
    }

    IEnumerator HitFlicker()
    {
        float t = 2;
        float quantum = 0.125f;
        while (t >= 0)
        {
            t -= 2 * quantum;
            mesh.enabled = false;
            gunObj.SetActive(false);
            canistersObj.SetActive(false);
            yield return new WaitForSeconds(quantum);
            mesh.enabled = true;
            gunObj.SetActive(true);
            canistersObj.SetActive(true);
            yield return new WaitForSeconds(quantum);
        }
    }
}
