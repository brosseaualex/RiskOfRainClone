using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monolith : MonoBehaviour, IDamageable
{
    public float hp = 250;
    public float enemySpawnDelayMin = 3f;
    public float enemySpawnDelayMax = 8f;
    float enemySpawnDelay;
    float timeUntilNextSpawn = 0f;
    float spawnRadiusMin = -8f;
    float spawnRadiusMax = 8f;

    public ParticleSystem explosionParticlesPrefab;
    public GameObject fireParticlesParent;
    public GameObject smokeParticlesParent;
    public GameObject[] enemyPrefabs;
    public GameObject[] pickupsPrefab;
    public int pickupMinSpawn = 3;
    public int pickupMaxSpawn = 6;

    bool isGrounded;
    bool isDead;
    MeshCollider monolithCollider;
    MeshRenderer monolithRenderer;
    Rigidbody monolithRb;
    AudioSource audioSource;
    NavMeshObstacle navMeshObstacle;
    AudioClip monolithCrashClip;
    AudioClip monolithExplosionClip;

    void Awake()
    {
        monolithCollider = GetComponent<MeshCollider>();
        monolithRenderer = GetComponent<MeshRenderer>();
        monolithRb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        navMeshObstacle = GetComponent<NavMeshObstacle>();
        monolithCrashClip = Resources.Load<AudioClip>("Sounds/Monolith_Crash");
        monolithExplosionClip = Resources.Load<AudioClip>("Sounds/Monolith_Explosion");
    }

    void Update()
    {
        if (!monolithRb.isKinematic)
        {
            IsGrounded();
        }
        else
        {
            isGrounded = true;
        }

        if (isGrounded && Time.time > timeUntilNextSpawn)
        {
            SpawnEnemy();
            enemySpawnDelay = Random.Range(enemySpawnDelayMin, enemySpawnDelayMax);
            timeUntilNextSpawn = Time.time + enemySpawnDelay;
        }

        if (hp <= 0 && !isDead)
        {
            OnDeath();
        }
    }

    public void SpawnEnemy()
    {
        int rndEnemy = Random.Range(0, enemyPrefabs.Length);

        GameObject enemyClone = Instantiate(enemyPrefabs[rndEnemy], new Vector3(transform.position.x + Random.Range(spawnRadiusMin, spawnRadiusMax), transform.position.y, transform.position.z + Random.Range(-8, 8)), Quaternion.identity);
        enemyClone.transform.SetParent(GameController.Instance.enemyParent.transform);
        GameController.Instance.AddEnemyToList(enemyClone);
    }

    public void TakeDamage(float damage)
    {
        if (isGrounded)
        {
            hp -= damage;
        }
    }

    void OnDeath()
    {
        isDead = true;
        timeUntilNextSpawn = 10f;
        monolithCollider.enabled = false;
        monolithRenderer.enabled = false;
        fireParticlesParent.SetActive(false);
        smokeParticlesParent.SetActive(false);

        audioSource.clip = monolithExplosionClip;
        audioSource.spatialBlend = 0.5f;
        audioSource.Play();

        ParticleSystem explosionClone = Instantiate(explosionParticlesPrefab, monolithRenderer.bounds.center, Quaternion.identity);

        SpawnPickups();
        GameController.Instance.RemoveMonolithFromList(gameObject);
        Destroy(explosionClone.gameObject, 3.9f);
        Destroy(gameObject, 4.5f);
    }

    void IsGrounded()
    {
        isGrounded = Physics.Raycast(monolithCollider.bounds.center - monolithCollider.bounds.extents + new Vector3(0f, 0.1f, 0f), -Vector3.up, 5f, LayerMask.GetMask("Ground"));

        if (isGrounded)
        {
            audioSource.Stop();
            audioSource.clip = monolithCrashClip;
            audioSource.Play();
            monolithRb.isKinematic = true;
            transform.position -= new Vector3(0f, 5.5f, 0f);
            navMeshObstacle.carving = true;

            enemySpawnDelay = Random.Range(enemySpawnDelayMin, enemySpawnDelayMax);
            timeUntilNextSpawn = Time.time + enemySpawnDelay;
        }
    }

    void SpawnPickups()
    {
        int spawnQty = Random.Range(pickupMinSpawn, pickupMaxSpawn + 1);

        for (int i = 0; i < spawnQty; i++)
        {
            int rndSpawn = Random.Range(0, pickupsPrefab.Length);
            Vector3 spawnPos = GameController.Instance.FindSpawnPosition();

            GameObject pickupClone = GameObject.Instantiate(pickupsPrefab[rndSpawn], spawnPos, Quaternion.identity);
            pickupClone.transform.SetParent(GameController.Instance.pickupsParent.transform);
        }
    }
}
