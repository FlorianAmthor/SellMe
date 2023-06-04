using SuspiciousGames.SellMe.Core.Generators;
using SuspiciousGames.SellMe.Core.Items;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemGenerator))]
public class ItemGeneratorEditor : Editor
{
    private SerializedObject _sObj;
    private ItemGenerator _itemGenerator;

    private void OnEnable()
    {
        _itemGenerator = (ItemGenerator)target;
        _sObj = new SerializedObject(target);
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(_sObj.FindProperty("_numOfTraits"));
        EditorGUILayout.PropertyField(_sObj.FindProperty("_traitDatas"));
        EditorGUILayout.PropertyField(_sObj.FindProperty("rarityTable"));
        if (!_itemGenerator.rarityTable)
        {
            string[] paths = AssetDatabase.FindAssets("RarityTable", new string[] { "Assets/ScriptableObjects" });
            if (paths.Length == 0)
            {
                if (GUILayout.Button("Create Rarity Table"))
                {
                    RarityTable asset = CreateInstance<RarityTable>();

                    AssetDatabase.CreateAsset(asset, "Assets/ScriptableObjects/ProbabilityTables/RarityTable.asset");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    EditorUtility.FocusProjectWindow();
                    _itemGenerator.rarityTable = asset;
                    Selection.activeObject = asset;
                }
            }
            else
            {
                _itemGenerator.rarityTable = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(paths[0]), typeof(RarityTable)) as RarityTable;
            }
        }
        EditorGUILayout.PropertyField(_sObj.FindProperty("categoryTable"));
        if (!_itemGenerator.categoryTable)
        {
            string[] paths = AssetDatabase.FindAssets("CategoryTable", new string[] { "Assets/ScriptableObjects" });
            if (paths.Length == 0)
            {
                if (GUILayout.Button("Create Category Table"))
                {
                    CategoryTable asset = CreateInstance<CategoryTable>();

                    AssetDatabase.CreateAsset(asset, "Assets/ScriptableObjects/ProbabilityTables/CategoryTable.asset");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    EditorUtility.FocusProjectWindow();
                    _itemGenerator.categoryTable = asset;

                    Selection.activeObject = asset;
                }
            }
            else
            {
                _itemGenerator.categoryTable = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(paths[0]), typeof(CategoryTable)) as CategoryTable;
            }
        }
        EditorGUILayout.PropertyField(_sObj.FindProperty("traitTable"));
        if (!_itemGenerator.traitTable)
        {
            string[] paths = AssetDatabase.FindAssets("TraitTable", new string[] { "Assets/ScriptableObjects" });
            if (paths.Length == 0)
            {
                if (GUILayout.Button("Create Trait Table"))
                {
                    TraitTable asset = CreateInstance<TraitTable>();

                    AssetDatabase.CreateAsset(asset, "Assets/ScriptableObjects/ProbabilityTables/TraitTable.asset");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    EditorUtility.FocusProjectWindow();
                    _itemGenerator.traitTable = asset;

                    Selection.activeObject = asset;
                }
            }
            else
            {
                _itemGenerator.traitTable = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(paths[0]), typeof(TraitTable)) as TraitTable;
            }
        }

        if (GUILayout.Button("Generate"))
        {
            ItemGenerator.Instance.Generate(out Item _);
        }
        _sObj.ApplyModifiedProperties();
    }
}
