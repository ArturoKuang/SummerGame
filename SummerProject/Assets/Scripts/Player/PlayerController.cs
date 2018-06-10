using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerBullet))]
public class PlayerController : MonoBehaviour {

    #region attribute
    public float movementSpeed = 3.0f;
    private PlayerBullet bulletScript;
    private Plane plane;
    private Ray ray;
    private Dash dash;
    private Animator animator;
    private Rigidbody rb;
    private PlayerMelee melee;
	private float xStickAxis;
    private float yStickAxis;
    #endregion



    void Start()
    {
        rb = GetComponent<Rigidbody>();
        dash = GetComponent<Dash>();
        melee = GetComponent<PlayerMelee>();
        bulletScript = GetComponent<PlayerBullet>();
        animator = GetComponent<Animator>();
        plane = new Plane(Vector3.up, Vector3.zero);
    }

    void Update()
    {
        //rotate player around mouse 
        float distance;
        float rotation = 0f;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(plane.Raycast(ray, out distance))
        {
            Vector3 target = ray.GetPoint(distance);
            Vector3 direction = target - transform.position;

            rotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;       
        }

        // rotate player around right stick
        else {
            xStickAxis = Input.GetAxis("RStick X");
            yStickAxis = Input.GetAxis("RStick Y");
            rotation = Mathf.Atan2(xStickAxis, yStickAxis) * Mathf.Rad2Deg;
        }

        // checks for default controller position
        if (rotation != 0) {
            transform.rotation = Quaternion.Euler(0, rotation, 0);
        }

        //Player Movement
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        float x = transform.position.x + moveHorizontal * movementSpeed * Time.deltaTime;
        float z = transform.position.z + moveVertical * movementSpeed * Time.deltaTime;

        //TODO: future code for player animations (use blend tree)
        //if (moveHorizontal != 0 || moveVertical != 0)
        //    animator.SetBool("run", true);
        //else
        //    animator.SetBool("run", false);


        //update player position
        transform.position = new Vector3(x, transform.position.y, z);

        //Player actions 
        if (Input.GetMouseButtonDown(0) && bulletScript.CanStartAbility()) 

        /**
         * Player actions
         * 
         * Below is a full list of controller inputs for PlayStation and XBOX controllers:
         * JoystickButton0 - X (square)
         * JoystickButton1 - A (cross)
         * JoystickButton2 - B (circle)
         * JoystickButton3 - Y (triangle)
         * JoystickButton4  - LB (L1)
         * JoystickButton5  - RB (R1)
         * JoystickButton6  - LT (L2)
         * JoystickButton7  - RT (R2)
         * JoystickButton8 - back
         * JoystickButton9 - start
         * JoystickButton10 - left stick[not direction, button]
         * JoystickButton11 - right stick[not direction, button]
         * 
         * For now, Fire1 is left click and RB (R1), and Dash is left shift and LB (L1)
         */

        if (Input.GetButtonDown("Fire1") && bulletScript.CanStartAbility())
        {
            bulletScript.Shoot();
        }
        if (Input.GetButtonDown("Dash") && dash.CanStartAbility())
        {
            dash.StartAbility();
        }
        if(Input.GetMouseButtonDown(1) && melee.CanStartAbility())
        {
            melee.StartAbility();
            animator.SetBool("Melee", true);
        }
        else
        {
            animator.SetBool("Melee", false);
        }
        if(dash.abilityOn) dash.UpdateMovement();
    }
}
