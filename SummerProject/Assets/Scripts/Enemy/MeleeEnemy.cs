using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class MeleeEnemy : MonoBehaviour, IEnemy {

    [SerializeField]
    public Transform target;
    public float meleeDistance = 1.5f;
    public float aggroDistance = 8f;
    public float meleeCD = 1.0f;
    public float meleeDamage = 10;
    private float meleeStartTime = float.MinValue;

    protected NavMeshAgent ThisAgent = null;
    
    public float health;
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
        health = 60;
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

        // Check if enemy will attack or move
        float curr_distance = CheckPlayerDistance();
        if (curr_distance <= aggroDistance) {
            if (curr_distance <= meleeDistance) {
                if (canAttack()) {
                    Debug.Log("PLAYER HIT");
                    Attack();   // Perform attack animation here and call Attack();
                }
            } else {
                animator.SetBool("Idle", false);
                ResetAnimations();
                Move();
            }
        }
        else {
            if (animator.GetBool("Idle") == false) {
                ResetAnimations();
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
        ThisAgent.SetDestination(target.position);
        SetWalkingAnimation();
    }

    /**
     * Play the death animation, then delete the model after it finishes
     */
    public void Death() {
        Animation death = GetComponent<Animation>(); // Need to get actual animations lol
        animator.SetTrigger("Death");
        Destroy(gameObject, death.clip.length);
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

        if (angle <= 43.75 && angle >= -43.75) {
            animator.SetBool("Forward", true);
        }
        else if (angle < 131.25 && angle > 43.75) {
            animator.SetBool("Right", true);
        }
        else if ((angle <= 180 && angle >= 131.25) || (angle <= -131.25 && angle >= -180)) {
            animator.SetBool("Backward", true);
        }
        else if (angle < -43.75 && angle > -131.25) {
            animator.SetBool("Left", true);
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







}
