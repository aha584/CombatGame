using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    public float projectileDamage;

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject opponent = other.gameObject;
        Health opponentHealth = opponent.GetComponent<Health>();
        if (opponentHealth == null) return;
        if (opponent.tag == gameObject.tag) return;
        opponentHealth.TakeDamage(projectileDamage);
        Destroy(gameObject);
    }
}
