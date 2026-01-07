using UnityEngine;
using UnityEngine.InputSystem;

public class MoveController : MonoBehaviour
{
    public float movementSpeed;

    private Rigidbody2D myRigidBody;
    private Keyboard currentInput;
    private const float gravity = 9.81f;

    private float angleToJump = 45f;
    [SerializeField] private bool isReachPeak = false;
    //Can Upgrade by using Edge Collider to get Collide event with ground
    [SerializeField] private float previousPos = 0;
    [SerializeField] private bool isOnAir = false;
    

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
        //Walk Physic
        WalkPhysic();
        //Jump Physic
        JumpPhysic();
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

    private void JumpPhysic()
    {
        if (currentInput.spaceKey.wasPressedThisFrame && !isOnAir)
        {
            isOnAir = true;
            if (CheckFaceSide())
            {
                myRigidBody.linearVelocityX = movementSpeed;
            }
            else
            {
                myRigidBody.linearVelocityX = -movementSpeed;
            }
            //Angle = 45
            myRigidBody.linearVelocityY = Mathf.Abs(myRigidBody.linearVelocityX) * Mathf.Tan(angleToJump * Mathf.PI / 180);
        }

        if (!isOnAir) return;
        if (myRigidBody.linearVelocityY > 0)
        {
            myRigidBody.linearVelocityY -= gravity * Time.deltaTime;
        }
        else if (isReachPeak)
        {
            myRigidBody.linearVelocityY -= gravity * Time.deltaTime;
        }
        else
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
    private void WalkPhysic()
    {
        if (currentInput.aKey.isPressed && !isOnAir)
        {
            if (CheckFaceSide())
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
            myRigidBody.linearVelocityX = -movementSpeed;
        }
        else if (currentInput.dKey.isPressed && !isOnAir)
        {
            if (!CheckFaceSide())
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
            myRigidBody.linearVelocityX = movementSpeed;
        }
        else
        {
            if (isOnAir) return;
            myRigidBody.linearVelocityX = 0f;
        }
    }
}
