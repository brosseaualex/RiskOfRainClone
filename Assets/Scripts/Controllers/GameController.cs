using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    #region Singleton
    private static GameController instance;

    private GameController() { }

    public static GameController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameController();
            }
            return instance;
        }
    }
    #endregion

    public Transform monolithParent;
    public Transform enemyParent;
    public Transform pickupsParent;

    [Header("Monolith Spawning")]
    public int maxMonolith = 10;
    public int gameStartMonolithCount = 3;
    public float monolithSpawnDelayMin = 5f;
    public float monolithSpawnDelayMax = 10f;
    public GameObject monolithPrefab;
    [HideInInspector]
    public List<GameObject> monoliths, enemies;

    float monolithSpawnDelay;
    float timeUntilNextMonolithSpawn = 0f;

    static Terrain terrain;
    static float terrainWidth;
    static float terrainLength;

    static float xTerrainPos;
    static float zTerrainPos;

    static float terrainOffset = 60f;

    static float monolithSpawnHeightMin = 75f;
    static float monolithSpawnHeightMax = 150f;

    [HideInInspector]
    public bool hasWon, hasLost, isPaused;
    
    void Awake()
    {
        instance = this;

        monoliths = new List<GameObject>();
        enemies = new List<GameObject>();

        terrain = GameObject.FindObjectOfType<Terrain>();

        terrainWidth = terrain.terrainData.size.x;
        terrainLength = terrain.terrainData.size.z;

        xTerrainPos = terrain.transform.position.x;
        zTerrainPos = terrain.transform.position.z;

        Time.timeScale = 1;

        ResetTimers();
    }

    void Start()
    {
        for (int i = 0; i < gameStartMonolithCount; i++)
        {
            SpawnMonolith();
        }
    }

    void Update()
    {
        if (monoliths.Count < maxMonolith)
        {
            if (Time.time > timeUntilNextMonolithSpawn)
            {
                SpawnMonolith();
                ResetTimers();
            }
        }        
    }

    void SpawnMonolith()
    {
        Vector3 spawnPos = FindSpawnPosition();
        GameObject monolithClone = Instantiate(monolithPrefab, spawnPos, Quaternion.identity);
        monolithClone.transform.SetParent(monolithParent.transform);
        AddMonolithToList(monolithClone);
    }

    void ResetTimers()
    {
        monolithSpawnDelay = Random.Range(monolithSpawnDelayMin, monolithSpawnDelayMax);
        timeUntilNextMonolithSpawn = Time.time + monolithSpawnDelay;
    }

    public void AddMonolithToList(GameObject monolith)
    {
        monoliths.Add(monolith);
    }

    public void RemoveMonolithFromList(GameObject monolith)
    {
        monoliths.Remove(monolith);

        if (monoliths.Count == 0)
        {
            maxMonolith = 0;
        }
    }

    public void AddEnemyToList(GameObject enemy)
    {
        enemies.Add(enemy);
    }

    public void RemoveEnemyFromList(GameObject enemy)
    {
        enemies.Remove(enemy);
    }

    public Vector3 FindSpawnPosition()
    {
        Vector3 validSpawn;

        float xRand = Random.Range(xTerrainPos + terrainOffset, xTerrainPos - terrainOffset + terrainWidth);
        float zRand = Random.Range(zTerrainPos + terrainOffset, zTerrainPos - terrainOffset + terrainLength);

        validSpawn.x = xRand;
        validSpawn.y = Random.Range(monolithSpawnHeightMin, monolithSpawnHeightMax);
        validSpawn.z = zRand;

        return validSpawn;
    }

    public bool CheckWinConditions()
    {
        if(monoliths.Count == 0 && enemies.Count == 0)
        {
            //hasWon = true;
        }

        return hasWon;
    }

    public bool CheckLoseConditions()
    {
        if (PlayerController.Instance.stats.hp <= 0)
        {
            hasLost = true;
        }

        return hasLost;
    }

    public IEnumerator FadeLineRenderer(LineRenderer lineRenderer)
    {
        Gradient lineRendererGradient = new Gradient();
        float fadeSpeed = 3f;
        float timeElapsed = 0f;
        float alpha = 1f;

        while (timeElapsed < fadeSpeed)
        {
            alpha = Mathf.Lerp(1f, 0f, timeElapsed / fadeSpeed);

            lineRendererGradient.SetKeys
            (
                lineRenderer.colorGradient.colorKeys,
                new GradientAlphaKey[] { new GradientAlphaKey(alpha, 1f) }
            );
            lineRenderer.colorGradient = lineRendererGradient;

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(lineRenderer.gameObject);
    }
}
