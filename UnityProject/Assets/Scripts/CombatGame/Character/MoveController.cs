using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class MoveController : MonoBehaviour
{
    public float movementSpeed;
    public float baseStrikeSpd;
    public bool isCrouch = false;
    public float sideMulti = 1f;
    public bool isOnAir = false;
    public InputController myInput;
    public float baseKnockback = 1f;
    public bool isHurt = false;

    private Rigidbody2D myRigidBody;
    private const float gravity = 9.81f;

    private float angleToJump = 45f;
    private bool isReachPeak = false;
    //Can Upgrade by using Edge Collider to get Collide event with ground
    private float previousPos = 0;
    private bool isStrike = false;

    //Can update input to KeyControls
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myInput = GetComponent<InputController>();
    }

    protected void CheckFaceSide()
    {
        if (transform.localScale.x > 0)
        {
            sideMulti = 1f;
        }
        else
        {
            sideMulti = -1f;
        }
    }
    protected void WalkPhysic()
    {
        if (isStrike || isOnAir || isCrouch) return;
        if (myInput.walkLeftControl.IsPressed())
        {
            if (sideMulti > 0)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }
        else if (myInput.walkRightControl.IsPressed())
        {
            if(sideMulti < 0)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }
        else
        {
            myRigidBody.linearVelocityX = 0f;
            return;
        }
        myRigidBody.linearVelocityX = sideMulti * movementSpeed;
    }
    protected void JumpPhysic()
    {
        if (isStrike || isCrouch) return;
        if (myInput.jumpControl is ButtonControl jumpButton)
        {
            if (jumpButton.wasPressedThisFrame && !isOnAir)
            {
                isOnAir = true;
                myRigidBody.linearVelocityX = sideMulti * movementSpeed;
                //Angle = 45
                myRigidBody.linearVelocityY = Mathf.Abs(myRigidBody.linearVelocityX) * Mathf.Tan(angleToJump * Mathf.PI / 180);
            }
        }

        if (!isOnAir) return;

        myRigidBody.linearVelocityY -= gravity * Time.deltaTime;
        if (myRigidBody.linearVelocityY <= 0f)
        {
            isReachPeak = true;
        }

        if (transform.position.y <= previousPos && isReachPeak)
        {
            isReachPeak = false;
            isOnAir = false;
            myRigidBody.linearVelocityY = 0f;
            transform.position = new Vector3(transform.position.x, previousPos,transform.position.z);
        }
    }
    protected void StrikePhysic()
    {
        //Impulse Mode
        //3m ~ 5m / 0.5 ~ 1s
        if (myInput.strikeControl is ButtonControl strikeButton)
        {
            if (strikeButton.wasPressedThisFrame && !isOnAir)
            {
                isStrike = true;
                transform.DOMoveX(transform.position.x + sideMulti * baseStrikeSpd, 0.5f).SetDelay(0.3f).OnComplete(() => isStrike = false);
            }
        }
    }
    protected void CrouchPhysic()
    {
        if (myInput.crouchControl is ButtonControl crouchButton)
        {
            if (crouchButton.wasPressedThisFrame && !isOnAir)
            {
                isCrouch = !isCrouch;
            }
        }
    }
    protected void HurtPhysic()
    {
        if (isHurt)
        {
            transform.DOMoveX(transform.position.x - sideMulti * baseKnockback, 0.1f).SetDelay(0.5f);
        }
    }
}
