using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerInputWrapper : MonoBehaviour
{
    playerMovement controller;
    public PlayerAnimationManager animate;
    public PlayerStats stats;
    static char ROPE_MODE = 'r';
    static char FIRE_MODE = 'f';
    static char SHIELD_MODE= 's';
    static List<char> modeList = new List<char> { ROPE_MODE, FIRE_MODE, SHIELD_MODE };
    //int modeIdx;

    float walkInput = 0;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<playerMovement>();
        //modeIdx = 0;
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
            animate.isWalking = true;
        }
        else if (walkInput == 1)
        {
            controller.turnLeft(false);
            animate.isWalking = true;
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
        if (context.phase == InputActionPhase.Canceled)
        {
            controller.stopShoot();
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
        //    Vector2 mode = context.ReadValue<Vector2>();
        //    if (context.phase == InputActionPhase.Started)
        //    {
        //        modeIdx = (mode.y > 0) ? modeIdx + 1 : modeIdx - 1;
        //        modeIdx = (modeIdx + 3) % 3;
        //        controller.changeMode(modeList[modeIdx]);
        //        stats.startModeWheel();
        //    }
    }

    public void iModeWheel(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            stats.startModeWheel();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            stats.closeModeWheel();
        }
    }


    public void iReset(InputAction.CallbackContext context) // TODO: remove eventually.
    {
        if (context.phase == InputActionPhase.Started)
        {
            SceneManager.LoadScene("TomerScene");
        }
    }

    public void iPause(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (!controller.manager.isPaused)
            {
                controller.manager.pauseGame();
            }
            else if(!controller.manager.gameIsOver)
            {
                controller.manager.resumeGame();
            }
        }
    }
}
