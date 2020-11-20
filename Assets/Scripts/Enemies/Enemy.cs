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
    protected Vector3 agentVelocity;

    [Header("Unit Settings")]
    public float hp = 50f;
    public float sightRange = 20f;
    public float minDamage = 0.1f;
    public float maxDamage = 0.3f;
    protected bool isDead;

    protected virtual void Update()
    {
        target = PlayerController.Instance.transform;

        if (hp > 0)
        {
            SearchPlayer();
        }
        else if (hp <= 0 && isDead == false)
        {
            OnDeath();
        }        
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;        
    }

    protected virtual void OnDeath()
    {
    }

    protected virtual void SearchPlayer()
    {
        Vector3 playerPosition;
        Vector3 enemyPosition;

        playerPosition = target.position;
        enemyPosition = transform.position;

        float distance = Vector3.Distance(playerPosition, enemyPosition);

        if (PlayerController.Instance.stats.hp > 0 && distance < sightRange)
        {
            agent.SetDestination(target.position);
            animator.SetBool("IsMoving", true);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
    }
}
