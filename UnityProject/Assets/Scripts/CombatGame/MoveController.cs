using UnityEngine;
using UnityEngine.InputSystem;

public class MoveController : MonoBehaviour
{
    public Animator myAnimator;


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
    }
}
