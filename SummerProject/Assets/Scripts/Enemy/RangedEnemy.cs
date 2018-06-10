using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class RangedEnemy : MonoBehaviour, IEnemy {

    [SerializeField]
    #region attribute
    public Transform target;
    public Transform bulletSpawn;
    public float health = 60f;
    public float rangedDistance =  4f;
    public float aggroDistance = 7f;
    public float deAggroDistance = 8f;
    public float rangedCD = 1.0f;
    public float rangedDamage = 6f;
    public float rotationSpeed = 10f;
    public float bulletSpeed = 10f;
    public float maxWanderDistance = 1.5f;
    private float rangedStartTime = float.MinValue;
    private bool isAggroed = false;
    private bool isWandering = false;
    private float randTheta;
    private Vector3 initialPosition;
    private Vector3 newPosition;
    #endregion 

    protected NavMeshAgent ThisAgent = null;

    public Rigidbody rb;

    private GameObject player;
    private Player playerScript;
    private Animator animator;
    private Collider map;

    /**
     * Allows modification of health 
     */
    public float Health { get { return health; } set { health = value; } }

    // Use this for initialization
    void Start() {
        ThisAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<Player>();
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update() {
        if (health <= 0) {
            Death();
        }

        ResetAnimations();

        // Check if enemy will attack or move
        float curr_distance = CheckPlayerDistance();
        if (curr_distance <= rangedDistance) {
            StopMoving();
            RotateTowards();
            if (CanAttack()) {
                Debug.Log("PLAYER HIT");
                // PLACEHOLDER, will change once I get attack animations up and running. It will probably be animator.SetBool("Attack", true)
                Attack();
            }
        }
        // Move if enemy is within range of the player
        else if ((curr_distance <= aggroDistance) || (curr_distance <= deAggroDistance && isAggroed)) {
            Move();
        }
        // If the player aggro'd the enemy, but has now gone out of range, then stop
        else if (curr_distance > deAggroDistance || !isAggroed) {
            if (isAggroed) {
                StopMoving();
            }
            else if (!isWandering) {
                initialPosition = transform.position;
                isWandering = true;
                Wander(initialPosition);
            }
            else if (ThisAgent.remainingDistance <= 0.5) {
                Wander(initialPosition);
                SetWalkingAnimation();
            }
        }
        // If out of range completely, then start to wander
        else {

            if (!animator.GetBool("Idle")) {
                animator.SetBool("Idle", true);
            }
        }

        Debug.Log("dist: " + ThisAgent.remainingDistance);


    }

    /**
     * Attacks the player and takes off their health
     * TODO: Implement 
     */
    public void Attack() {
        //bullets spawn and travel
        GameObject obj = EnemyObjectPool.current.GetPooledObject();
        if (obj == null) return;
        obj.transform.position = bulletSpawn.position;
        obj.transform.rotation = bulletSpawn.rotation;
        Rigidbody bulletRB = obj.GetComponent<Rigidbody>();
        Vector3 direction = (target.position - transform.position).normalized;
        bulletRB.velocity =  direction * bulletSpeed;
        obj.SetActive(true);
        rangedStartTime = Time.time;
    }

    /**
     * Simply move towards the player using navmesh
     */
    public void Move() {
        ThisAgent.speed = 3f;
        ThisAgent.isStopped = false;
        isWandering = false;
        isAggroed = true;
        ThisAgent.SetDestination(target.position);
        ResetAnimations();
        SetWalkingAnimation();
    }


    /**
     * Stops the navmesh agent
     */
    public void StopMoving() {
        isAggroed = false;
        ThisAgent.velocity = Vector3.zero;
        ThisAgent.isStopped = true;
    }

    /**
     * Play the death animation, then delete the model after it finishes
     */
    public void Death() {
        animator.SetTrigger("Death");
    }

    /**
     * Checks to see if the melee cool down has refreshed or not
     */
    public bool CanAttack() {
        // Add animation duration later
        return (Time.time - rangedStartTime) > rangedCD;
    }

    /**
     * Returns Distance between enemy and player
     */
    public float CheckPlayerDistance() {
        return Vector3.Distance(target.position, transform.position);
    }

    /**
     * Sets the corresponding walking animation based on angle to player
     */
    public void SetWalkingAnimation() {
        Vector3 targetDir = target.position - transform.position;
        float angle = Vector3.SignedAngle(targetDir, transform.forward, Vector3.up);
        float topLeft = 43.75f, topRight = -43.75f;
        float botLeft = 131.25f, botRight = -131.25f;

        if (angle <= topLeft && angle >= topRight) {
            animator.SetBool("Forward", true);
        }
        else if (angle < botLeft && angle > topLeft) {
            animator.SetBool("Left", true);
        }
        else if ((angle <= 180 && angle >= botLeft) || (angle <= botRight && angle >= -180)) {
            animator.SetBool("Backward", true);
        }
        else if (angle < topRight && angle > botRight) {
            animator.SetBool("Right", true);
        }
    }

    /**
     * Reset all booealn parameters for animation states except for Idle
     */
    public void ResetAnimations() {
        animator.SetBool("Forward", false);
        animator.SetBool("Backward", false);
        animator.SetBool("Left", false);
        animator.SetBool("Right", false);
        animator.SetBool("Idle", false);
    }

    /**
     * Rotates the enemy to the player
     */
    private void RotateTowards() {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    /**
     * Causes the enemy to wander in a slight radius
     */
    private void Wander(Vector3 initialTransform) {
        ThisAgent.speed = 1.5f;
        ThisAgent.isStopped = false;
        randTheta += Random.Range(0, 360);
        float randX = initialTransform.x + (maxWanderDistance * Mathf.Cos(randTheta));
        float randZ = initialTransform.z + (maxWanderDistance * Mathf.Sin(randTheta));
        newPosition = new Vector3(randX, initialTransform.y, randZ);
        ThisAgent.SetDestination(newPosition);
    }








}
