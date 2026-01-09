using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class KnightMove : MoveController
{
    public float baseDashSpd;

    private bool isDash = false;

    // Update is called once per frame
    void Update()
    {
        //Check Face Side
        CheckFaceSide();
        //Walk Physic
        WalkPhysic();
        //Jump Physic
        JumpPhysic();
        //Strike Physic
        StrikePhysic();
        //Crouch Physic
        CrouchPhysic();
        //Dash Physic
        DashPhysic();
        //Cast physic
        CastPhysic();
    }
    private void DashPhysic()
    {
        //Impulse Mode
        //3m ~ 5m / 1s
        if (myInput.dashControl is ButtonControl dashButton)
        {
            if (dashButton.wasPressedThisFrame && !isOnAir)
            {
                isDash = true;
                transform.DOMoveX(transform.position.x + sideMulti * baseDashSpd, 0.5f).SetDelay(0.5f).OnComplete(() => isDash = false);
            }
        }
    }
    private void CastPhysic()
    {
        //Knockback little bit
        if (myInput.castControl is ButtonControl castButton)
        {
            if (castButton.wasPressedThisFrame && !isOnAir)
            {
                transform.DOMoveX(transform.position.x - sideMulti * baseKnockback, 0.1f).SetDelay(0.5f);
            }
        }
    }
}
