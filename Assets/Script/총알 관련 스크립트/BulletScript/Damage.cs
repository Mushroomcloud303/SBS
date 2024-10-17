using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IDamageable
{
    void TakeDamage(float damage);
}// 데미지를 받을 수 있는 인터페이스 이걸 원하는 클래스에 상속 받고 TakeDamage를 구현하면 됨
public class DamageControler
{
    public static void DealDamage(IDamageable target, float damage)
    {
        target.TakeDamage(damage);
    }
}// 데미지 주고 싶으면 호출하세영
public abstract class Damage : MonoBehaviour
{
    [SerializeField]protected int pierce;
    public WeaponData data; // 기왕이면 프리팹에 설정 해주자 onEnable에서 처음 실행할때 널오류뜸 일단 오브젝트에서 건내주기는 함 처음만 견디면 될 듯
    protected Collider2D myCo;
    protected SpriteRenderer mySr;
    protected Transform myTr;
    protected WaitForSeconds wait;

    protected void Awake()
    {
        myCo = GetComponent<Collider2D>();
        mySr = GetComponentInChildren<SpriteRenderer>(); // 굳이 자식에게 스프라이트 넣어서 이렇게 함
        myTr = GetComponent<Transform>();
        wait = new WaitForSeconds(data.Rapid);
    }

    protected virtual void OnEnable()
    {
        pierce = (int)data.Piercing;
        myCo.enabled = true;    
        mySr.enabled = true;
    }

    protected void TakeDamage(IDamageable target, WeaponData data)// 아마 콜라이더가 인터페이스 없는 일은 없을검 ㅇㅇ
    {
        DamageControler.DealDamage(target, data.Damage);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}

