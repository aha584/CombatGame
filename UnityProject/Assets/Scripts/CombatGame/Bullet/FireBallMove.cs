using UnityEngine;

public class FireBallMove : MonoBehaviour
{
    public float movementSpeed;

    private Rigidbody2D myRigidBody;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.localScale.x > 0)
        {
            myRigidBody.linearVelocityX = movementSpeed;
        }
        else
        {
            myRigidBody.linearVelocityX = -movementSpeed;
        }
        Destroy(gameObject, 10f);
    }
}
