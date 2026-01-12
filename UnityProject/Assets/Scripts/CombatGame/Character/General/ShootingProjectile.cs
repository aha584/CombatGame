using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class ShootingProjectile : MonoBehaviour
{
    public GameObject projectilePrefab;

    private Vector3 projectileOffset;
    private InputController myInput;
    private MoveController moveControl;

    private Vector3 bornPos;
    private float timer = 0f;
    private float attackDelay;
    private float crouchOffset = 0.4f;
    private int crouchMulti = 0;
    private float sideMulti = 1f;
    private ButtonControl shootingProjectile;
    private bool isDragon = false;

    private void Start()
    {
        moveControl = GetComponent<MoveController>();
        myInput = GetComponent<InputController>();
        if (transform.tag == "Dragon Warrior")
        {
            projectileOffset = new Vector3(1.2f, 0.9f, 0);
            attackDelay = 0.3f;
            isDragon = true;
        }
        else if (transform.tag == "Knight")
        {
            projectileOffset = new Vector3(0, 1.5f, 0);
            attackDelay = 0.5f;
            isDragon = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckAndShoot(projectileOffset, attackDelay, isDragon);

    }
    private void CheckAndShoot(Vector3 projectileOffset, float attackDelay, bool isDragon)
    {
        if(isDragon)
        {
            CheckIsCrouch();
            if (myInput.attackControl is ButtonControl attackButton)
            {
                shootingProjectile = attackButton;
            }
        }
        else
        {
            if (myInput.castControl is ButtonControl castButton)
            {
                shootingProjectile = castButton;
            }
        }
        CheckFaceSide();
        bornPos = new Vector3(transform.position.x + sideMulti * projectileOffset.x,
                              transform.position.y - projectileOffset.y - crouchMulti * crouchOffset,
                              transform.position.z);
        timer += Time.deltaTime;
        if (!shootingProjectile.wasPressedThisFrame) return;
        if (timer > attackDelay)
        {
            GameObject projectileClone = Instantiate(projectilePrefab, bornPos, Quaternion.identity);
            projectileClone.transform.localScale = transform.localScale;
            timer = 0f;
        }
    }
    private void CheckIsCrouch()
    {
        if (moveControl.isCrouch)
        {
            crouchMulti = 1;
            attackDelay = 0.75f;
        }
        else
        {
            crouchMulti = 0;
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
