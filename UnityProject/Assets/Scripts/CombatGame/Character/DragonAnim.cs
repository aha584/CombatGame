using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Windows;

public class DragonAnim : AnimController
{

    // Update is called once per frame
    void Update()
    {   
        //Walk Anim
        WalkAnim();
        //Attack Anim
        AttackAnim();
        //Jump Anim
        JumpAnim();
        //Crounch Anim
        CrouchAnim();
        //Strike Anim
        StrikeAnim();
        //Fly Kick Anim
        FlyKichAnim();
    }
    private void FlyKichAnim()
    {
        //Same with strike
        if (myInput.flyKickControl is ButtonControl flyKickButton)
        {
            if (flyKickButton.wasPressedThisFrame)
            {
                myAnimator.SetTrigger("isFlyKick");
            }
        }
    }

}
