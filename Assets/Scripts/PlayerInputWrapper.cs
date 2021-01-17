using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerInputWrapper : MonoBehaviour
{
    playerMovement controller;
    public PlayerAnimationManager animate;
    char ROPE_MODE = 'r';
    char FIRE_MODE = 'f';
    char SHIELD_MODE= 's';

    float walkInput = 0;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<playerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (walkInput != 0)
        {
            char direction = (walkInput > 0) ? 'r' : 'l';
            controller.move(direction);
        }
    }



    public void iMove(InputAction.CallbackContext context)
    {
        walkInput = context.ReadValue<float>();
        if (walkInput == -1)
        {
            controller.turnLeft(true);
        }
        else if (walkInput == 1)
        {
            controller.turnLeft(false);
        }
        animate.setWalking(walkInput != 0);
    }

    public void iJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            controller.jump();
        }
    }

    public void iShoot(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            controller.shoot();
        }
        if (controller.shieldOn && context.phase == InputActionPhase.Canceled)
        {
            controller.stopShield();
        }
    }

    public void iSlowTime(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            controller.slowTime(true);
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            controller.slowTime(false);
        }
    }

    public void iChangeMode(InputAction.CallbackContext context)
    {
        Vector2 mode = context.ReadValue<Vector2>();
        if (mode.x == -1)
        {
            controller.changeMode(ROPE_MODE);
        }
        else if (mode.y == -1)
        {
            controller.changeMode(FIRE_MODE);
        }
        else if (mode.x == 1)
        {
            controller.changeMode(SHIELD_MODE);
        }
    }


    public void iReset(InputAction.CallbackContext context) // TODO: remove eventually.
    {
        if (context.phase == InputActionPhase.Started)
        {
            SceneManager.LoadScene("TomerScene");
        }
    }
}
