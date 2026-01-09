using UnityEngine;

public class ProjectileAnim : MonoBehaviour
{
    private Animator myAnimator;
    private ProjectileMove moveScript;

    private float multiplier = 1f;
    private float baseSpeed = 6.25f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myAnimator = GetComponent<Animator>();
        moveScript = GetComponent<ProjectileMove>();
    }

    // Update is called once per frame
    void Update()
    {
        multiplier = moveScript.movementSpeed / baseSpeed;
        myAnimator.SetFloat("flySpeed", multiplier);
    }
}
