using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageExplosive : Damage
{
    
    public void Explode()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, data.Size, LayerMask.GetMask("Enemy")); // 6은 Enemy 레이어

        foreach (Collider2D target in targets)
        {
            Debug.Log(target);
            DamageControler.DealDamage(target.GetComponent<IDamageable>(), data.Damage);
        }
    }

    //public void Disable()
    //{
    //    transform.parent.gameObject.SetActive(false);
    //}
}
