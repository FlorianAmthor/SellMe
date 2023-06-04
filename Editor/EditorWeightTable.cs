using System;
using UnityEditor;
using UnityEngine;

public static class EditorWeightTable
{
    public static void Show<T>(SerializedProperty list) where T : Enum
    {
        for (int i = 0; i < list.arraySize; i++)
        {
            SerializedProperty sProb = list.GetArrayElementAtIndex(i);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField(Enum.GetName(typeof(T), sProb.FindPropertyRelative("data").intValue)+":", new GUIStyle() { alignment = TextAnchor.MiddleLeft, fontSize = 12, fontStyle = FontStyle.Bold, fixedWidth = Screen.width /2, stretchWidth = false });
            EditorGUILayout.PropertyField(sProb.FindPropertyRelative("weight"), label: GUIContent.none, GUILayout.Width(Screen.width / 2));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }
    }
}