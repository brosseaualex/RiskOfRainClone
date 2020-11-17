using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : Enemy
{
    public float flightSpeed = 3f;
    public float hoverHeight = 5f;
    public float distanceOffset = 2.5f;
    public float shootDistance = 5f;

    protected override void Awake()
    {
        base.Awake();
        transform.position += new Vector3(0f, 8f, 0f);
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
    }

    void ShootPlayer()
    {

    }
}
