using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Player movement class in charge of checking if the player is grounded or not
 * controlling player movement
 * implementing gravity and other physics
 * jumping
 */

public class Movement : MonoBehaviour {

    //retrieve transforms and controllers
    [SerializeField] CharacterController controller;  //character controller
    [SerializeField] Transform cam;                   //main camera view

    //movement
    [SerializeField] float speed = 6f;                //movement speed
    [SerializeField] float turnSmoothTime = 0.1f;     //time taken to rotate player to the direction that the camera is facing
    private float turnSmoothVelocity;                 //speed to rotate player
    [SerializeField] Vector3 moveDirection;           //the direciton of player movement

    //is the player grounded?
    [SerializeField] Transform groundCheck;           //position of the ground check
    [SerializeField] float groundDistance = 0.4f;     //how close to the ground to 
    [SerializeField] bool isGrounded;                 //is the player currently grounded?
    //[SerializeField] LayerMask playerLayer;         //currently playerLayer is accessed directly with the number 7, as the behaviour is different if accessed by variable

    //gravity
    private float gravity = -20f;                     //gravity to be added when falling
    [SerializeField] float currentGravity;            //the current vertical speed of the player
    private float constantGravity = -2f;              //when on ground, acting downward force on the player
    [SerializeField] float maxGravity;                //maximum gravity so as to not go super fast
    private Vector3 gravityDirection = Vector3.down;  //direction of the gravity
    [SerializeField] Vector3 gravityMovement;         //gravity movement vector

    private Vector3 jumpDirection = Vector3.up;       //jump direction
    [SerializeField] float jumpSpeed = 200f;          //jump strength

    /* 
     * UPDATE
     * First it is checked if the player is on the ground
     * then gravity is calcualted
     * then it is checked if the player is jumnping
     * then movement is calcualted
     */
    void Update() {
        IsGrounded(); 
        CalculateGravity();
        CalculateJump();
        CalculateMovement();
    }

    /*
     * Method to check if the player is on the ground or not
     * If no collisions are detected in a sphere underneath the player, then the player is not grounded
     * else they are grounded
     * this ignores collisions on layer 7, which is the player layer. This will need to be changed if player layer number is changed.
     */
    private void IsGrounded() {
        Collider[] collisions = Physics.OverlapSphere(groundCheck.position, groundDistance, 7);
        if (collisions.Length == 0) {
            isGrounded = false;
        } else {
            isGrounded = true;
        }
    }
    
    /* 
     * Calculates the player movement
     * retrives the normalised direction of desired movement, based on input and direction faceing
     * if the input is asking for movement, then turn gradually towards the direction of it
     * and move in the desired direction
     */
    private void CalculateMovement() {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f) {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDirection.normalized * speed * Time.deltaTime);
        }
    }

    /*
     * Calcualtes the downward velocity of the player
     * if the player is grounded, set constant downward velocity to keep the player firmly on the ground
     * if not grounded, increase downward velocity, up to a point (maxGravity).
     */
    private void CalculateGravity() {
        if (isGrounded && currentGravity < 0) {
            currentGravity = constantGravity;
        } else {
            if (currentGravity > maxGravity) {
                currentGravity += gravity * Time.deltaTime;
            }
        }

        gravityMovement = gravityDirection * -currentGravity;
        controller.Move(gravityMovement * Time.deltaTime);
    }

    /*
     * Calcaulte jump
     * set vertical velocity upwards
     */
    private void CalculateJump() {
        if(Input.GetButtonDown("Jump") && isGrounded) {
            currentGravity = jumpSpeed;
            gravityMovement = currentGravity * jumpDirection;
        }
        controller.Move(gravityMovement * Time.deltaTime);
    }
}
