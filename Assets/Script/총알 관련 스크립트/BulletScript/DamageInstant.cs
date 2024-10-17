using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageInstant : Damage
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Enemy")
            return;
        else
        {
            DamageControler.DealDamage(collision.GetComponent<IDamageable>(), data.Damage);
            if (pierce < 0) // 무한 관통
            {
                return;
            }
            else if (pierce == 1) // 관통 끝
            {
                mySr.enabled = false;
                myCo.enabled = false;
                return;
            }
            else // 관통
            {
                pierce--;
            }
        }      
    }

    private void OneTarget()
    {

    }
}      