using TMPro;
using UnityEngine;

public class MPlayer : MonoBehaviour
{
    [SerializeField] private float _delayShoot;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Camera _cam;
    [SerializeField] private Transform _firePoint;

    private float timer = 0;
    
    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
        timer += Time.deltaTime;
    }

    private void Shoot()
    {
        if(timer > _delayShoot)
        {
            timer = 0;
            Vector3 worldPos = _cam.ScreenToWorldPoint(Input.mousePosition);
            worldPos = new Vector3(worldPos.x, worldPos.y, 0);
            Vector3 dir = worldPos - _firePoint.position;
            dir = dir.normalized;
            GameObject bullet = Instantiate(_bulletPrefab, _firePoint.position, Quaternion.identity);
            bullet.transform.up = dir;
        }
    }
}
