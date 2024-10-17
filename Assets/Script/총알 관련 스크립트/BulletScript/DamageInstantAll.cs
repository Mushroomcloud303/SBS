using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageInstantAll : Damage
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
   
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, data.Size, LayerMask.GetMask("Enemy")); // 6은 Enemy 레이어
        
        foreach (Collider2D target in targets)
        {
            Debug.Log(target);
            DamageControler.DealDamage(target.GetComponent<IDamageable>(), data.Damage);
        }
    }
}

