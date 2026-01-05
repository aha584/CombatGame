using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed;


    // Update is called once per frame
    void Update()
    {
        transform.position += _speed * Time.deltaTime * transform.up;
    }
}
