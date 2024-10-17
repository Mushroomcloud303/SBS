using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDotStay : Damage
{
    private float time = 0;

    private void FixedUpdate()
    {
        time += Time.deltaTime;
        if (time >= data.Rapid)
        {
            Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, data.Size, LayerMask.GetMask("Enemy"));
            foreach (var target in targets)
            {
                IDamageable damageable = target.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    DamageControler.DealDamage(damageable, data.Damage);
                }
            }
            time = 0;
        }
        
    }
}
