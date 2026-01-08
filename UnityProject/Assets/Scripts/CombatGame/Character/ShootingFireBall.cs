using UnityEngine;
using UnityEngine.InputSystem;

public class ShootingFireBall : MonoBehaviour
{
    public GameObject fireballPrefab;
    public Vector3 fireBallOffset;

    private Keyboard currentInput;

    private Vector3 bornPos;
    private float timer = 0f;
    private float attackDelay = 0.2f;
    private MoveController moveControl;
    private float crouchOffset = 0.4f;
    private int crouchMulti = 0;
    private float sideMulti = 1f;

    private void Start()
    {
        moveControl = GetComponent<MoveController>();
        currentInput = Keyboard.current;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentInput == null) return;
        if (moveControl.isCrouch)
        {
            crouchMulti = 1;
            attackDelay = 0.75f;
        }
        else
        {
            crouchMulti = 0;
        }
        CheckFaceSide();
        bornPos = new Vector3(transform.position.x + sideMulti * fireBallOffset.x,
                              transform.position.y - fireBallOffset.y - crouchMulti * crouchOffset,
                              transform.position.z);
        timer += Time.deltaTime;
        if (!currentInput.jKey.wasPressedThisFrame) return;
        if (timer > attackDelay)
        {
            GameObject fireBallClone = Instantiate(fireballPrefab, bornPos, Quaternion.identity);
            fireBallClone.transform.localScale = transform.localScale;
            timer = 0f;
        }
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
}
