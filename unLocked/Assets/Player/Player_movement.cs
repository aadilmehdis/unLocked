using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_movement : MonoBehaviour
{
    
    public GameObject Player;

    public bool is_walking;
    public bool is_grounded;


    // setting player speed
    private float speed;
    private float walking_speed = 2f;
    private float running_speed = 5f;
    private float crouching_speed = 2.5f;


    public float rotation_speed;
    public float jump_height;


    Rigidbody rb;
    Animator animator;
    CapsuleCollider player_collider;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        player_collider = GetComponent<CapsuleCollider>();
        is_grounded = true;
    }

    // Update is called once per frame
    void Update()
    {    

        if ( !Input.GetKey(KeyCode.LeftShift) && !Input.GetKeyDown(KeyCode.LeftControl)) 
        {   
            float translation = Input.GetAxis("Vertical") * walking_speed;
            float rotation = Input.GetAxis("Horizontal") * rotation_speed;
            translation *= Time.deltaTime;
            rotation *= Time.deltaTime;

            transform.Translate(0, 0, translation);

            transform.Rotate(0, rotation, 0);
        
                // transform.position += transform.TransformDirection (Vector3.forward) * Time.deltaTime * walking_speed;
            if (translation != 0)
            {
                animator.SetBool("is_walking", true);
                animator.SetBool("is_running", false);

            }
            else
            {
                animator.SetBool("is_walking", false);
                animator.SetBool("is_running", false);

            }

            
        }
        else if ( Input.GetKey(KeyCode.LeftShift) && !Input.GetKeyDown(KeyCode.LeftControl)) 
        {
            Debug.Log("Shift");
            float translation = Input.GetAxis("Vertical") * running_speed;
            float rotation = Input.GetAxis("Horizontal") * rotation_speed;
            translation *= Time.deltaTime;
            rotation *= Time.deltaTime;

            transform.Translate(0, 0, translation);

        
                // transform.position += transform.TransformDirection (Vector3.forward) * Time.deltaTime * walking_speed;
            transform.Rotate(0, rotation, 0);
            
            if (translation != 0)
            {
                animator.SetBool("is_running", true);
                animator.SetBool("is_walking", false);
                translation = 0;
            }
            else
            {
                animator.SetBool("is_running", false);
                animator.SetBool("is_walking", false);
            }

        }
    }
}