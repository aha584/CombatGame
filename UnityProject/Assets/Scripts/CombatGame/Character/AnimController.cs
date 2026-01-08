using UnityEngine;
using UnityEngine.InputSystem;

public class AnimController : MonoBehaviour
{
    private Animator myAnimator;
    private Keyboard currentInput;
    private MoveController moveController;

    private float previousPos = 0f;
    private float multiplier = 1f;
    private float baseWalkSpeed = 5f;
    private bool isCrouch = false;
    private bool isDizzy = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentInput = Keyboard.current;
        myAnimator = GetComponent<Animator>();
        moveController = GetComponent<MoveController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentInput == null) return;

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

    private void WalkAnim()
    {
        if (currentInput.aKey.isPressed || currentInput.dKey.isPressed)
        {
            myAnimator.SetBool("isMoving", true);
            multiplier = moveController.movementSpeed / baseWalkSpeed;
            myAnimator.SetFloat("walkSpeed", multiplier);
        }
        else
        {
            myAnimator.SetBool("isMoving", false);
        }
    }

    private void JumpAnim()
    {
        if (currentInput.spaceKey.wasPressedThisFrame)
        {
            myAnimator.SetTrigger("isJump");
            myAnimator.SetBool("isOnAir", true);
        }
        if (transform.position.y < previousPos)
        {
            myAnimator.SetBool("isOnAir", false);
        }
    }

    private void CrouchAnim()
    {
        if (currentInput.cKey.wasPressedThisFrame)
        {
            myAnimator.SetBool("isCrouch", !isCrouch);
            isCrouch = !isCrouch;
        }
    }

    private void AttackAnim()
    {
        if (currentInput.jKey.wasPressedThisFrame)
        {
            myAnimator.SetTrigger("isAttack");
        }
    }
    private void StrikeAnim()
    {
        //Can update to with key combo
        if (currentInput.iKey.wasPressedThisFrame)
        {
            myAnimator.SetTrigger("isStrike");
        }
    }
    private void FlyKichAnim()
    {
        //Same with strike
        if (currentInput.oKey.wasPressedThisFrame)
        {
            myAnimator.SetTrigger("isFlyKick");
        }
    }
    private void HurtAnim()//Call when take damage
    {
        myAnimator.SetTrigger("isHurt");
    }
    private void DieAnim()//Call when health = 0
    {
        myAnimator.SetBool("isDie", true);
    }
    private void WinAnim()//Call when health = 0
    {
        myAnimator.SetTrigger("isWin");
    }
    private void DizzyAnim() // Call when dizzy
    {
        myAnimator.SetBool("isDizzy", !isDizzy);
    }
}
