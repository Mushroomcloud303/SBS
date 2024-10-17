using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponObjectBook : WeaponObject
{
    private GameObject[] bullets;
    [SerializeField] private Transform bookObject;
    private int myLevel;

    protected override void Init()
    {
        myLevel = 0;
        bookObject = transform.Find("BookObject");
    }

    protected override void WeaponMechanism()
    {
        //Debug.Log(weaponData.Amount);
        float angle = 360;
        float step = angle / weaponData.Amount;
        float radian = step * Mathf.Deg2Rad; // 각도를 라디안으로 변환
        
        // 현재 총알 초기화
        if(bullets != null)
        {
            foreach (var bullet in bullets)
            {
                bullet.SetActive(false);
            }
        }
        bullets = new GameObject[(int)weaponData.Amount];

        //Debug.Log("책 갯수: " + weaponData.Amount);
        for (int i = 0; i < weaponData.Amount; i++)
        {
            //Debug.Log("i : " + i);
            
            float x = Mathf.Cos(radian * i);
            float y = Mathf.Sin(radian * i);
            Vector3 pos = new Vector3(x, y, 0);
            GameObject bullet = GameManager.instance.poolManager.GetFromPoolBullet(weaponData.bulletPrefabID); // 풀매니저에서 생성
            bullet.GetComponent<Damage>().data = weaponData;
            bullet.transform.parent = bookObject;
            bullet.transform.position = Vector3.zero;
            bullet.transform.localPosition = pos*weaponData.Range;
            bullets[i] = bullet;
        }
    }

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        if (GameManager.instance.weaponLevels[index] != 0)
        {
            bookObject.gameObject.SetActive(true);
            bookObject.Rotate(Vector3.forward * Time.deltaTime * weaponData.Speed);
            if(myLevel != GameManager.instance.weaponLevels[index])
            {
                myLevel = GameManager.instance.weaponLevels[index];
                WeaponMechanism();
            }
        }
        else
        {
            
            bookObject.gameObject.SetActive(false);
        }
        
    }

    public void ResetBullets()
    {
        WeaponMechanism();
    }
}
