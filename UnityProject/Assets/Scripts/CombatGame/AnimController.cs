using UnityEngine;
using UnityEngine.InputSystem;

public class AnimController : MonoBehaviour
{
    private Animator myAnimator;
    private Keyboard currentInput;

    private const float gapToGround = 0.01f;

    private float previousPos;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentInput = Keyboard.current;
        myAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentInput == null) return;

        //Walk Anim
        WalkAnim();
        //Jump Anim
        if (currentInput.spaceKey.wasPressedThisFrame)
        {
            previousPos = transform.position.y;

            myAnimator.SetTrigger("isJump");
            myAnimator.SetBool("isOnAir", true);
        }
        if (Mathf.Abs(transform.position.y - previousPos) <= gapToGround)
        {
            myAnimator.SetBool("isOnAir", false);
        }
    }

    bool CheckFaceSide()
    {
        if (transform.localScale.x > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void WalkAnim()
    {
        if (currentInput.aKey.isPressed)
        {
            myAnimator.SetBool("isMoving", true);
        }
        else if (currentInput.dKey.isPressed)
        {
            myAnimator.SetBool("isMoving", true);
        }
        else
        {
            myAnimator.SetBool("isMoving", false);
        }
    }
}
