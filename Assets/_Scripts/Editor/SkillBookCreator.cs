using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

public class SkillBookCreator : OdinEditorWindow
{
    public class SpellBookData
    {
        public string Name;
        public string Description;
        public Rarity Rarity;
        public float Capacity;
        public float MaxQi;
        public float QiRegen;
        public float SkillTime;
        public float RecoveryTime;

        public SpellBookData(string name, string description, Rarity rarity, float capacity, float maxQi, float qiRegen, float skillTime, float regenTime)
        {
            Name = name;
            Description = description;
            Rarity = rarity;
            Capacity = capacity;
            MaxQi = maxQi;
            QiRegen = qiRegen;
            SkillTime = skillTime;
            RecoveryTime = regenTime;
        }
    }



    [MenuItem("Tools/Upgrade Creator")]
    private static void OpenWindow()
    {
        var window = GetWindow<SkillBookCreator>();
        window.StartingColumn = 'c';
        window.EndingColumn = 'c';
        window.Show();
    }


    [BoxGroup("Create")]
    public TextAsset SpellBookTable;

    [BoxGroup("Create")]
    public char StartingColumn;

    [BoxGroup("Create")]
    public char EndingColumn;


    // [BoxGroup("Info")]
    // public TextAsset InfoFile;


    const string DESTINATION_FOLDER_PATH = "Assets/_Game/Skill Book";
    const string TEMPLATE_SKILL_BOOK_PATH = "Assets/_Game/Skill Book/_New Skill Book Template/_New Skill Book.asset";
    const string TEMPLATE_SPECIAL_EFFECT_PREFAB_PATH = "Assets/_Game/Skill Book/_New Skill Book Template/_New Skill Book Special Effect.prefab";
    const string TEMPLATE_SKILL_BOOK_STATS_PATH = "Assets/_Game/Skill Book/_New Skill Book Template/_New Skill Book Stats.asset";
    const string INFO_FILE_PATH = "Assets/_Game/SkillBookInformation.md";


