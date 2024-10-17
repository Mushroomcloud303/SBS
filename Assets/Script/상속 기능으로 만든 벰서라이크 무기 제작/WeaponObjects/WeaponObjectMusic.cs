using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponObjectMusic : WeaponObject
{

    protected override void Init()
    {

    }

    protected override void WeaponMechanism()
    {
        Vector2[] targets = CalculateCirclePositions((int)weaponData.Amount, 360, transform.position);
        foreach (var target in targets)
        {
            ShotToTargets(target);
        }
    }

    private void Awake()
    {
        Init();
    }

    private void FixedUpdate() // 한방에 소환해야되서 추가로 구현함
    {
        CallPerFrameNoDelay();
    }

    private Vector2[] CalculateCirclePositions(int n, float radius, Vector2 center)
    {
        Vector2[] positions = new Vector2[n];
        float angleStep = 360f / n; // 원을 n으로 나눈 각도
        float radian = angleStep * Mathf.Deg2Rad; // 각도를 라디안으로 변환
        for (int i = 0; i < n; i++)
        {
            // 원형의 위치 계산
            float x = center.x + radius * Mathf.Cos(radian*i);
            float y = center.y + radius * Mathf.Sin(radian*i);
            positions[i] = new Vector2(x, y);
        }

        return positions;
    }
}
