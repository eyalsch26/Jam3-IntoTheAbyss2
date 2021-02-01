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

    // sound
    AudioSource audio;
    public AudioClip jumpAud;
    public AudioClip shotAud;
    public AudioClip RopeStartAud;
    public AudioClip RopeEndAud;
    public AudioClip SlowTimeAud;
    //public AudioClip SlowTimeEndAud;
    public AudioClip EmptyAud;
    public AudioClip landPlatformAud;
    public AudioClip acidSplashAud;
    public AudioClip hurtAud;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
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

    // audio methods
    public void playJump()
    {
        audio.clip = jumpAud;
        audio.pitch = (movementScript.isSlowingTime) ? 0.7f : 1;
        audio.Play();
    }

    public void playShot()
    {
        audio.clip = shotAud;
        audio.pitch = (movementScript.isSlowingTime) ? 0.7f : 1;
        audio.Play();
    }

    public void playRopeStart()
    {
        audio.clip = RopeStartAud;
        audio.pitch = (movementScript.isSlowingTime) ? 0.7f : 1;
        audio.Play();
    }

    public void playRopeCreated()
    {
        audio.clip = RopeEndAud;
        audio.pitch = (movementScript.isSlowingTime) ? 0.7f : 1;
        audio.Play();
    }

    public void playSlowTime()
    {
        audio.clip = SlowTimeAud;
        audio.pitch = 0.7f;
        audio.Play();
    }

    public void playSlowTimeEnd()
    {
        audio.pitch = -1;
        audio.clip = SlowTimeAud;
        audio.Play();
        audio.pitch = 1;
    }


    public void playEmptyAud()
    {
        audio.clip = EmptyAud;
        audio.pitch = (movementScript.isSlowingTime) ? 0.7f : 1;
        audio.Play();
    }

    public void playLandOnPlatform()
    {
        audio.clip = landPlatformAud;
        audio.pitch = (movementScript.isSlowingTime) ? 0.7f : 1;
        audio.Play();
    }

    public void playAcidSplash()
    {
        audio.clip = acidSplashAud;
        audio.pitch = (movementScript.isSlowingTime) ? 0.7f : 1;
        audio.Play();
    }

    public void playHurtSound()
    {
        audio.clip = hurtAud;
        audio.pitch = (movementScript.isSlowingTime) ? 0.7f : 1.5f;
        audio.Play();
    }   
}
