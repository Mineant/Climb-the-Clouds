using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using DG.Tweening;

public class WaveManager : MMSingleton<WaveManager>, MMEventListener<MMLifeCycleEvent>
{
    // public WaveData CurrentWaveData;
    public BoxCollider SpawnCollider;
    public MMF_Player PreparingToSpawnFeedback;
    public MMF_Player FinishedSpawningFeedback;
    // public PlayableDirector EnemyEnterStageDirectorPrefab;
    public List<Health> SpawnedEnemies;
    public Transform BossSpawnPoint;
    public StatDictionaryAllInOne EnemyModifiers;
    public GameObject BattlingObject;
    public MMF_Player WaveClearedFeedback;

    protected override void Awake()
    {
        base.Awake();
        SpawnedEnemies = new();
        BattlingObject.SetActive(false);
    }

    public void StartWave(WaveData wave)
    {
        StartCoroutine(_WaveCoroutine(wave));
    }

    public void StartBossWave(Character bossEnemy)
    {
        StartCoroutine(_StartBossWaveCoroutine(bossEnemy));
    }

    private IEnumerator _StartBossWaveCoroutine(Character bossEnemy)
    {
        GameEvent.Trigger(GameEventType.EnemyWaveStarted);
        BattlingObject.SetActive(true);
        SpawnedEnemies.Clear();

        EnemyData bossData = new EnemyData(bossEnemy, 1000);
        Character enemyInstance = SpawnEnemy(bossData);
        StartCoroutine(_EnemyEnterStageCoroutine(enemyInstance, false));
        enemyInstance.transform.position = BossSpawnPoint.position;
        SpawnedEnemies.Add(enemyInstance.GetComponent<Health>());

        while (SpawnedEnemies.Count > 0) yield return true;

        BattlingObject.SetActive(false);
        GameEvent.Trigger(GameEventType.EnemyWaveCleared);
        WaveClearedFeedback.PlayFeedbacks();
    }


    private IEnumerator _WaveCoroutine(WaveData wave)
    {
        // CurrentWaveData = wave;
        GameEvent.Trigger(GameEventType.EnemyWaveStarted);
        BattlingObject.SetActive(true);
        SpawnedEnemies.Clear();

        int numberOfWaves = wave.NumberOfWaves;
        int waveMoney = wave.TotalMoney;
        int currentWave = 0;

        for (int i = 0; i < numberOfWaves; i++)
        {
            int currentBudget = waveMoney;

            // Spawn enemies until limit
            while (true)
            {
                if (wave.Enemies.Where(e => e.Cost <= currentBudget).Count() == 0) break;
                EnemyData enemyData = wave.Enemies.Where(e => e.Cost <= currentBudget).PickRandom<EnemyData>();

                // Spawn enemy
                Character enemyInstance = SpawnEnemy(enemyData);
                StartCoroutine(_EnemyEnterStageCoroutine(enemyInstance, currentWave > 0));
                SpawnedEnemies.Add(enemyInstance.GetComponent<Health>());

                currentBudget -= enemyData.Cost;
            }

            // when all enemies are killed, continue the routine
            while (SpawnedEnemies.Count > 0)
                yield return true;

            currentWave++;
        }

        BattlingObject.SetActive(false);
        GameEvent.Trigger(GameEventType.EnemyWaveCleared);
        WaveClearedFeedback.PlayFeedbacks();
    }

    protected Character SpawnEnemy(EnemyData enemyData)
    {
        var enemyInstance = Instantiate(enemyData.EnemyPrefab);
        enemyInstance.GetComponent<Health>().PointsWhenDestroyed = enemyData.Cost * 2;
        enemyInstance.GetComponent<BaseStats>().AddBasicStatModifiers(EnemyModifiers, this);
        enemyInstance.transform.eulerAngles = Vector3.up * 180f;
        return enemyInstance;
    }

    protected IEnumerator _EnemyEnterStageCoroutine(Character enemyInstance, bool playAnimation)
    {
        // Set the enemy final spawnpoint
        Vector3 spawnPosition = Helpers.GetRandomPointInsideCollider(SpawnCollider);
        enemyInstance.transform.position = spawnPosition;

        if (playAnimation)
        {
            // disable all damage, physics, AI, of enemy
            enemyInstance.CharacterHealth.DamageDisabled();
            enemyInstance.GetComponent<Collider>().enabled = false;
            enemyInstance.GetComponent<TopDownController>().enabled = false;
            enemyInstance.CharacterBrain.enabled = false;

            // Wait for random time
            enemyInstance.transform.position = Vector3.down * 100f;
            yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 3f));

            // Play a warning particle effect on the final spawnpoint
            PreparingToSpawnFeedback.PlayFeedbacks(spawnPosition);


            // assign the animators to the correct tracks
            enemyInstance.transform.position = new Vector3(UnityEngine.Random.Range(40f, 50f) * Helpers.RandomPositiveNegativeSign(), UnityEngine.Random.Range(-5f, -10f), UnityEngine.Random.Range(-20f, 20f)) + spawnPosition;
            Sequence tween = enemyInstance.transform.DOJump(spawnPosition, UnityEngine.Random.Range(5f, 10f), 1, UnityEngine.Random.Range(1f, 1.5f));
            yield return tween.WaitForCompletion();

            // when finished, play a particle effect, enable the enemy damage, physics, AI, and destroy the timeline.
            FinishedSpawningFeedback.PlayFeedbacks(spawnPosition);
            enemyInstance.CharacterHealth.DamageEnabled();
            enemyInstance.GetComponent<Collider>().enabled = true;
            enemyInstance.GetComponent<TopDownController>().enabled = true;
            enemyInstance.CharacterBrain.enabled = true;

            enemyInstance.transform.position = spawnPosition;
        }
    }

    public void OnMMEvent(MMLifeCycleEvent eventType)
    {
        if (eventType.MMLifeCycleEventType == MMLifeCycleEventTypes.Death) SpawnedEnemies.Remove(eventType.AffectedHealth);
    }

    void OnEnable()
    {
        this.MMEventStartListening<MMLifeCycleEvent>();
    }

    void OnDisable()
    {
        this.MMEventStopListening<MMLifeCycleEvent>();
    }
}

[System.Serializable]
public class WaveData
{
    public int TotalMoney;
    public int NumberOfWaves;
    public int HighestCostUnit;
    public List<EnemyData> Enemies;

    // public WaveData()
    // {

    // }

    // public WaveData(int totalMoney, int maxCostPerWave, int highestCostUnit, List<EnemyData> enemyInfos)
    // {
    //     TotalMoney = totalMoney;
    //     MaxCostPerWave = maxCostPerWave;
    //     HighestCostUnit = highestCostUnit;
    //     Enemies = enemyInfos;
    // }

}


[System.Serializable]
public class EnemyData
{
    public Character EnemyPrefab;
    public int Cost;

    public EnemyData(Character enemyPrefab, int cost)
    {
        EnemyPrefab = enemyPrefab;
        Cost = cost;
    }

}