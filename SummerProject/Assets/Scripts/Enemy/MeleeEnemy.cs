using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class MeleeEnemy : MonoBehaviour, IEnemy {

    [SerializeField]
    public Transform target;
    public float health = 60f;
    public float meleeDistance = 1.5f;
    public float aggroDistance = 6f;
    public float deAggroDistance = 9f;
    public float meleeCD = 1.0f;
    public float meleeDamage = 10f;
    private float meleeStartTime = float.MinValue;
    private bool isMoving = false;

    protected NavMeshAgent ThisAgent = null;
    
    public Rigidbody rb;

    private  GameObject player;
    private Player playerScript;
    private Animator animator;


    /**
     * Allows modification of health 
     */
    public float Health { get { return health; } set { health = value; } }

    // Use this for initialization
    void Start () {
        ThisAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<Player>();
        animator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        if (health <= 0) {
            Death();
        }

        ResetAnimations();

        // Check if enemy will attack or move
        float curr_distance = CheckPlayerDistance();
        if (curr_distance <= meleeDistance) {
            StopMoving();
            if (canAttack()) {
                Debug.Log("PLAYER HIT");
                // PLACEHOLDER, will change once I get attack animations up and running. It will probably be animator.SetBool("Attack", true)
                Attack();
            }
        }
        // Move if enemy is within range of the player
        else if ((curr_distance <= aggroDistance) || (curr_distance <= deAggroDistance && isMoving)) {
            Move();
        }
        // If the player aggro'd the enemy, but has now gone out of range, then stop
        else if (curr_distance > deAggroDistance) {
            if (isMoving) {
                StopMoving();
            }
            animator.SetBool("Idle", true);
        }
        else {
            if (!animator.GetBool("Idle")) {
                animator.SetBool("Idle", true);
            }
        }


    }

    /**
     * Attacks the player and takes off their health
     * TODO: Implement the animation of hitting, and slow down or stop the enemy when it is playing this animation.
     */
    public void Attack() {
        playerScript.Health -= meleeDamage;
        meleeStartTime = Time.time;
    }

    /**
     * Simply move towards the player using navmesh
     */
    public void Move() {
        ThisAgent.isStopped = false;
        isMoving = true;
        ThisAgent.SetDestination(target.position);
        ResetAnimations();
        SetWalkingAnimation();
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
    private bool canAttack() {
        // Add animation duration later
        return (Time.time - meleeStartTime) > meleeCD;
    }

    /**
     * Returns Distance between enemy and player
     */
    private float CheckPlayerDistance() {
        return Vector3.Distance(target.position, transform.position);
    }

    /**
     * Sets the corresponding walking animation based on angle to player
     */
    private void SetWalkingAnimation() {
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
    private void ResetAnimations() {
        animator.SetBool("Forward", false);
        animator.SetBool("Backward", false);
        animator.SetBool("Left", false);
        animator.SetBool("Right", false);
        animator.SetBool("Idle", false);
    }

    /**
     * Stops the navmesh agent
     */
    private void StopMoving() {
        isMoving = false;
        ThisAgent.velocity = Vector3.zero;
        ThisAgent.isStopped = true;
    }







}