    [Button]
    [BoxGroup("Create")]
    public void Create()
    {
        // Load Prefabs
        SkillBook templateSkillBook = (SkillBook)AssetDatabase.LoadAssetAtPath(TEMPLATE_SKILL_BOOK_PATH, typeof(SkillBook));
        GameObject templateSpecialEffect = (GameObject)AssetDatabase.LoadAssetAtPath(TEMPLATE_SPECIAL_EFFECT_PREFAB_PATH, typeof(GameObject));
        BasicStats templateSkillBookStats = (BasicStats)AssetDatabase.LoadAssetAtPath(TEMPLATE_SKILL_BOOK_STATS_PATH, typeof(BasicStats));

        // Get the table
        var table = CSVReader.SplitCsvGrid(SpellBookTable.text);

        // Get useful information
        int startCol = Helpers.LetterToNumber(StartingColumn) - 1;
        int endCol = Helpers.LetterToNumber(EndingColumn) - 1;

        // Get a Skill Book
        for (int col = startCol; col <= endCol; col++)
        {
            for (int j = 0; j < 4; j++)
            {
                var skillBookData = new SpellBookData(GetCell(col, j, 0), table[col, 11], (Rarity)j, GetCellNumber(col, j, 1), GetCellNumber(col, j, 2), GetCellNumber(col, j, 3), GetCellNumber(col, j, 4), GetCellNumber(col, j, 5));
                string skillBookName = $"{skillBookData.Name}";
                var skillBookFolderPath = $"{DESTINATION_FOLDER_PATH}/{skillBookName}";
                if (AssetDatabase.IsValidFolder(skillBookFolderPath))
                {
                    Debug.Log($"{skillBookFolderPath} already exists.");
                    continue;
                }
                AssetDatabase.CreateFolder(DESTINATION_FOLDER_PATH, skillBookName);

                // Create Special Effect
                GameObject specialEffectInstance = (GameObject)PrefabUtility.InstantiatePrefab(templateSpecialEffect.gameObject);
                GameObject specialEffectPrefab = PrefabUtility.SaveAsPrefabAsset(specialEffectInstance, $"{skillBookFolderPath}/{skillBookName} Special Effect.prefab");
                EditorUtility.SetDirty(specialEffectPrefab);

                // Create Stats
                BasicStats skillBookStatsInstance = ScriptableObject.CreateInstance<BasicStats>();
                AssetDatabase.CreateAsset(skillBookStatsInstance, $"{skillBookFolderPath}/{skillBookName} Stats.asset");
                skillBookStatsInstance.GenerateSkillBookStats();
                skillBookStatsInstance.StatDictionaryAllInOne.BaseValueStatDictionary[Stat.SkillCapacity] = skillBookData.Capacity;
                skillBookStatsInstance.StatDictionaryAllInOne.FlatModifierStatDictionary[Stat.MaxQi] = skillBookData.MaxQi;
                skillBookStatsInstance.StatDictionaryAllInOne.FlatModifierStatDictionary[Stat.QiRegen] = skillBookData.QiRegen;
                skillBookStatsInstance.StatDictionaryAllInOne.FlatModifierStatDictionary[Stat.SkillTime] = skillBookData.SkillTime;
                skillBookStatsInstance.StatDictionaryAllInOne.FlatModifierStatDictionary[Stat.RecoveryTime] = skillBookData.RecoveryTime;
                EditorUtility.SetDirty(skillBookStatsInstance);

                // Create skill book
                SkillBook skillBookInstance = Instantiate(templateSkillBook);
                AssetDatabase.CreateAsset(skillBookInstance, $"{skillBookFolderPath}/{skillBookName}.asset");
                skillBookInstance.ItemID = skillBookName;
                skillBookInstance.ItemName = skillBookName;
                skillBookInstance.BasicStats = skillBookStatsInstance;
                skillBookInstance.SpecialEffect = specialEffectPrefab.GetComponent<BaseUpgradeMono>();
                skillBookInstance.Rarity = (Rarity)j;
                EditorUtility.SetDirty(skillBookInstance);

                AssetDatabase.SaveAssets();
            }
        }

        AssetDatabase.SaveAssets();

        float GetCellNumber(int col, int rarity, int offset)
        {
            return float.Parse(GetCell(col, rarity, offset));
        }

        string GetCell(int col, int rarity, int offset)
        {
            return table[col, 13 + rarity * 8 + offset];
        }
    }






    [Button]
    [BoxGroup("Info")]
    public void PrintSkillBooks()
    {
        var skillBooks = EditorHelpers.GetAllScriptableObjectInstances<SkillBook>();

        List<string> skillBookInfos = new();
        foreach (var skillBook in skillBooks)
        {
            string text = "";
            text += $"{skillBook.ItemName}  Rarity:{skillBook.Rarity}   Skill Types: {String.Join(", ", skillBook.RelatedSkillTypes)}\n";
            if (skillBook.BasicStats != null) text += $"Stats: {StatHelper.GetAllInOneStatString(skillBook.BasicStats.StatDictionaryAllInOne, ", ")}\n";
            if (skillBook.SpecialEffectVariations.Count > 0)
            {
                for (int i = 0; i < skillBook.SpecialEffectVariations.Count; i++)
                {
                    text += $"Special Effect {i}: {skillBook.SpecialEffectVariations[i].GetDescription()}\n";
                }
            }
            else
            {
                if (skillBook.SpecialEffect != null) text += $"Special Effect: {skillBook.SpecialEffect.GetDescription()}\n";
            }
            skillBookInfos.Add(text);
        }

        StreamWriter writer = new StreamWriter(INFO_FILE_PATH, true);
        writer.WriteLine(String.Join("\n\n--------------------------------------------------------------------\n\n", skillBookInfos));
        writer.Close();
    }
}
