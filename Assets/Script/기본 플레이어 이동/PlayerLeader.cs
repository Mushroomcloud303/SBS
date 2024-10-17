using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLeader : MonoBehaviour
{
    public Vector2 inputVector;
    public float moveSpeed = 5f;
    public bool left = false;

    private Rigidbody2D myRigid;
    private SpriteRenderer mySprite;
    public Animator myAnimator;

    private void Awake()
    {
        myRigid = GetComponent<Rigidbody2D>();
        mySprite = GetComponent<SpriteRenderer>();
        myAnimator = GetComponent<Animator>();
        //myAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    private void FixedUpdate()
    {
        if (!GameManager.instance.isLive)
            return;
        Vector2 nextPosition = inputVector*moveSpeed;
        myRigid.MovePosition(myRigid.position + nextPosition * Time.fixedDeltaTime);

        if (GameManager.instance.presentHp <= 0)
        {
            for (int i = 2; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }

            myAnimator.SetTrigger("playerDead");
            GameManager.instance.GameOver();
        }
    }

    private void LateUpdate()
    {
        if (!GameManager.instance.isLive)
            return;
        myAnimator.SetFloat("playerSpeed", inputVector.magnitude); // inputVector의 크기를 Speed에 전달, 그에 따라 애니메이션 출력 변경

        //좌우 반전 적용
        if(inputVector.x != 0)
        {
            mySprite.flipX = inputVector.x < 0; // inputVector.x가 0보다 작으면 true, 아니면 false
            left = inputVector.x < 0;
        }
    }

    // PlayerLeader 오브젝트의 Player Input 에 입력값을 inputVector에 저장
    private void OnMove(InputValue cPlayerInputValue)
    {
        inputVector = cPlayerInputValue.Get<Vector2>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 9) // 닿은 게 다음 층 입구일 경우
        {
            if(GameManager.instance.floor < GameManager.instance.maxfloor-1)
            {
                GameManager.instance.floor += 1;
                //GameManager.instance.NextLevel();
            }
            else
            {
                GameManager.instance.GameVictory();
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(!GameManager.instance.isLive)
            return;

        if (collision.gameObject.layer == 6) // 닿은 게 적 레이어일 경우
            GameManager.instance.presentHp -= Time.deltaTime*collision.gameObject.GetComponent<Enemy>().enemyDamage;        
    }

    public void Dead()
    {
        Debug.Log("플레이어 사망");
        GameManager.instance.isLive = false;
        myAnimator.SetTrigger("Dead");
    }

    public void AfterDeadAni()
    {
        GameManager.instance.Result();
    }
}

