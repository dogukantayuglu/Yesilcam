using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public Animator animator;
    private Rigidbody2D m_Rigidbody2D;
    public LayerMask groundLayer;
    [HideInInspector] public bool attackAnimationIsPlaying;
    [HideInInspector] public bool takingDamage;



    public float runSpeed = 40f;

    private float horizontalMove = 0f;
    private bool jump = false;
    private bool crouch = false;
    private bool canStanupNow;



    //Functions
    void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        attackAnimationIsPlaying = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.ToString().Contains("Attack");
        takingDamage = animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerTakeDamage");
        //Horizontal Movement
        if (attackAnimationIsPlaying || takingDamage)
        {
            horizontalMove = 0f;
            animator.SetFloat("Speed", 0f);
        }
        else
        {
            horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
            animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        }


        //Jumping and Falling
        var playerVerticalVelocity = m_Rigidbody2D.velocity.y;
        bool isUnderCeiling = controller.CeilingCheck();


        if (playerVerticalVelocity > 4 && !controller.IsGrounded())
        {
            animator.SetBool("IsJumping", true);
            animator.SetBool("IsFalling", false);
        }

        if (playerVerticalVelocity < -4 && !controller.IsGrounded())
        {
            animator.SetBool("IsFalling", true);
            animator.SetBool("IsJumping", false);
        }

        if (controller.IsGrounded())
        {
            animator.SetBool("IsFalling", false);
            animator.SetBool("IsJumping", false);
        }

        if (Input.GetButtonDown("Jump") && !isUnderCeiling && !takingDamage)
        {
            jump = true;
            animator.SetBool("IsCrouching", false);
        }


        //Crouching Mechanics
        if (Input.GetButtonDown("Crouch") && controller.IsGrounded())
        {
            crouch = true;
            animator.SetBool("IsCrouching", true);
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            canStanupNow = true;
        }

        if (!controller.CeilingCheck())
        {
            if (canStanupNow)
            {
                crouch = false;
                animator.SetBool("IsCrouching", false);
                canStanupNow = false;
            }
        }

    }


    void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);

        jump = false;
    }
}

