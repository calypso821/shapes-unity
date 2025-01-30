using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip deathSound;

    public enum EnemyType
    {
        Sphere,
        Cube,
        Cylinder
    }

    [Header("Enemy Type Settings")]
    [SerializeField] private EnemyType enemyType;

    [SerializeField] private int health = 20;
    [SerializeField] private float attackDistance = 2f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private int attackDamage = 5;

    private float _lastDamageTime;
    private bool isDead = false; // Prepreči večkratno sprožitev smrti
    private bool isFrozen = false; // Dodana spremenljivka za zamrznitev
    private float freezeDuration = 0.1f; // Trajanje zamrznitve (0.5 sekunde)
    
    private PlayerController _player;
    private Transform _playerTransform;
    private NavMeshAgent _agent;

    private ScoreManager _scoreManager;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            _playerTransform = player.transform;
            _player = _playerTransform.transform.GetComponent<PlayerController>();
        }
        else
        {
            Debug.LogWarning("Player transform not found");
        }
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (isDead) return; // Če je sovražnik mrtev, ne izvajaj logike gibanja ali napadov
        if (isFrozen) return; // Če je zamrznjen, ne izvaja gibanja ali napadov

        var dist = Vector3.Distance(_playerTransform.position, transform.position);

        // Če je sovražnik blizu igralca, se ustavi in gleda proti igralcu
        if (dist <= attackDistance)
        {
            _agent.enabled = false;
            Vector3 directionToPlayer = _playerTransform.position - transform.position;

            directionToPlayer.y = 0;

            // Če smer ni ničelna, rotiraj proti igralcu
            if (directionToPlayer.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = targetRotation;
            }

            if (Time.time >= _lastDamageTime + attackCooldown)
            {
                _player.TakeDamage(attackDamage);
                _lastDamageTime = Time.time;
            }
        }
        else
        {
            _agent.enabled = true;
            _agent.SetDestination(_playerTransform.position);
        }
    }

    public EnemyType GetEnemyType()
    {
        return enemyType;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; // Prepreči nadaljnjo obdelavo, če je sovražnik že mrtev

        health -= damage;
        PlayHitSound();
        Freeze();

        if (health <= 0)
        {
            Die();
        }
    }

    private void PlayHitSound()
    {
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
    }

    private void Die()
    {
        if (isDead) return; // Prepreči večkratno sprožitev smrti

        isDead = true;

        // Predvajaj zvok smrti na ločenem GameObjectu
        if (deathSound != null)
        {
            GameObject tempAudio = new GameObject("DeathSound");
            AudioSource tempAudioSource = tempAudio.AddComponent<AudioSource>();
            tempAudioSource.clip = deathSound;
            tempAudioSource.Play();

            // Uniči začasni GameObject po koncu zvoka
            Destroy(tempAudio, deathSound.length);
        }

        // Dodaj točkovanje pred uničenjem sovražnika
        ScoreManager.Instance.AddKillScore();

        // Uniči sovražnika takoj
        Destroy(gameObject);
    }

    // Nova funkcionalnost za zamrznitev sovražnika
    public void Freeze()
    {
        if (isFrozen) return; // Prepreči večkratno zamrznitev
        isFrozen = true;
        _agent.enabled = false; // Izklopi gibanje, medtem ko je zamrznjen
        StartCoroutine(UnfreezeAfterDelay());
    }

    private IEnumerator UnfreezeAfterDelay()
    {
        yield return new WaitForSeconds(freezeDuration); // Čakaj pol sekunde
        isFrozen = false;
        _agent.enabled = true; // Ponovno omogoči gibanje
    }
}
