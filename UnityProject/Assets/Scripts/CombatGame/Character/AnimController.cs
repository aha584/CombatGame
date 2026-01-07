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
        //Jump Anim
        JumpAnim();
        
    }

    private void WalkAnim()
    {
        if (currentInput.aKey.isPressed)
        {
            myAnimator.SetBool("isMoving", true);
            multiplier = moveController.movementSpeed / baseWalkSpeed;
            myAnimator.SetFloat("walkSpeed", multiplier);
        }
        else if (currentInput.dKey.isPressed)
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
        //Jump Attack Anim
        JumpAttackAnim();
        if (transform.position.y < previousPos)
        {
            myAnimator.SetBool("isOnAir", false);
        }
    }

    private void JumpAttackAnim()
    {
        if (currentInput.jKey.wasPressedThisFrame)
        {
            myAnimator.SetTrigger("isAttack");
        }
    }
}
