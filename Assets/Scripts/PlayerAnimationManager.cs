using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimationManager : MonoBehaviour
{
    public Transform aimTarget;
    public playerMovement movementScript;
    public SkinnedMeshRenderer mesh;
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
        StartCoroutine(FadeTo(0, 0.25f));
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

    IEnumerator FadeTo(float aValue, float aTime)
    {
        Material startMat = mesh.material;
        hitMaterial.color = startMat.color;
        mesh.material = hitMaterial;

        for (int i = 0; i < 4; i++)
        {
            for (float t = 0.0f; t < 1.0f; t += Time.unscaledDeltaTime / aTime)
            {
                Color newColor = new Color(1, 1, 1, Mathf.Lerp(1, aValue, t));
                mesh.material.color = newColor;
                yield return null;
            }
            for (float t = 0.0f; t < 1.0f; t += Time.unscaledDeltaTime / aTime)
            {
                Color newColor = new Color(1, 1, 1, Mathf.Lerp(aValue, 1, t));
                mesh.material.color = newColor;
                yield return null;
            }
        }
        mesh.material = startMat;
    }
}
