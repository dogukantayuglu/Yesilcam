using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HamraBehaviour : MonoBehaviour
{
    #region Public Variables

    public float attackDistance;
    public float moveSpeed;
    public float timer;
    public LayerMask collisionLayer;
    [HideInInspector] public bool isFacingLeft;
    [HideInInspector] public bool inRange;



    #endregion

    #region Private Variables
    private Animator anim;
    private Enemy_Behaviour enemyBehaviour;
    private Transform target;
    private Rigidbody2D rigidBody2D;
    private float distance;
    private bool attackMode;
    private bool cooling;
    private float intTimer;
    private int currentHealth;
    private Vector3 previousPosition;
    private Vector3 currentPosition;
    private Vector3 direction;
    private bool isOnAttackAnimation;
    private bool canMove;
    private bool isAlive;

    #endregion

    void Awake()
    {
        intTimer = timer;
        anim = GetComponent<Animator>();
        enemyBehaviour = GetComponent<Enemy_Behaviour>();
        rigidBody2D = GetComponent<Rigidbody2D>();

        currentPosition = this.gameObject.transform.position;
        previousPosition = currentPosition;
        isAlive = true;
    }

    void Update()
    {
        isOnAttackAnimation = anim.GetCurrentAnimatorClipInfo(0)[0].clip.name.ToString().Contains("Attack");
        Debug.Log(target);
    }


    void FixedUpdate()
    {
        target = enemyBehaviour.target;
        inRange = enemyBehaviour.inRange;
        currentHealth = enemyBehaviour.currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }

        if (isAlive)
        {
            DetectCollision();
            CharacterDirection();
            ManageMoveAnimation();

            if (!attackMode && canMove)
            {
                Move();
            }

            if (inRange)
            {
                EnemyLogic();
            }
        }

    }

    void EnemyLogic()
    {
        distance = Vector2.Distance(transform.position, target.position);
        if (distance > attackDistance && cooling == false && canMove)
        {
            Move();
            StopAttack();
        }
        else if (attackDistance >= distance && cooling == false)
        {
            Attack();
        }

        if (cooling)
        {
            Cooldown();
            anim.SetBool("Attack", false);
        }
    }

    void Move()
    {
        anim.SetBool("canWalk", true);

        if (!isOnAttackAnimation)
        {
            Vector2 targetPosition = new Vector2(target.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    void Attack()
    {
        timer = intTimer;
        attackMode = true;

        anim.SetBool("Attack", true);
        anim.SetBool("canWalk", false);
    }

    void Die()
    {
        Destroy(rigidBody2D);
        isAlive = false;

        cooling = false;
        anim.SetBool("Attack", false);
        anim.SetBool("canWalk", false);
        anim.SetTrigger("hasDied");

        CinemachineShake.Instance.ShakeCamera(1.5f, 0.2f);


    }

    void Cooldown()
    {
        timer -= Time.deltaTime;
        if (timer <= 0 && cooling)
        {
            cooling = false;
            timer = intTimer;
        }
    }

    void StopAttack()
    {
        cooling = false;
        attackMode = false;
        anim.SetBool("Attack", false);
    }

    public void TriggerCooling()
    {
        cooling = true;
    }
    

    void CharacterDirection()
    {
        isOnAttackAnimation = anim.GetCurrentAnimatorStateInfo(0).IsName("Hamra_Attack");
        if (target.transform.position.x - transform.position.x < 0 && !isOnAttackAnimation)
        {
            isFacingLeft = true;
        }

        else if (target.transform.position.x - transform.position.x > 0 && !isOnAttackAnimation)
        {
            isFacingLeft = false;
        }

    }

    void ManageMoveAnimation()
    {
        //If character stops stops move animations
        currentPosition = this.gameObject.transform.position;

        if (Math.Abs((currentPosition.x - previousPosition.x)) < 0.02)
        {
            anim.SetBool("notMoving", true);
        }
        else
        {
            anim.SetBool("notMoving", false);
        }
        previousPosition = currentPosition;
    }

    void DetectCollision()
    {
        var hit = Physics2D.Raycast(transform.position, isFacingLeft ? Vector2.left : Vector2.right, 0.5f, collisionLayer);
        if (hit.collider != null)
        {
            canMove = false;
        }

        else
        {
            canMove = true;
        }


    }
}
