using UnityEngine;
using UnityEngine.InputSystem;

public class AnimController : MonoBehaviour
{


    private Animator myAnimator;
    private Keyboard currentInput;

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
        //move by A D
        if (currentInput.aKey.isPressed)
        {
            if (transform.localScale.x > 0f)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
            myAnimator.SetBool("isMoving", true);
        }
        else if (currentInput.dKey.isPressed)
        {
            if (transform.localScale.x < 0f)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
            myAnimator.SetBool("isMoving", true);
        }
        else
        {
            myAnimator.SetBool("isMoving", false);
        }
    }
}
