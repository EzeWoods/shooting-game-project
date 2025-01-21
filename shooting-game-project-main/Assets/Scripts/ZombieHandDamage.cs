using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ZombieHanddamage : MonoBehaviour
{
    [SerializeField] int damageAmount;
    [SerializeField] float destroyTime;

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

            IDamage dmg = other.GetComponent<IDamage>();
            if (dmg != null)
            {
                dmg.takeDamage(damageAmount);
            }

            Destroy(gameObject, destroyTime);
        }
    }


  
