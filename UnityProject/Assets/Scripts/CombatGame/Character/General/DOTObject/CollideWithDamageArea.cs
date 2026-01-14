using UnityEngine;

public class CollideWithDamageArea : MonoBehaviour
{
    Health myHealth;

    private void Start()
    {
        myHealth = GetComponent<Health>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject area = other.gameObject;
        AreaDamage areaScript = area.GetComponent<AreaDamage>();
        if (myHealth == null || areaScript == null) return;
        myHealth.OnTakeDOT(areaScript);
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        myHealth.ResetDOT();
    }
}
