using UnityEngine;

public class DragonDamageInfo : DamageInfo
{
    public DragonMove dragonMove;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dragonMove = GetComponent<DragonMove>();
    }
    private void Update()
    {
        if (dragonMove == null) return;
        DragonCollider();
    }
    public void DragonCollider()
    {
        allScriptCWO[0].collideDamage = collideWithOther;
        allScriptCWO[1].collideDamage = basicAttackDamage;
        if (dragonMove.isStrike)
        {
            allScriptCWO[1].collideDamage = strikeDamage;
        }
        else if (dragonMove.isFlyKick)
        {
            allScriptCWO[0].collideDamage = strikeDamage;
        }
    }
}
