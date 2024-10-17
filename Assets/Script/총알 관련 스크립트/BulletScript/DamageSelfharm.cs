using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSelfharm : Damage
{
    public void AttackRandomAngle()
    {
        float angle = Random.Range(0, 360);
        myTr.rotation = Quaternion.Euler(0, 0, angle);

        Collider2D[] targets = Physics2D.OverlapBoxAll(myTr.position, new Vector2(data.Size, 1), angle, LayerMask.GetMask("Enemy"));
        if(targets.Length == 0)
        {
            return;
        }
        else
        {
            foreach (var target in targets)
            {
                DamageControler.DealDamage(target.GetComponent<IDamageable>(), data.Damage);
            }
        }
    }
}
