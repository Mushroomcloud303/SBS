using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Enemy Info")]
    public MonsterData monsterData;
    public float enemySpeed; // 적 이속
    float slowedSpeed; // 엘몹은 넉백 대신 잠깐 이속 줄어드는 걸로 하기 위한 변수
    public float enemyHealth; // 적 현재 체력
    public float enemyMaxHealth; // 적 최대 체력
    public int nightOnly; // 이 정수가 1이면 낮에는 소멸합니다. 0일 경우 반대로 낮에 소멸합니다. 그 외의 값을 주면 낮이든 밤이든 소멸하지 않습니다.
    public float enemyDamage; // 적 공격력
    public bool isElite; // 엘리트 몹인지 판정합니다.
    [Header("Enemy Control")]
    public Rigidbody2D targetRigid;
    public RuntimeAnimatorController[] enemyAnimators;
    //코드 내에서 사용할 변수
    public bool isLive = false;
    private Rigidbody2D myRigid;
    private SpriteRenderer mySprite;
    private Animator myAnimator;
    private WaitForFixedUpdate wait;
    private Collider2D myCollider;
    public float rayDistance = 1f;  // 레이캐스트 길이
    public LayerMask groundLayer;   // 타일맵 레이어 설정

    NavMeshAgent agent;
    public bool PF;
    [SerializeField]private bool tooFar;

    private void Awake()
    {
        myRigid = GetComponent<Rigidbody2D>();
        mySprite = GetComponent<SpriteRenderer>();
        myAnimator = GetComponent<Animator>();
        wait = new WaitForFixedUpdate();
        myCollider = GetComponent<Collider2D>();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        
    }
    private void OnEnable()
    {
        agent.enabled = false;
        targetRigid = GameManager.instance.playerLeader.GetComponent<Rigidbody2D>(); // 활성화 될때 타겟 설정
        myCollider.enabled = true;
        isLive = true;
        myRigid.simulated = true;
        mySprite.sortingOrder = 2;
        myAnimator.SetBool("Dead",false);
        if (PF)
            agent.enabled = true;
        enemyHealth = enemyMaxHealth;     
    }

    public void SetPosition(Transform targetPo)
    {
        agent.Warp(targetPo.position);
        agent.enabled = true;
    }
    //생성 초기 설정
    public void Init(SpawnData data)
    {
        myAnimator.runtimeAnimatorController = enemyAnimators[data.spriteType];
        enemySpeed = data.speed;
        slowedSpeed = data.speed / 2;
        enemyMaxHealth = data.health;
        nightOnly = data.nightOnly;
        enemyDamage = data.damage;
        isElite = data.isElite;
        enemyHealth = enemyMaxHealth;

        if (isElite)
            mySprite.color = Color.red;
        else
            mySprite.color = Color.white;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * rayDistance);
    }

    private void FixedUpdate()
    {
        agent.isStopped = !GameManager.instance.isLive;
        if (!GameManager.instance.isLive) // 게임이 멈췄으면
            return; // 이하 내용 실행 안함
        Debug.Log("게임 실행중 아님");
        if (!isLive) // 살아있지 않다면
            return; // 이하 내용 실행 안함    

        if (myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
        {
            if (!isElite) // 엘몹이 아니면
                return; // 맞는 동안은 움직이지 않음
            else // 엘몹이면
            {
                enemySpeed = slowedSpeed;
            }
        }
        else
        {
            enemySpeed = slowedSpeed * 2;
        }

        //타겟의 위치로 이동
        agent.SetDestination(targetRigid.gameObject.transform.position);
        if (agent.pathStatus == NavMeshPathStatus.PathPartial || agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            // 경로가 유효하지 않다면 몬스터를 비활성화
            Erase();
        }
        agent.speed = enemySpeed;
        myRigid.velocity = Vector2.zero; // 이동 속도 초기화로 관성 방지
    }

    private void LateUpdate()
    {
        if (!GameManager.instance.isLive) // 게임이 멈췄으면
            return; // 이하 내용 실행 안함

        if (!isLive) // 살아있지 않다면?
            return; // 아래 내용은 실행되지 않는다.

        mySprite.flipX = targetRigid.position.x < myRigid.position.x; // 타겟의 위치에 따라 좌우 반전
    }

    public void TakeDamage(float damage)
    {
        enemyHealth -= damage;
        StartCoroutine(KnockBack());
        if (enemyHealth > 0)
        {
            myAnimator.SetTrigger("Hit");
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit); // 피격 효과음 재생
        }
        else
        {
            isLive = false;
            myCollider.enabled = false;
            myRigid.simulated = false;
            mySprite.sortingOrder = 1;
            myAnimator.SetBool("Dead", true);
            agent.enabled = false;
            GameManager.instance.killCount++;
            StartCoroutine(GameManager.instance.GetExp());

            if (GameManager.instance.isLive) // 게임이 끝나지 않았다면? (마지막에 다 죽을 때 귀테러 방지용)
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead); // 사망 효과음 재생
        }
    }

    IEnumerator KnockBack()
    {
        yield return wait; // 한 프레임 대기
        Vector3 playerPosition = GameManager.instance.playerLeader.transform.position;
        Vector3 direction = (transform.position - playerPosition).normalized;
        myRigid.AddForce(direction * 5, ForceMode2D.Impulse);
    }

    void Erase() // 몹이 죽는 연출없이 사라지게 하고 싶을 때 씁니다.
    {
        isLive = false;
        myCollider.enabled = false;
        myRigid.simulated = false;
        agent.enabled = false;
        Dead();
    }

    void Dead()
    {
        gameObject.SetActive(false);
        agent.enabled = false;
    }
}
