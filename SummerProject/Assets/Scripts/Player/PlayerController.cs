using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerBullet))]
public class PlayerController : MonoBehaviour {

    #region attribute
    public float movementSpeed = 3.0f;
    private PlayerBullet bulletScript;
    private Player player;
    private Plane plane;
    private Ray ray;
    private Dash dash;
    private Animator animator;
    private Rigidbody rb;
    private Vector3 lastPosition = Vector3.zero;
    #endregion

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        dash = GetComponent<Dash>();
        player = GetComponent<Player>();
        bulletScript = GetComponent<PlayerBullet>();
        animator = GetComponent<Animator>();
        plane = new Plane(Vector3.up, Vector3.zero);
    }

    void Update()
    {

        //rotate player around mouse 
        float distance;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(plane.Raycast(ray, out distance))
        {
            Vector3 target = ray.GetPoint(distance);
            Vector3 direction = target - transform.position;
            float rotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, rotation, 0);
        }

        //Player Movement
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        float x = transform.position.x + moveHorizontal * movementSpeed * Time.deltaTime;
        float z = transform.position.z + moveVertical * movementSpeed * Time.deltaTime;

        //player animations
        Vector3 playerDirection = transform.position - lastPosition;
        playerDirection.Normalize();
        Vector3 localDirection = transform.InverseTransformDirection(playerDirection);
        lastPosition = transform.position;

        if(localDirection.z > 0.1)
        {
            animator.SetBool("run", true);
        }
        else if(localDirection.z < -0.1)
        {
            animator.SetBool("run_back", true);
        }
        else if(localDirection.x > 0.0)
        {
            animator.SetBool("right", true);
        }
        else if (localDirection.x < -0.0)
        {
            animator.SetBool("left", true);
        }
        else
        {
            animator.SetBool("right", false);
            animator.SetBool("left", false);
            animator.SetBool("run", false);
            animator.SetBool("run_back", false);
        }


        //update player position
        transform.position = new Vector3(x, transform.position.y, z);

        //Player actions 
        if (Input.GetMouseButtonDown(0) && bulletScript.CanStartAbility()) 
        {
            bulletScript.Shoot();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && dash.CanStartAbility())
        {
            dash.StartAbility();
        }
        if(dash.abilityOn) dash.UpdateMovement();
    }
}
