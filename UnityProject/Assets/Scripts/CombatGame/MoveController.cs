using UnityEngine;
using UnityEngine.InputSystem;

public class MoveController : MonoBehaviour
{
    public float movementSpeed;

    private Rigidbody2D myRigidBody;
    private Keyboard currentInput;

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
        if (currentInput.aKey.isPressed)
        {
            myRigidBody.linearVelocityX = -movementSpeed;
        }
        else if (currentInput.dKey.isPressed)
        {
            myRigidBody.linearVelocityX = movementSpeed;
        }
    }
}
