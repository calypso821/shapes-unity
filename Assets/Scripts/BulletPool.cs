using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int poolSize = 50;

    private GameObject[] _bulletPool;
    
    private void Start()
    {
        _bulletPool = new GameObject[poolSize];

        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform, true);
            bullet.SetActive(false);
            _bulletPool[i] = bullet;
        }
    }

    public GameObject GetBullet()
    {
        foreach (var bullet in _bulletPool)
        {
            if (!bullet.activeInHierarchy)
            {
                bullet.SetActive(true);
                return bullet;
            }
        }

        return null;
    }
}
