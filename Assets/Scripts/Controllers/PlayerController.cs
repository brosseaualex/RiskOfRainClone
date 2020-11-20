using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum JumpingState
{
    Landed,
    Jumping,
    Falling
}

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour, IDamageable
{
    #region Singleton
    private static PlayerController instance;

    private PlayerController() { }

    public static PlayerController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PlayerController();
            }
            return instance;
        }
    }
    #endregion

    private Camera camera;
    private HudController hudController;

    [Header("Unit Components")]
    public Rigidbody rigidBody;
    public CapsuleCollider unitCollider;
    public Animator animator;
    public GameObject gunTipPosition;
    public LineRenderer bulletTrailPrefab;    
    public ParticleSystem muzzleFlashParticles;
    public ParticleSystem cartridgeEjectionParticles;
    AudioSource gunSound;

    [Header("Unit Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float maxSpeed = 14f;
    [HideInInspector]
    public float currentSpeed;
    [Header("Jump Settings")]
    public float baseJumpForce = 40f;
    [Tooltip("The extra gravity is applied when player jumps to remove a bit of the 'floaty' feel when landing")]
    public float extraGravity = 25f;

    public EntityStats stats;    

    JumpingState jumpingState;
    Vector2 movement;
    Vector2 mouseDelta;
    bool isGrounded;
    bool jumpPressed;
    bool shootPressed;

    private void Start()
    {
        instance = this;
        camera = gameObject.GetComponentInChildren<Camera>();
        hudController = GameObject.FindObjectOfType<HudController>();
        gunSound = gameObject.GetComponent<AudioSource>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (!GameController.Instance.hasLost && !GameController.Instance.hasWon)
        {
            CheckInput();
        }

        IsGrounded();

        UpdateAnimations();

        UpdatePlayerData();
    }

    private void FixedUpdate()
    {
        if (!GameController.Instance.isPaused)
        {
            if (!GameController.Instance.hasLost && !GameController.Instance.hasWon)
            {
                Move();
                AddExtraGravity();
            }
        }
    }

    void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            hudController.ShowMenu();
        }

        movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        mouseDelta = new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));

        jumpPressed = Input.GetButtonDown("Jump");
        shootPressed = Input.GetAxis("Fire1") > 0;

        if (isGrounded && jumpPressed)
        {
            jumpingState = JumpingState.Jumping;
            Jump();
            
        }

        if (shootPressed && isGrounded)
        {
            Shoot();
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = runSpeed;
        }
        else
        {
            currentSpeed = walkSpeed;
        }
    }

    void Move()
    {
        movement.Normalize();

        rigidBody.velocity = transform.forward * movement.y * currentSpeed + transform.right * movement.x * currentSpeed + Vector3.up * rigidBody.velocity.y;
        rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, maxSpeed);

        animator.SetFloat("MovementForward", movement.y);
        animator.SetFloat("MovementRight", movement.x);
        animator.SetFloat("Speed", rigidBody.velocity.magnitude / maxSpeed);
    }

    void Shoot()
    {
        if (Time.time >= stats.timeBeforeNextShot)
        {
            gunSound.Play();

            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            RaycastHit playerRaycastHit;
            Vector3 lineRendererEnd;
            //Debug.DrawRay(ray.origin, ray.direction * stats.weaponRange, Color.red, 3f);
            if (Physics.Raycast(ray, out raycastHit, stats.weaponRange, stats.gunHitLayers.value))
            {
                //Debug.DrawLine(ray.origin + Vector3.zero, raycastHit.point - Vector3.zero, Color.red);

                Physics.Raycast(transform.position + new Vector3(0f, 1.6f, 0f), raycastHit.point - transform.position - new Vector3(0f, 1.6f, 0f), out playerRaycastHit, stats.weaponRange, stats.gunHitLayers.value);

                IDamageable damageable = playerRaycastHit.transform.GetComponent<IDamageable>();

                if (damageable != null)
                {
                    float weaponDamage = Random.Range(stats.minWeaponDamage, stats.maxWeaponDamage);
                    damageable.TakeDamage(weaponDamage);

                    //Debug.Log("Weapon damage: " + weaponDamage);
                }

                lineRendererEnd = playerRaycastHit.point;

                //Debug.DrawRay(transform.position + new Vector3(0f, 1.6f, 0f), raycastHit.point - transform.position - new Vector3(0f, 1.6f, 0f), Color.red);
                //Debug.Log("Pow " + playerRaycastHit.transform.name);
            }
            else
            {
                lineRendererEnd = gunTipPosition.transform.position + ray.direction * stats.weaponRange;
            }

            LineRenderer bulletTrailClone = Instantiate(bulletTrailPrefab);
            bulletTrailClone.SetPositions(new Vector3[] { gunTipPosition.transform.position, lineRendererEnd });

            StartCoroutine(GameController.Instance.FadeLineRenderer(bulletTrailClone));

            muzzleFlashParticles.Play();
            cartridgeEjectionParticles.Play();

            stats.timeBeforeNextShot = Time.time + stats.rateOfFire;
        }
    }

    void UpdateAnimations()
    {
        if (!isGrounded)
        {
            jumpingState = JumpingState.Falling;
        }
        else
        {
            jumpingState = JumpingState.Landed;
        }

        if (isGrounded && jumpPressed)
        {
            jumpingState = JumpingState.Jumping;
        }

        animator.SetFloat("Health", stats.hp);
        animator.SetInteger("JumpingState", (int)jumpingState);
    }

    public void TakeDamage(float damage)
    {
        if (stats.hp > 0)
        {
            stats.hp -= damage;
        }
    }

    void IsGrounded()
    {
        isGrounded = Physics.Raycast(unitCollider.bounds.center - unitCollider.bounds.extents + new Vector3(0.25f, 0.25f, 0.25f), -Vector3.up, 1.5f, LayerMask.GetMask("Ground", "Box"));
    }

    void Jump()
    {
        rigidBody.AddForce(Vector3.up * baseJumpForce, ForceMode.Impulse);
    }

    void AddExtraGravity()
    {
        rigidBody.AddForce(Vector3.down * extraGravity);
    }

    void UpdatePlayerData()
    {
        hudController.UpdatePlayerHud(stats);
    }

    public void Pickup(PassivePickup.PassiveStats passiveStats)
    {
        stats += passiveStats.stats;
        hudController.UpdatePassiveHud(passiveStats);
    }

    public void OnDeath()
    {
    }

}

