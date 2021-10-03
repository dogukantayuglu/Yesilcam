using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotZoneCheck : MonoBehaviour
{
    private Enemy_Behaviour enemyParent;
    private bool inRange;
    private Animator anim;
    private bool isOnAttackAnimation;

    private void Awake()
    {
        enemyParent = GetComponentInParent<Enemy_Behaviour>();
        anim = GetComponentInParent<Animator>();
        isOnAttackAnimation = anim.GetCurrentAnimatorClipInfo(0)[0].clip.name.ToString().Contains("Attack");
    }

    private void Update()
    {
        if (inRange && !isOnAttackAnimation)
        {
            enemyParent.Flip();
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            inRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            inRange = false;
            gameObject.SetActive(false);
            enemyParent.triggerArea.SetActive(true);
            enemyParent.inRange = false;
            enemyParent.SelectTarget();
        }
    }
}
