using SuspiciousGames.SellMe.Core.Generators;
using SuspiciousGames.SellMe.Core.Items;
using System;
using UnityEditor;

public abstract class TableEditor<K> : Editor where K : Enum
{
    protected SerializedObject sObj;
    protected SerializedProperty probabilities;
    protected ProbabilityTable<K> probabilityTable;

    private void OnEnable()
    {
        sObj = new SerializedObject(target);
        probabilityTable = target as ProbabilityTable<K>;
        probabilities = sObj.FindProperty("_probabilities");
    }

    public override void OnInspectorGUI()
    {
        EditorProbabilityTable.Show<K>(probabilities);
        EditorGUILayout.Space();
        sObj.ApplyModifiedProperties();
        Repaint();
    }
}

[CustomEditor(typeof(CategoryTable))]
public class CategoryTableEditor : TableEditor<Category> { }

[CustomEditor(typeof(RarityTable))]
public class RarityTableEditor : TableEditor<Rarity> { }

[CustomEditor(typeof(TraitTable))]
public class TraitTableEditor : TableEditor<Trait> { }
