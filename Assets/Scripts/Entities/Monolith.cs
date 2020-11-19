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

    public GameObject fireParticles;
    public GameObject[] enemyPrefabs;
    public GameObject[] pickupsPrefab;
    public int pickupMinSpawn = 3;
    public int pickupMaxSpawn = 6;

    public bool isGrounded;
    MeshCollider monolithCollider;
    Rigidbody monolithRb;
    AudioSource audioSource;
    NavMeshObstacle navMeshObstacle;
    AudioClip monolithCrashSound;

    void Awake()
    {
        monolithCollider = GetComponent<MeshCollider>();
        monolithRb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        navMeshObstacle = GetComponent<NavMeshObstacle>();
        monolithCrashSound = Resources.Load<AudioClip>("Sounds/Monolith_Crash");
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
        if (hp <= 0)
        {
            OnDeath();
        }
        else
        {
            if (isGrounded)
            {
                hp -= damage;
            }
        }
    }

    public void OnDeath()
    {
        SpawnPickups();
        GameController.Instance.RemoveMonolithFromList(gameObject);
        Destroy(gameObject);
    }

    void IsGrounded()
    {
        isGrounded = Physics.Raycast(monolithCollider.bounds.center - monolithCollider.bounds.extents + new Vector3(0f, 0.1f, 0f), -Vector3.up, 5f, LayerMask.GetMask("Ground"));

        if (isGrounded)
        {
            audioSource.Stop();
            audioSource.clip = monolithCrashSound;
            audioSource.Play();
            monolithRb.isKinematic = true;
            transform.position -= new Vector3(0f, 5.5f, 0f);
            navMeshObstacle.carving = true;
            fireParticles.gameObject.SetActive(false);

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
