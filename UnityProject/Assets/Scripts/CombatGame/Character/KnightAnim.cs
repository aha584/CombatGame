using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class KnightAnim : AnimController
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
        //Dash Anim
        DashAnim();
        //Block Anim
        BlockAnim();
        //Cast Anim
        CastAnim();
    }
    private void DashAnim()
    {
        //Same with strike
        if (myInput.dashControl is ButtonControl dashButton)
        {
            if (dashButton.wasPressedThisFrame)
            {
                myAnimator.SetTrigger("isDash");
            }
        }
    }
    private void BlockAnim()
    {
        if(myInput.blockControl is ButtonControl blockButton)
        {
            if (blockButton.wasPressedThisFrame)
            {
                myAnimator.SetTrigger("isBlock");
            }
        }
    }
    private void CastAnim()
    {
        if (myInput.castControl is ButtonControl castButton)
        {
            if (castButton.wasPressedThisFrame)
            {
                myAnimator.SetTrigger("isCast");
            }
        }
    }
}
