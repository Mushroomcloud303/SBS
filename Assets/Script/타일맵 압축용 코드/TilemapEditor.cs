using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(Tilemap))]
public class TilemapEditor : Editor
{
    // 인스펙터에 표시할 내용을 재정의 타일맵 크기 압축 버튼을 추가
    public override void OnInspectorGUI()
    {
        // 기본 타일맵 인스펙터를 표시
        base.OnInspectorGUI();

        // 타겟이 되는 타일맵 오브젝트
        Tilemap tilemap = (Tilemap)target;

        // 버튼을 추가하여 Tilemap.CompressBounds()를 실행하도록 함
        if (GUILayout.Button("Compress Bounds"))
        {
            // Tilemap.CompressBounds() 호출
            tilemap.CompressBounds();
            Debug.Log("Tilemap bounds compressed.");
        }
    }
}
