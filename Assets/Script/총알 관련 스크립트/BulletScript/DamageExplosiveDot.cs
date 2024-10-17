using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DamageExplosiveDot : Damage
{
    private bool isExploding = false;
    float time = 0;

    private void Start()
    {
        wait = new WaitForSeconds(data.Speed);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        isExploding = false;
        time = 0;
    }

    public void DotMech()
    {
        StartCoroutine(Explode());
    }

    private void FixedUpdate()
    {
        if(isExploding)// 폭발 후 3초 후에 비활성화
        {
            time += Time.fixedDeltaTime;
            if(time >= 3f)
            {
                transform.gameObject.SetActive(false);
            }
        }
    }

    IEnumerator Explode()
    {
        isExploding = true;
        float time = 0;
        time += Time.fixedDeltaTime;
        if(time < 3f) // 임시적으로 3초동안 지속
        {
            Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, data.Size, LayerMask.GetMask("Enemy")); // 6은 Enemy 레이어

            foreach (Collider2D target in targets)
            {
                //타겟이 죽으면 브레이크
                if (target.GetComponent<Enemy>().isLive == false || target.gameObject.activeSelf == false)
                    continue;
                DamageControler.DealDamage(target.GetComponent<IDamageable>(), data.Damage);
                yield return wait;
            }
        }
        yield return new WaitForFixedUpdate();
    }
}
