using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    public float hurtSide;

    public float timer;
    public float delayDOT;
    public float dotToTake;
    public bool isTakeDOT = false;

    public Action onDead;
    public Action onHealthChange;
    public Action onDizzy;
    public Action onWin;
    public Action onHurt;

    public AnimController myAnimControl;
    public MoveController myMoveControl;
    public GameObject explosionPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myAnimControl = GetComponent<AnimController>();
        myMoveControl = GetComponent<MoveController>(); 

        currentHealth = maxHealth;
        onDead += OnDead;
        onDead += myAnimControl.DieAnim;
        onDizzy += myAnimControl.DizzyAnim;
        onHurt += myAnimControl.HurtAnim;
        onHurt += () => myMoveControl.HurtPhysic(hurtSide);
        onWin += myAnimControl.WinAnim;
    }

    private void Update()
    {
        if (isTakeDOT)
        {
            if(timer >= delayDOT)
            {
                TakeDamage(dotToTake);
                timer = 0;
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0) return;
        currentHealth -= damage;

        GameObject exploClone = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        
        if (currentHealth <= 0)
        {
            onDead?.Invoke();
        }
        else
        {
            onHealthChange?.Invoke();
            onHurt?.Invoke();
        }

        Destroy(exploClone, 0.5f);
    }

    public void OnDead()
    {
        Destroy(gameObject, 3f);
    }

    public void OnTakeDOT(AreaDamage areaScript)
    {
        delayDOT = areaScript.delayDOT;
        timer = delayDOT;
        dotToTake = areaScript.areaDamage;
        isTakeDOT = true;
    }
    public void ResetDOT()
    {
        isTakeDOT = false;
        timer = 0f;
        delayDOT = 0f;
        dotToTake = 0f;
    }
}
