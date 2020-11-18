using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Unit Components")]
    protected Collider unitCollider;
    protected Animator animator;
    protected Transform target;
    Vector3 originalPosition;
    NavMeshAgent agent;

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
        originalPosition = transform.position;
    }

    protected virtual void Update()
    {
        SearchPlayer();

        if (hp <= 0)
        {
            OnDeath();
        }
    }

    protected virtual void OnCollisionStay(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            float damage = Random.Range(minDamage, maxDamage);
            //player = collision.gameObject.GetComponent<PlayerController>();
            PlayerController.Instance.TakeDamage(damage);
        }
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
    }

    public void OnDeath()
    {
        GameController.Instance.RemoveEnemyFromList(gameObject);
        Destroy(gameObject);
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
