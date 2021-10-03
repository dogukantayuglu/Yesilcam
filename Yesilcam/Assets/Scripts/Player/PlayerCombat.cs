using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour, ICollisionHandler
{
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public int attackDamage = 40;
    public LayerMask enemyLayers;
    public Animator animator;
    public float maxHealth = 100;
    public float damageCdTimer = 1f;
    public bool canTakeDamage = true;
    public float knockBackAmount = 3000f;

    private Rigidbody2D m_Rigidbody2D;
    private PlayerMovement playerMovement;
    private bool attackAnimationIsPlaying;
    private float currentHealth;
    private float initDamageCdTimer;
    private Enemy_Behaviour enemyBehaviour;

    void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        initDamageCdTimer = damageCdTimer;
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //Debug.Log("Player health: "+currentHealth);
        attackAnimationIsPlaying = playerMovement.attackAnimationIsPlaying;
        //Crouch Attack
        if (Input.GetButtonDown("Fire1") && animator.GetBool("IsCrouching") && !attackAnimationIsPlaying && canTakeDamage)
        {
            animator.SetTrigger("CrouchAttack");
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
            foreach (Collider2D enemy in hitEnemies)
            {
                enemy.GetComponent<Enemy_Behaviour>().TakeDamage(attackDamage);
            }
        }

        else
        {
            animator.ResetTrigger("CrouchAttack");
        }

        //Standing Attack
        if (Input.GetButtonDown("Fire1") && !animator.GetBool("IsCrouching") && !attackAnimationIsPlaying && canTakeDamage)
        {
            animator.SetTrigger("StandAttack");
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
            foreach (Collider2D enemy in hitEnemies)
            {
                enemy.GetComponent<Enemy_Behaviour>().TakeDamage(attackDamage);
            }
        }
        else
        {
            animator.ResetTrigger("StandAttack");
        }

        if (!canTakeDamage)
        {
            DamageCooldown();
        }

    }

    public void TakeDamage(float damage)
    {
        

        if (currentHealth > 0 && canTakeDamage)
        {
            currentHealth -= damage;
            animator.SetTrigger("TakeDamage");
            CinemachineShake.Instance.ShakeCamera(3f, 0.5f);
            if (enemyBehaviour != null)
            {
                if (enemyBehaviour.isFacingLeft)
                {
                    m_Rigidbody2D.AddForce(new Vector2(-knockBackAmount, 0f));
                }
                else
                {
                    m_Rigidbody2D.AddForce(new Vector2(knockBackAmount, 0f));

                }
            }
            else
            {
                m_Rigidbody2D.AddForce(new Vector2(-knockBackAmount, 0f));
            }
            
            canTakeDamage = false;
        }

        if (currentHealth <= 0)
        {
        }

    }

    void DamageCooldown()
    {
        damageCdTimer -= Time.deltaTime;
        if (damageCdTimer <= 0 && !canTakeDamage)
        {
            canTakeDamage = true;
            damageCdTimer = initDamageCdTimer;
        }
    }

    public void CollisionEnter(string colliderName, GameObject other)
    {
        enemyBehaviour = other.transform.parent.gameObject.GetComponent<Enemy_Behaviour>();
    }
}
