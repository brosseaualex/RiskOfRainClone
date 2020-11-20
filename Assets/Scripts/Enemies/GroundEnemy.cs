using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GroundEnemy : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
        unitCollider = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update();

        if (agent.baseOffset >= -0.9f && isDead)
        {
            agent.baseOffset -= 0.3f * Time.deltaTime;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            animator.SetBool("IsAttacking", true);
            float damage = Random.Range(minDamage, maxDamage);
            PlayerController.Instance.TakeDamage(damage);
        }
    }

    void OnCollisionExit()
    {
        animator.SetBool("IsAttacking", false);
    }

    protected override void OnDeath()
    {
        isDead = true;
        agent.speed = 0;
        animator.SetBool("HasDied", true);
        unitCollider.enabled = false;
        GameController.Instance.RemoveEnemyFromList(gameObject);
        Destroy(gameObject, 10f);
    }

    
}
