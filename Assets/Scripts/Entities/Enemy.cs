using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Unit Components")]
    protected Collider unitCollider;
    protected Animator animator;
    protected Transform target;
    protected Vector3 originalPosition;
    protected NavMeshAgent agent;
    protected MeshRenderer meshRenderer;

    [Header("Unit Settings")]
    public float hp = 50f;
    public float sightRange = 4f;
    public float minDamage = 0.1f;
    public float maxDamage = 0.3f;

    protected virtual void Awake()
    {
        target = PlayerController.Instance.transform;
        agent = GetComponent<NavMeshAgent>();
        unitCollider = GetComponent<BoxCollider>();
        animator = GetComponent<Animator>();
        meshRenderer = GetComponent<MeshRenderer>();
        originalPosition = transform.position;
    }

    protected virtual void Update()
    {
        if (hp > 0)
        {
            SearchPlayer();
        }
        else
        {
            OnDeath();
        }
    }

    protected virtual void OnCollisionStay(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            float damage = Random.Range(minDamage, maxDamage);
            PlayerController.Instance.TakeDamage(damage);
        }
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
    }

    public void OnDeath()
    {
        meshRenderer.enabled = false;
        unitCollider.enabled = false;
        GameController.Instance.RemoveEnemyFromList(gameObject);
        Destroy(gameObject, 3.5f);
    }

    protected virtual void SearchPlayer()
    {
        Vector3 playerPosition;
        Vector3 enemyPosition;

        playerPosition = target.position;
        enemyPosition = transform.position;

        float distance = Vector3.Distance(playerPosition, enemyPosition);

        if (PlayerController.Instance.stats.hp > 0)
        {
            if (distance < sightRange)
            {
                agent.SetDestination(target.position);
            }
        }
        else
        {
            agent.SetDestination(originalPosition);
        }
    }
}
