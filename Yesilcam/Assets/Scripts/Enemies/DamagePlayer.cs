using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    public PlayerCombat playerCombat;

    private GameObject player;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerCombat = player.GetComponent<PlayerCombat>();
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            playerCombat.TakeDamage(10);
        }
    }
}
