using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponObjectExercise : WeaponObject
{
    private Vector3 start;
    private Vector3 mid;
    private Vector3 end;
    private bool left = false;
    [SerializeField] private float height = 2f;

    protected override void Init()
    {
        delay = new WaitForSeconds(weaponData.SummonDelay);
    }

    protected override void WeaponMechanism()
    {
        GameObject bullet = GameManager.instance.poolManager.GetFromPoolBullet(weaponData.bulletPrefabID); // 풀매니저에서 생성
        bullet.GetComponent<Damage>().data = weaponData;
        bullet.transform.parent = gameObject.transform;
        bullet.transform.position = Vector3.zero;
        bullet.transform.localPosition = Vector3.up * 2; //머리에서 나오는거 같게 위에다 생성
        // 궤도의 정규화를 위한 부모 설정
        start = Vector3.up * 2;
        end = RandomPosition();
        mid = new Vector3(end.x / 2, height+2f, 0);
        bullet.transform.DORotate(new Vector3(0, 0, left ? 360 : -360), weaponData.Rapid, RotateMode.FastBeyond360);
        bullet.transform.DOLocalPath(new Vector3[] { start, mid, end }, weaponData.Rapid, PathType.CatmullRom, PathMode.TopDown2D, 10, Color.red).SetEase(Ease.InOutSine)
            .OnComplete(() => bullet.SetActive(false)); // 위치로 이동하면 비활성화를 뜻하는 람다식
    }

    private void Awake()
    {
        Init();
    }

    private void FixedUpdate()
    {
        CallPerFrame();
    }

    private Vector3 RandomPosition()
    {
        left = Random.Range(0, 2) == 0;
        return new Vector3( left ? Random.Range(-5f, -2f) : Random.Range(2f, 5f), -12 , 0);
    }

}
