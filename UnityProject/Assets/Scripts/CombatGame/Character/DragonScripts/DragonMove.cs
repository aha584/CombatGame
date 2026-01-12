using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Windows;

public class DragonMove : MoveController
{
    public float baseFlyKickSpd;

    public bool isFlyKick = false;

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
        //Fly Kick Physic
        FlyKickPhysic();
    }
    private void FlyKickPhysic()
    {
        //Impulse Mode
        //3m ~ 5m / 1s
        if (myInput.flyKickControl is ButtonControl flyKickButton)
        {
            if (flyKickButton.wasPressedThisFrame && !isOnAir)
            {
                isFlyKick = true;
                transform.DOMoveX(transform.position.x + sideMulti * baseFlyKickSpd, 0.5f).SetDelay(0.5f).OnComplete(() => isFlyKick = false);
                Instantiate(dashWindPrefab, transform.position, Quaternion.identity);
                Destroy(dashWindPrefab, 2f);
            }
        }
    }
}
