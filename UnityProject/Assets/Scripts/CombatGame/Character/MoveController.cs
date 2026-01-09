using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveController : MonoBehaviour
{
    public float movementSpeed;
    public float baseStrikeSpd;
    public float baseFlyKickSpd;
    public bool isCrouch = false;

    private Rigidbody2D myRigidBody;
    private Keyboard currentInput;
    private const float gravity = 9.81f;

    private float angleToJump = 45f;
    private float sideMulti = 1f;
    private bool isReachPeak = false;
    //Can Upgrade by using Edge Collider to get Collide event with ground
    private float previousPos = 0;
    private bool isOnAir = false;
    private bool isStrike = false;
    private bool isFlyKick = false;

    //Can update input to KeyControls
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        currentInput = Keyboard.current;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentInput == null) return;
        //Check Face Side
        CheckFaceSide();
        //Walk Physic
        WalkPhysic();
        //Jump Physic
        JumpPhysic();
        //Strike Physic
        StrikePhysic();
        //Fly Kick Physic
        FlyKickPhysic();
        //Crouch Physic
        CrouchPhysic();
    }

    private void CheckFaceSide()
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
    private void WalkPhysic()
    {
        if (isStrike || isFlyKick || isOnAir || isCrouch) return;
        if (currentInput.aKey.isPressed)
        {
            if (sideMulti > 0)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }
        else if (currentInput.dKey.isPressed)
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
    private void JumpPhysic()
    {
        if (isStrike || isFlyKick || isCrouch) return;
        if (currentInput.spaceKey.wasPressedThisFrame && !isOnAir)
        {
            isOnAir = true;
            myRigidBody.linearVelocityX = sideMulti * movementSpeed;
            //Angle = 45
            myRigidBody.linearVelocityY = Mathf.Abs(myRigidBody.linearVelocityX) * Mathf.Tan(angleToJump * Mathf.PI / 180);
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
    private void StrikePhysic()
    {
        //Impulse Mode
        //3m ~ 5m / 0.5 ~ 1s
        if (currentInput.iKey.wasPressedThisFrame && !isStrike)
        {
            isStrike = true;
            transform.DOMoveX(transform.position.x + sideMulti * baseStrikeSpd, 0.5f).SetDelay(0.3f).OnComplete(() => isStrike = false);
        }
    }
    private void FlyKickPhysic()
    {
        //Impulse Mode
        //3m ~ 5m / 1s
        if (currentInput.oKey.wasPressedThisFrame && !isFlyKick)
        {
            isFlyKick = true;
            transform.DOMoveX(transform.position.x + sideMulti * baseFlyKickSpd, 0.5f).SetDelay(0.5f).OnComplete(() => isFlyKick = false);
        }
    }
    private void CrouchPhysic()
    {
        if (currentInput.cKey.wasPressedThisFrame)
        {
            isCrouch = !isCrouch;
        }
    }
}
