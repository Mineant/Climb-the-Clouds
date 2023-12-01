using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public class RarityLootTable : MonoBehaviour
// {
//     // Start is called before the first frame update
//     void Start()
//     {

//     }

//     // Update is called once per frame
//     void Update()
//     {

//     }
// }

[CreateAssetMenu(fileName = "RarityLootTableSO", menuName = "MyGame/Loot/RarityLootTableSO", order = 0)]
public class RarityLootTableSO : MintLootTableScriptableObject<RarityLootTable, RarityLoot, Rarity>
{

}

[System.Serializable]
public class RarityLootTable : MintLootTable<RarityLoot, Rarity>
{

}

[System.Serializable]
public class RarityLoot : MintLoot<Rarity>
{
    public RarityLoot(Rarity loot, float weight) : base(loot, weight)
    {
    }

}