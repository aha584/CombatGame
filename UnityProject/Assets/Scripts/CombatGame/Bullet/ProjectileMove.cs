using UnityEngine;

public class ProjectileMove : MonoBehaviour
{
    public float movementSpeed;

    private SpriteRenderer mySprite;
    private Rigidbody2D myRigidBody;
    private float timer = 0f;
    private float flyDelayTime = 0.275f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Need To Fix Its Bullet Collide With It
        //Consider with Tag
        myRigidBody = GetComponent<Rigidbody2D>();
        mySprite = GetComponent<SpriteRenderer>();
        mySprite.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer < flyDelayTime)
        {
            return;
        }
        mySprite.enabled = true;
        if(transform.localScale.x > 0)
        {
            myRigidBody.linearVelocityX = movementSpeed;
        }
        else
        {
            myRigidBody.linearVelocityX = -movementSpeed;
        }
        Destroy(gameObject, movementSpeed / 2);
    }
}
