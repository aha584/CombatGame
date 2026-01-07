using UnityEngine;

public class FireBallAnim : MonoBehaviour
{
    private Animator myAnimator;
    private FireBallMove moveScript;

    private float multiplier = 1f;
    private float baseSpeed = 6.25f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myAnimator = GetComponent<Animator>();
        moveScript = GetComponent<FireBallMove>();
    }

    // Update is called once per frame
    void Update()
    {
        multiplier = moveScript.movementSpeed / baseSpeed;
        myAnimator.SetFloat("flySpeed", multiplier);
    }
}
