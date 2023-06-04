using UnityEditor;
using UnityEngine;

public static class EditorProbabilityTable
{
    public static void Show<T>(SerializedProperty list)
    {
        EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.TextField("Rarity", new GUIStyle() { alignment = TextAnchor.MiddleCenter, fontSize = 12, fontStyle = FontStyle.Bold, fixedWidth = Screen.width / 4 , stretchWidth = false});
        EditorGUILayout.TextField("Value", new GUIStyle() { alignment = TextAnchor.MiddleCenter, fontSize = 12, fontStyle = FontStyle.Bold, fixedWidth = Screen.width / 4, stretchWidth = false });
        EditorGUILayout.TextField("Probability", new GUIStyle() { alignment = TextAnchor.MiddleCenter, fontSize = 12, fontStyle = FontStyle.Bold, fixedWidth = Screen.width / 4, stretchWidth = false , contentOffset = new Vector2(15,0)});
        EditorGUILayout.TextField("", new GUIStyle() { alignment = TextAnchor.MiddleCenter, fontSize = 12, fontStyle = FontStyle.Bold, fixedWidth = Screen.width / 4, stretchWidth = false });
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        float sum = 0;


        for (int i = 0; i < list.arraySize; i++)
        {
            SerializedProperty sProb = list.GetArrayElementAtIndex(i);
            sum += sProb.FindPropertyRelative("probability").floatValue;
        }

        for (int i = 0; i < list.arraySize; i++)
        {
            SerializedProperty sProb = list.GetArrayElementAtIndex(i);
            float prob;
            if (sum != 0)
                prob = sProb.FindPropertyRelative("probability").floatValue / sum;
            else
                prob = 0;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(sProb.FindPropertyRelative("data"), label: GUIContent.none, GUILayout.Width(Screen.width / 4));
            EditorGUILayout.PropertyField(sProb.FindPropertyRelative("probability"), label: GUIContent.none, GUILayout.Width(Screen.width / 4));
            EditorGUILayout.LabelField(prob.ToString(), new GUIStyle() { alignment = TextAnchor.MiddleCenter }, GUILayout.Width(Screen.width / 4));
            if (GUILayout.Button("-"))
            {
                list.DeleteArrayElementAtIndex(i);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }
        EditorGUILayout.Space();

        if (GUILayout.Button("Add Probability"))
        {
            list.InsertArrayElementAtIndex(list.arraySize);
        }
    }
}
