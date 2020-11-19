using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : Enemy
{
    [Header("Base stats")]
    public float flightSpeed = 3f;
    public float hoverHeight = 5f;
    public float distanceOffset = 2.5f;

    [Header("Shooting stats")]
    public float shootingRange = 12f;
    public float rateOfFire = 0.5f;
    public LineRenderer bulletTrailPrefab;

    float spawnHeight = 8f;
    float timeBeforeNextShot = 0f;

    protected override void Awake()
    {
        base.Awake();
        unitCollider = GetComponent<SphereCollider>();
        transform.position += new Vector3(0f, spawnHeight, 0f);
    }

    protected override void SearchPlayer()
    {
        Vector3 playerPosition;
        Vector3 enemyPosition;

        playerPosition = target.position;
        enemyPosition = transform.position;

        float distance = Vector3.Distance(playerPosition, enemyPosition);

        if (distance < sightRange)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(target.position.x + distanceOffset, target.position.y + hoverHeight, target.position.z + distanceOffset), flightSpeed * Time.deltaTime);
        }

        if (distance < shootingRange)
        {
            if (Time.time >= timeBeforeNextShot)
            {
                ShootPlayer();
            }
        }        
    }

    void ShootPlayer()
    {
        RaycastHit enemyhit;
        Vector3 lineRendererEnd;

        if (Physics.Raycast(transform.position, (target.position - transform.position + new Vector3(0f, 1.5f, 0f)), out enemyhit, shootingRange))
        {
            //Debug.DrawRay(transform.position, (target.position - transform.position), Color.red);

            IDamageable damageable = enemyhit.transform.GetComponent<IDamageable>();

            if (damageable != null)
            {
                float damage = Random.Range(minDamage, maxDamage);
                damageable.TakeDamage(damage);
            }

            lineRendererEnd = enemyhit.point;
        } else
        {
            lineRendererEnd = target.position - transform.position * shootingRange;
        }

        LineRenderer bulletTrailClone = Instantiate(bulletTrailPrefab);
        bulletTrailClone.SetPositions(new Vector3[] { transform.position + new Vector3(0f, 0.05f, 0f), lineRendererEnd });

        StartCoroutine(GameController.Instance.FadeLineRenderer(bulletTrailClone));

        timeBeforeNextShot = Time.time + rateOfFire;
    }
}
