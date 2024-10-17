using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOverEat : Damage
{
    private Vector2 summonPoint;
    Vector2 summonPositon;
    public void OverEatMech()
    {
        Collider2D[] targets = Physics2D.OverlapBoxAll(transform.position + (GameManager.instance.playerLeader.left ? new Vector3(-data.Size / 2, -1) : new Vector3(data.Size / 2, -1))
            , new Vector2(data.Size, 2), 0, LayerMask.GetMask("Enemy")); // 6은 Enemy 레이어
        foreach (Collider2D target in targets)
        {
            //타겟이 죽으면 브레이크
            if (target.GetComponent<Enemy>().isLive == false || target.gameObject.activeSelf == false)
                break;
            DamageControler.DealDamage(target.GetComponent<IDamageable>(), data.Damage);
        }
    }

    private void Update()
    {
        summonPoint = GameManager.instance.playerLeader.left ? new Vector2(-1, 2) : new Vector2(1, 2);
        transform.localPosition = (Vector3)summonPoint;
        mySr.flipX = GameManager.instance.playerLeader.left;
    }

    //void Update()
    //{
    //    //Debug.Log("Update");
    //    //// summonPosition 계산
    //    //Vector3 summonPosition = transform.position + (GameManager.instance.playerLeader.left ? new Vector3(-data.size / 2, 0) : new Vector3(data.size / 2, 0));
    //    //Debug.Log(summonPosition);
    //    //// OverlapBox의 크기
    //    //Vector2 boxSize = new Vector2(data.size, 2);

    //    //// 박스의 네 모서리 위치 계산
    //    //Vector3 boxTopLeft = summonPosition + new Vector3(-boxSize.x / 2, boxSize.y / 2, 0);
    //    //Vector3 boxTopRight = summonPosition + new Vector3(boxSize.x / 2, boxSize.y / 2, 0);
    //    //Vector3 boxBottomLeft = summonPosition + new Vector3(-boxSize.x / 2, -boxSize.y / 2, 0);
    //    //Vector3 boxBottomRight = summonPosition + new Vector3(boxSize.x / 2, -boxSize.y / 2, 0);

    //    //// 박스의 모서리를 연결하는 선들을 그립니다.
    //    //Debug.DrawLine(boxTopLeft, boxTopRight, Color.red);
    //    //Debug.DrawLine(boxTopRight, boxBottomRight, Color.red);
    //    //Debug.DrawLine(boxBottomRight, boxBottomLeft, Color.red);
    //    //Debug.DrawLine(boxBottomLeft, boxTopLeft, Color.red);
    //    //Debug.Log("DrawLine");

    //    //// 오버랩 박스를 시각화하고 싶다면 이 아래에 OverlapBoxAll 호출 추가
    //    //Collider2D[] targets = Physics2D.OverlapBoxAll(summonPosition, boxSize, 0, LayerMask.GetMask("Enemy"));
    //    //Debug.Log(targets.Length);
    //    //foreach (Collider2D target in targets)
    //    //{
    //    //    Debug.Log(target.name);
    //    //}

    //    // StartCoroutine의 호출은 이 Update 메서드 안에서 하거나
    //    // 적절한 위치에서 호출해야 합니다. 예를 들어:
    //    // StartCoroutine(OverEat(targets));
    //}
}
