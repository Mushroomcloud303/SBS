using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponObjectBeer : WeaponObject
{
    private Vector3 start;
    private Vector3 mid;
    private Vector3 end;
    private bool left = false;
    [SerializeField] float height = 2f;

    protected override void Init()
    {
        delay = new WaitForSeconds(weaponData.SummonDelay);
    }

    protected override void WeaponMechanism()
    {
        end = GameManager.instance.playerLeader.transform.position + GetRandomPosition(weaponData.Range);
        if(end == Vector3.zero)
        {
            return;
        }
        GameObject bullet = GameManager.instance.poolManager.GetFromPoolBullet(weaponData.bulletPrefabID); // 풀매니저에서 생성
        bullet.GetComponent<Damage>().data = weaponData;
        bullet.transform.position = GameManager.instance.playerLeader.transform.position;
        // 궤도의 정규화를 위한 부모 설정
        start = bullet.transform.position;
        left = end.x < start.x ? true : false;
        mid = new Vector3((start.x + end.x) / 2, end.y + height, 0);
        bullet.GetComponent<SpriteRenderer>().sortingOrder = Random.Range(-19, 0);
        Tween rotationTween = bullet.transform.DORotate(new Vector3(0, 0, left ? 360 : -360), weaponData.Speed/2, RotateMode.FastBeyond360).SetLoops(2, LoopType.Restart);
        Tween pathTween = bullet.transform.DOPath(new Vector3[] { start, mid, end }, weaponData.Speed, PathType.CatmullRom, PathMode.TopDown2D, 10, Color.red).SetEase(Ease.OutSine)
            .OnComplete(() => bullet.GetComponent<Animator>().SetTrigger("Explode"))// 위치로 이동하면 비활성화를 뜻하는 람다식
            .OnKill(() => {
                             bullet.GetComponent<Animator>().SetTrigger("Explode");
                             bullet.transform.rotation = Quaternion.identity;
                           }
                    );//중간에 종료되면 비활성화를 뜻하는 람다식
        bullet.GetComponent<CallKillTween>().pathTween = pathTween;
        bullet.GetComponent<CallKillTween>().rotationTween = rotationTween;
    }

    private void Awake()
    {
        Init();
    }

    private void FixedUpdate()
    {
        CallPerFrame();
    }


}
