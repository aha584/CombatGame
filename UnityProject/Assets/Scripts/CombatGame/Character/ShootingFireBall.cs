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

    private void Start()
    {
        currentInput = Keyboard.current;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentInput == null) return;
        if (CheckFaceSide())
        {
            bornPos = new Vector3(transform.position.x + fireBallOffset.x, transform.position.y - fireBallOffset.y, transform.position.z);
        }
        else
        {
            bornPos = new Vector3(transform.position.x - fireBallOffset.x, transform.position.y - fireBallOffset.y, transform.position.z);
        }
        timer += Time.deltaTime;
        if (!currentInput.jKey.wasPressedThisFrame) return;
        if(timer > attackDelay)
        {
            GameObject fireBallClone = Instantiate(fireballPrefab, bornPos, Quaternion.identity);
            fireBallClone.transform.localScale = transform.localScale;
            timer = 0f;
        }
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
}
