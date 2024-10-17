using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTreeUDL : SceneTree
{
    protected override IEnumerator SetLeafSequence(Vector2 direction, SceneTree rootTree)
    {
        yield return null;
        //Debug.Log("SetLeafSequence 시작");
        //Debug.Log("왼쪽 트리 위치 : " + leftTree.currentPosition);
        //Debug.Log("오른쪽 트리 위치 : " + rightTree.currentPosition);
        //Debug.Log("위쪽 트리 위치 : " + upTree.currentPosition);
        //Debug.Log("아래쪽 트리 위치 : " + downTree.currentPosition);

        switch (direction)
        {
            case Vector2 left when direction == Vector2.left:
                //Debug.Log("왼쪽으로 감");
                yield return StartCoroutine(LoadAndPositionSceneUsingDir(Vector2.left, Vector2.left * 38, this, (sceneTree) => this.leftTree = sceneTree));
                //Debug.Log("왼쪽 트리 설치");
                //this.rightTree = rootTree;
                //Debug.Log("오른쪽 트리 설치");
                yield return StartCoroutine(LoadAndPositionSceneUsingDir(Vector2.up, Vector2.up * 36, this, (sceneTree) => this.upTree = sceneTree));
                //Debug.Log("위쪽 트리 설치");
                yield return StartCoroutine(LoadAndPositionSceneUsingDir(Vector2.down, Vector2.down * 36, this, (sceneTree) => this.downTree = sceneTree));
                //Debug.Log("아래쪽 트리 설치");
                break;

            case Vector2 right when direction == Vector2.right:
                //Debug.Log("오른쪽으로 감");
                this.leftTree = rootTree;
                //Debug.Log("왼쪽 트리 설치");
                //yield return StartCoroutine(LoadAndPositionScene(Vector2.right * 38, this, (sceneTree) => this.rightTree = sceneTree));
                //Debug.Log("오른쪽 트리 설치");
                yield return StartCoroutine(LoadAndPositionSceneUsingDir(Vector2.up, Vector2.up * 36, this, (sceneTree) => this.upTree = sceneTree));
                //Debug.Log("위쪽 트리 설치");
                yield return StartCoroutine(LoadAndPositionSceneUsingDir(Vector2.down, Vector2.down * 36, this, (sceneTree) => this.downTree = sceneTree));
                //Debug.Log("아래쪽 트리 설치");
                break;

            case Vector2 up when direction == Vector2.up:
                //Debug.Log("위로 감");
                yield return StartCoroutine(LoadAndPositionSceneUsingDir(Vector2.left, Vector2.left * 38, this, (sceneTree) => this.leftTree = sceneTree));
                //Debug.Log("왼쪽 트리 설치");
                //yield return StartCoroutine(LoadAndPositionScene(Vector2.right * 38, this, (sceneTree) => this.rightTree = sceneTree));
                //Debug.Log("오른쪽 트리 설치");
                yield return StartCoroutine(LoadAndPositionSceneUsingDir(Vector2.up, Vector2.up * 36, this, (sceneTree) => this.upTree = sceneTree));
                //Debug.Log("위쪽 트리 설치");
                this.downTree = rootTree;
                //Debug.Log("아래쪽 트리 설치");
                break;

            case Vector2 down when direction == Vector2.down:
                //Debug.Log("아래로 감");
                yield return StartCoroutine(LoadAndPositionSceneUsingDir(Vector2.left, Vector2.left * 38, this, (sceneTree) => this.leftTree = sceneTree));
                //Debug.Log("왼쪽 트리 설치");
                //yield return StartCoroutine(LoadAndPositionScene(Vector2.right * 38, this, (sceneTree) => this.rightTree = sceneTree));
                //Debug.Log("오른쪽 트리 설치");
                this.upTree = rootTree;
                //Debug.Log("위쪽 트리 설치");
                yield return StartCoroutine(LoadAndPositionSceneUsingDir(Vector2.down, Vector2.down * 36, this, (sceneTree) => this.downTree = sceneTree));
                //Debug.Log("아래쪽 트리 설치");
                break;

            default:
                // 방향이 올바르지 않은 경우 처리할 수 있습니다.
                break;
        }
    }
}
