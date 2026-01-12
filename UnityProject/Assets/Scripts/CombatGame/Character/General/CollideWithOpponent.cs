using UnityEngine;

public class CollideWithOpponent : MonoBehaviour
{
    public float collideDamage;

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject opponent = other.gameObject;
        Health opponentHealth = opponent.GetComponent<Health>();
        if (opponentHealth == null) return;
        if (opponentHealth.tag == gameObject.tag) return;
        opponentHealth.TakeDamage(collideDamage);
    }
}
