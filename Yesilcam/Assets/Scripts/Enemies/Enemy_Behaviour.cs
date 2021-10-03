using System;
using UnityEngine;
using UnityEngine.Rendering;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Enemy_Behaviour : MonoBehaviour,ICollisionHandler
{
    #region Public Variables

    public Transform leftLimit;
    public Transform rightLimit;
    [HideInInspector] public Transform target;
    [HideInInspector] public bool inRange;
    public GameObject hotZone;
    public GameObject triggerArea;
    public GameObject hitBox;
    public int attackDamage = 10;
    public int maxHealth = 100;
    [HideInInspector] public bool isFacingLeft;
    [HideInInspector] public int currentHealth;



    #endregion

    #region Private Variables
    private Animator anim;
    private bool isOnAttackAnimation;

    #endregion

    void Awake()
    {
        SelectTarget();
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        isOnAttackAnimation = anim.GetCurrentAnimatorClipInfo(0)[0].clip.name.ToString().Contains("Attack");
        CharacterDirection();
    }


    void FixedUpdate()
    {
        if (!InsideofLimits() && !inRange && !isOnAttackAnimation)
        {
            SelectTarget();
        }
    }

   
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        //Hurt animation

    }

   

    private bool InsideofLimits()
    {
        return transform.position.x > leftLimit.position.x && transform.position.x < rightLimit.position.x;
    }

    public void SelectTarget()
    {
        float distanceToLeft = Vector2.Distance(transform.position, leftLimit.position);
        float distanceToRight = Vector2.Distance(transform.position, rightLimit.position);

        if (distanceToLeft > distanceToRight)
        {
            target = leftLimit;
        }
        else
        {
            target = rightLimit;
        }

        Flip();
    }

    public void Flip()
    {
        Vector3 rotation = transform.eulerAngles;
        if (transform.position.x > target.position.x)
        {
            rotation.y = 180f;
        }
        else
        {
            rotation.y = 0f;
        }

        transform.eulerAngles = rotation;
    }

    void CharacterDirection()
    {
        
        if (target.transform.position.x - transform.position.x < 0 && !isOnAttackAnimation)
        {
            isFacingLeft = true;
        }

        else if (target.transform.position.x - transform.position.x > 0 && !isOnAttackAnimation)
        {
            isFacingLeft = false;
        }

    }
    
    public void CollisionEnter(string colliderName, GameObject other)
    {
        if (colliderName == "hitBox" && other.tag == "Player")
        {
            other.GetComponent<PlayerCombat>().TakeDamage(attackDamage);
        }
    }
    void TriggerDestroyObjectOnDeath()
    {
        Destroy(gameObject);
    }
}
