using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public enum BulletType
    {
        Sphere,
        Cube,
        Cylinder
    }

    [SerializeField] private BulletType bulletType;
    
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioSource audioSource;

    private ParticleEffectPool _effectPool;

    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float checkRadius = 0.5f;
    [SerializeField] public int damage = 5;

    private readonly Collider[] _overlaps = new Collider[10];
    private float _timeActive;

    private void Awake()
    {
        GameObject poolObject = GameObject.FindWithTag(bulletType.ToString() + "Pool");
        _effectPool = poolObject.GetComponent<ParticleEffectPool>();
        if (_effectPool != null)
        {
            Debug.Log("Found ParticleEffectPool: " + poolObject.name);
        }
        else
        {
            Debug.LogError("ParticleEffectPool script not found on the object!");
        }
    }

    private void OnEnable()
    {
        _timeActive = 0f;
    }

    private void PlayHitSound()
    {
        if (audioSource && hitSound)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
        }
        else
        {
            Debug.LogWarning("audioSource or hitSound missing");
        }
    }


    public void CheckSpawn()
    {
        Array.Clear(_overlaps, 0, _overlaps.Length);
        int length = Physics.OverlapSphereNonAlloc(transform.position, checkRadius, _overlaps);
        if (length > 0)
        {
            for (int i = 0; i < length; i++)
            {
                var col = _overlaps[i];
                
                if (!col) break;
                
                
                
                if (col.CompareTag("Enemy"))
                {
                    Enemy enemy = col.GetComponent<Enemy>();

                    if (enemy)
                    {
                        if (CanDamageEnemyType(enemy.GetEnemyType()))
                            enemy.TakeDamage(damage);
                    }

                    gameObject.SetActive(false);
                }
            }
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        _timeActive += Time.deltaTime;
        if (_timeActive >= lifetime)
        {
            DeactivateBullet();
            return;
        }

        // Ray ray = new Ray(transform.position, transform.forward);
        // if (Physics.SphereCast(ray, checkRadius, out RaycastHit hitInfo, speed * Time.deltaTime))
        // {
        //     Debug.Log("Collider detected: " + hitInfo.collider.gameObject.name + ", Tag: " + hitInfo.collider.gameObject.tag);
        //     if (hitInfo.collider.CompareTag("Enemy"))
        //     {
        //         Enemy enemy = hitInfo.collider.GetComponent<Enemy>();
        //
        //         if (enemy)
        //         {
        //             if (CanDamageEnemyType(enemy.GetEnemyType()))
        //                 enemy.TakeDamage(damage);
        //         }
        //     }
        //     gameObject.SetActive(false);
        // }
        
        // CheckSpawn();
    
        transform.Translate(Vector3.forward * (speed * Time.deltaTime));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
    
            if (enemy)
            {
                if (CanDamageEnemyType(enemy.GetEnemyType()))
                    enemy.TakeDamage(damage);
            }
        }
        DeactivateBullet();
    }

    private void DeactivateBullet()
    {
        PlayHitSound();
        if (_effectPool)
        {
            
            ParticleSystem effect = _effectPool.GetEffect();
            effect.transform.position = transform.position;
            effect.transform.rotation = Quaternion.identity;

            _effectPool.StartCoroutine(ReturnEffectToPool(effect));
        }
        gameObject.SetActive(false);
    }

    private IEnumerator ReturnEffectToPool(ParticleSystem effect)
    {
        yield return new WaitForSeconds(effect.main.duration + effect.main.startLifetime.constantMax);
        _effectPool.ReturnEffect(effect);
    }

    bool CanDamageEnemyType(Enemy.EnemyType enemyType)
    {
        return (int)bulletType == (int)enemyType;

    }
}
