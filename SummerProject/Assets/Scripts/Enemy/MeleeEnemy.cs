using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class MeleeEnemy : MonoBehaviour, IEnemy {

    [SerializeField]
    public Transform target;
    protected NavMeshAgent ThisAgent = null;
    private float meleeCD = 1.0f;
    private float meleeDistance = 1.5f;
    private float meleeDamage = 10;

    private float meleeStartTime = float.MinValue;

    public float health;
    public Rigidbody rb;

    public GameObject player;
    public Player playerScript;


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
    }
	
	// Update is called once per frame
	void Update () {
        if (health <= 0) {
            Death();
        }

		if (InRange() && canAttack()) {
            Debug.Log("COLLIDED");
            // Perform attack animation here and call Attack();
            Attack();
        }

        Move();
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
     * Simply move towards the player
     */
    public void Move() {
        ThisAgent.SetDestination(target.position);
    }

    /**
     * Play the death animation, then delete the model after it finishes
     */
    public void Death() {
        Animation death = GetComponent<Animation>(); // Need to get actual animations lol
        death.Play();
        Destroy(gameObject, death.clip.length);
    }

    /**
     * Checks if enemy is in range of player to perform an attack
     */
    private bool InRange() {
        if (Vector3.Distance(target.position, transform.position) <= meleeDistance) {
            return true;
        }

        return false;
    }

    private bool canAttack() {
        // Add animation duration later
        return (Time.time - meleeStartTime) > meleeCD;
    }



}
