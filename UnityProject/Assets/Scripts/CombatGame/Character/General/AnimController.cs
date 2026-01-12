using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class AnimController : MonoBehaviour
{
    public Animator myAnimator;
    public InputController myInput;

    private MoveController moveController;

    private float previousPos = 0f;
    private float multiplier = 1f;
    private float baseWalkSpeed = 5f;
    private bool isCrouch = false;
    private bool isDizzy = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myAnimator = GetComponent<Animator>();
        moveController = GetComponent<MoveController>();
        myInput = GetComponent<InputController>();
    }

    protected void WalkAnim()
    {
        if (myInput.walkLeftControl.IsPressed() || myInput.walkRightControl.IsPressed())
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

    protected void JumpAnim()
    {
        if (myInput.jumpControl is ButtonControl jumpButton)
        {
            if (jumpButton.wasPressedThisFrame)
            {

                myAnimator.SetTrigger("isJump");
                myAnimator.SetBool("isOnAir", true);
            }
        }
        if (!moveController.isOnAir)
        {
            myAnimator.SetBool("isOnAir", false);
        }
    }

    protected void CrouchAnim()
    {
        if (myInput.crouchControl is ButtonControl crouchButton)
        {
            if (crouchButton.wasPressedThisFrame)
            {
                myAnimator.SetBool("isCrouch", !isCrouch);
                isCrouch = !isCrouch;
            }
        }
    }

    protected void AttackAnim()
    {
        if (myInput.attackControl is ButtonControl attackButton)
        {
            if (attackButton.wasPressedThisFrame)
            {
                myAnimator.SetTrigger("isAttack");
            }
        }
            
    }
    protected void StrikeAnim()
    {
        //Can update to with key combo
        if (myInput.strikeControl is ButtonControl strikeButton)
        {
            if (strikeButton.wasPressedThisFrame)
            {
                myAnimator.SetTrigger("isStrike");
            }
        }
    }
    public void HurtAnim()//Call when take damage
    {
        myAnimator.SetTrigger("isHurt");
    }
    public void DieAnim()//Call when health = 0
    {
        myAnimator.SetBool("isDie", true);
    }
    public void WinAnim()//Call when health = 0
    {
        myAnimator.SetTrigger("isWin");
    }
    public void DizzyAnim() // Call when dizzy
    {
        myAnimator.SetBool("isDizzy", !isDizzy);
    }
}
