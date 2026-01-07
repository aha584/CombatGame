using UnityEngine;
using UnityEngine.InputSystem;

public class MoveController : MonoBehaviour
{
    public float movementSpeed;

    private Rigidbody2D myRigidBody;
    private Keyboard currentInput;
    private const float gravity = 9.81f;
    [SerializeField] private int tempInt = 0;

    [SerializeField] private bool isReachPeak = false;
    [SerializeField] private float previousPos;
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

        WalkPhysic();

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
            //Can Upgrade by using Edge Collider to get Collide event with ground
            previousPos = transform.position.y;
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
            myRigidBody.linearVelocityY = Mathf.Abs(myRigidBody.linearVelocityX) * Mathf.Tan(Mathf.PI / 4);
        }

        if (!isOnAir) return;
        if (myRigidBody.linearVelocityY > 0 && tempInt == 0 && isReachPeak)
        {
            myRigidBody.linearVelocityY -= gravity * Time.deltaTime;
        }
        else
        {
            tempInt = 1;
            isReachPeak = true;
        }

        if(isReachPeak)
        {
            myRigidBody.linearVelocityY += gravity * Time.deltaTime;
        }

        if (transform.position.y <= previousPos && !isReachPeak)
        {
            tempInt--;
            isReachPeak = false;
            isOnAir = false;
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
