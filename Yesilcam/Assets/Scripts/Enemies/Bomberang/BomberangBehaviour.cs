using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class BomberangBehaviour : MonoBehaviour
{
    #region Public Variables

    public float moveSpeed;
    public float timer;
    public LayerMask collisionLayer;
    public int explosionRange = 4;
    [HideInInspector] public bool isFacingLeft;
    [HideInInspector] public bool inRange;



    #endregion

    #region Private Variables
    private Animator anim;
    private Transform target;
    private Enemy_Behaviour enemyBehaviour;
    private Rigidbody2D rigidBody2D;
    private bool attackMode;
    private float initTimer;
    private bool isOnAttackAnimation;
    private bool isOnExplotionAnimation;
    private Vector2 targetPosition;
    private GameObject player;
    private bool playerIsGrounded;
    private bool isAlive = true;
    private bool canMove = true;
    #endregion

    void Awake()
    {
        initTimer = timer;
        anim = GetComponent<Animator>();
        enemyBehaviour = GetComponent<Enemy_Behaviour>();
        rigidBody2D = GetComponent<Rigidbody2D>();
        attackMode = false;
        player = GameObject.FindGameObjectWithTag("Player");

    }

    void Update()
    {
        isOnAttackAnimation = anim.GetCurrentAnimatorClipInfo(0)[0].clip.name.ToString().Contains("Attack");
        isOnExplotionAnimation = anim.GetCurrentAnimatorClipInfo(0)[0].clip.name.ToString().Contains("Explode");
        
    }


    void FixedUpdate()
    {
        target = enemyBehaviour.target;
        inRange = enemyBehaviour.inRange;
        playerIsGrounded = player.GetComponent<CharacterController2D>().IsGrounded();

        if (isAlive)
        {
            DetectCollision();
        }

        if (canMove)
        {
            if (enemyBehaviour.currentHealth <= 0 && isAlive)
            {
                Die();
            }

            if (!attackMode)
            {
                Move();
            }
            if (inRange && !attackMode)
            {
                Seek();

                if (timer > 0)
                {
                    Cooldown();
                }

                if (!playerIsGrounded)
                {
                    initTimer = 0.2f;
                }
            }

            if (attackMode)
            {
                Attack();
            }
        }
       

       

    }


    void Move()
    {
        if (!isOnAttackAnimation && !isOnExplotionAnimation)
        {
            targetPosition = new Vector2(target.position.x, target.position.y);
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    void Seek()
    {
        if (!isOnAttackAnimation && !isOnExplotionAnimation)
        {
            targetPosition = new Vector2(target.position.x + 3, target.position.y + 4);
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, (moveSpeed + 3) * Time.deltaTime);
        }
    }

    void Attack()
    {
        if (!isOnAttackAnimation && !isOnExplotionAnimation)
        {
            targetPosition = new Vector2(target.position.x, target.position.y-1f);
        }

        if (!anim.GetCurrentAnimatorClipInfo(0)[0].clip.name.ToString().Contains("Pre"))
        {
            anim.SetTrigger("attackTrigger");
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, (moveSpeed + 10) * Time.deltaTime);
        }

    }

    void Cooldown()
    {
        initTimer -= Time.deltaTime;
        if (initTimer <= 0)
        {
            attackMode = true;
        }
    }

    void Die()
    {
        canMove = false;
        Destroy(rigidBody2D);
        ExplosionDamage();
        anim.SetTrigger("explode");
        CinemachineShake.Instance.ShakeCamera(1.5f, 0.2f);
        isAlive = false;

    }
    void DetectCollision()
    {
        var hit = Physics2D.Raycast(transform.position, Vector2.down, 1f, collisionLayer);
        if (hit.collider != null)
        {
            Die();
        }
    }

    //void ExplodeOnImpact()
    //{
    //    ExplosionDamage();
    //    anim.SetTrigger("explode");
    //}

    void ExplosionDamage()
    {
        var hitbox = enemyBehaviour.hitBox;
        hitbox.transform.localScale = hitbox.transform.localScale * explosionRange;
    }


}
