using SuspiciousGames.SellMe.Core.Customer;
using SuspiciousGames.SellMe.Core.Items;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CustomerTrait))]
public class CustomerTraitEditor : Editor
{
    protected SerializedObject sObj;
    protected SerializedProperty rarities;
    protected SerializedProperty categories;
    protected SerializedProperty traits;
    protected SerializedProperty name;

    public bool showRarity = true;
    public bool showCategory = true;
    public bool showTrait = true;

    private void OnEnable()
    {
        sObj = new SerializedObject(target);
        rarities = sObj.FindProperty("rarityInterests");
        categories = sObj.FindProperty("categoryInterests");
        traits = sObj.FindProperty("traitInterests");
        name = sObj.FindProperty("traitName");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(name);
        EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();
        showRarity = EditorGUILayout.Foldout(showRarity, "Rarity");
        if (showRarity)
        {
            EditorWeightTable.Show<Rarity>(rarities);
        }
        EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();
        showCategory = EditorGUILayout.Foldout(showCategory, "Category");
        if (showCategory)
        {
            EditorWeightTable.Show<Category>(categories);
        }
        EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();
        showTrait = EditorGUILayout.Foldout(showTrait, "Trait");
        if (showTrait)
        {
            EditorWeightTable.Show<Trait>(traits);
        }

        EditorGUILayout.Space();
        sObj.ApplyModifiedProperties();
        Repaint();
    }
}
