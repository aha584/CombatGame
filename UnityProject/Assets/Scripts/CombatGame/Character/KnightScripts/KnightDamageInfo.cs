using UnityEngine;

public class KnightDamageInfo : DamageInfo
{
    public float blockDamage;
    public float jumpAttackDamage;

    // Update is called once per frame
    void Start()
    {
        KnightCollide();
    }
    public void KnightCollide()
    {
        allScriptCWO[0].collideDamage = collideWithOther;
        allScriptCWO[1].collideDamage = basicAttackDamage;
        allScriptCWO[2].collideDamage = blockDamage;
        allScriptCWO[3].collideDamage = jumpAttackDamage;
        allScriptCWO[4].collideDamage = strikeDamage;
    }
}
