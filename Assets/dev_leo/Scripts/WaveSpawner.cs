using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public static WaveSpawner Instance { get; private set; }

    [System.Serializable]
    public class SpawnLine
    {
        public Transform spawnPoint;
    }

    [System.Serializable]
    public class PrefabSpawnData
    {
        public GameObject prefab;
        public Vector3 initialSpeed = Vector3.zero;
    }

    [System.Serializable]
    public class LetterGroup
    {
        public string groupName = "Group";
        public List<char> letters = new List<char>();
    }

    [System.Serializable]
    public class WaveDefinition
    {
        public string waveName = "Wave";
        public List<PrefabSpawnData> enemyPrefabs = new List<PrefabSpawnData>();
        public List<int> spawnLineIndices = new List<int>();
        public float spawnInterval = 2f;
        public bool repeatWave = true;
        public bool randomizeSpawnLines = false;
        public int minSpawnLines = 1;
        public int maxSpawnLines = 3;
        public bool limitSpawns = false;
        public int maxSpawnsInWave = 10;
        public bool avoidLetterRepeats = false;
        public bool useAllLetterGroups = true;
        public List<string> letterGroupNames = new List<string>();
        // public List<LetterGroup> letterGroups = new List<LetterGroup>();
    }

    [SerializeField] private List<SpawnLine> spawnLines = new List<SpawnLine>();
    [SerializeField] private List<WaveDefinition> waves = new List<WaveDefinition>();
    [SerializeField] private List<LetterGroup> letterGroups = new List<LetterGroup>();
    [SerializeField] private bool loopWaves = true;
    [SerializeField] private bool autoStartOnAwake = false;

    private float waveTimer = 0f;
    private int currentWaveIndex = 0;
    private int currentSpawnCount = 0;
    private bool waveEnded = false;
    private bool isSpawning = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("WaveSpawner singleton already exists. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (autoStartOnAwake)
        {
            StartSpawn();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Update()
    {
        if (!isSpawning || waves.Count == 0)
        {
            return;
        }

        WaveDefinition currentWave = waves[currentWaveIndex];

        if (currentWave.limitSpawns && currentSpawnCount >= currentWave.maxSpawnsInWave && !waveEnded)
        {
            waveEnded = true;
            MoveToNextWave();
            return;
        }

        if (waveEnded)
        {
            return;
        }

        waveTimer += Time.deltaTime;
        if (waveTimer < currentWave.spawnInterval)
        {
            return;
        }

        SpawnWave(currentWave);
        waveTimer = 0f;

        if (currentWave.limitSpawns && currentSpawnCount >= currentWave.maxSpawnsInWave)
        {
            waveEnded = true;
            MoveToNextWave();
        }
        else if (!currentWave.limitSpawns && !currentWave.repeatWave)
        {
            waveEnded = true;
            MoveToNextWave();
        }
    }

    public void StartSpawn()
    {
        if (waves.Count == 0)
        {
            Debug.LogWarning("Wave list is empty. Add waves in inspector.");
            return;
        }

        isSpawning = true;
    }

    public void StopSpawn()
    {
        isSpawning = false;
    }

    public void SetWave(int waveIndex)
    {
        if (waves.Count == 0)
        {
            Debug.LogWarning("Wave list is empty. Add waves in inspector.");
            return;
        }

        currentWaveIndex = Mathf.Clamp(waveIndex, 0, waves.Count - 1);
        waveTimer = 0f;
        currentSpawnCount = 0;
        waveEnded = false;
    }

    public void NextWave()
    {
        MoveToNextWave();
    }

    public void ManualSpawn()
    {
        if (waves.Count == 0)
        {
            Debug.LogWarning("Нет настроенных волн для ручного спавна.");
            return;
        }

        SpawnWave(waves[currentWaveIndex]);
    }

    private void MoveToNextWave()
    {
        if (waves.Count == 0)
        {
            return;
        }

        if (!loopWaves && currentWaveIndex == waves.Count - 1)
        {
            isSpawning = false;
            return;
        }

        currentWaveIndex = (currentWaveIndex + 1) % waves.Count;
        waveTimer = 0f;
        currentSpawnCount = 0;
        waveEnded = false;
    }

    private void SpawnWave(WaveDefinition wave)
    {
        if (spawnLines.Count == 0)
        {
            Debug.LogWarning("Спавнер не настроен: отсутствуют линии спавна.");
            return;
        }

        if (wave.enemyPrefabs == null || wave.enemyPrefabs.Count == 0)
        {
            Debug.LogWarning($"В волне '{wave.waveName}' нет префабов врагов.");
            return;
        }

        List<int> linesToSpawn = GetSpawnLines(wave);
        foreach (int lineIndex in linesToSpawn)
        {
            SpawnOnLine(lineIndex, wave);
        }

        currentSpawnCount++;
    }

    private List<int> GetSpawnLines(WaveDefinition wave)
    {
        List<int> linesToSpawn = new List<int>();

        if (wave.randomizeSpawnLines)
        {
            int lineCount = Random.Range(wave.minSpawnLines, wave.maxSpawnLines + 1);
            lineCount = Mathf.Clamp(lineCount, 1, spawnLines.Count);

            List<int> availableLines = new List<int>();
            for (int i = 0; i < spawnLines.Count; i++)
            {
                availableLines.Add(i);
            }

            for (int i = 0; i < lineCount; i++)
            {
                int randomIndex = Random.Range(0, availableLines.Count);
                linesToSpawn.Add(availableLines[randomIndex]);
                availableLines.RemoveAt(randomIndex);
            }
        }
        else
        {
            if (wave.spawnLineIndices.Count == 0)
            {
                for (int i = 0; i < spawnLines.Count; i++)
                {
                    linesToSpawn.Add(i);
                }
            }
            else
            {
                linesToSpawn.AddRange(wave.spawnLineIndices);
            }
        }

        return linesToSpawn;
    }

    private void SpawnOnLine(int lineIndex, WaveDefinition wave)
    {
        if (lineIndex < 0 || lineIndex >= spawnLines.Count)
        {
            Debug.LogWarning($"Индекс линии вне диапазона: {lineIndex}");
            return;
        }

        SpawnLine line = spawnLines[lineIndex];
        if (line.spawnPoint == null)
        {
            Debug.LogWarning($"Точка спавна для линии {lineIndex} не задана.");
            return;
        }

        PrefabSpawnData prefabData = wave.enemyPrefabs[Random.Range(0, wave.enemyPrefabs.Count)];
        if (prefabData.prefab == null)
        {
            Debug.LogWarning($"В волне '{wave.waveName}' есть пустой префаб.");
            return;
        }

        // GameObject spawnedObject = Instantiate(prefabData.prefab, line.spawnPoint.position, line.spawnPoint.rotation);
        string word = GetRandomWordForWave(wave);
        GameObject spawnedObject = WorldGenerator.Instance.GenerateEnemyWithWord(
        prefabData.prefab,
        line.spawnPoint.position,
        word
        );
        ApplyInitialSpeed(spawnedObject, prefabData.initialSpeed);
    }

    private void ApplyInitialSpeed(GameObject obj, Vector3 speed)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = speed;
            return;
        }

        Rigidbody2D rb2D = obj.GetComponent<Rigidbody2D>();
        if (rb2D != null)
        {
            rb2D.linearVelocity = speed;
            return;
        }

        if (speed != Vector3.zero)
        {
            Debug.LogWarning($"Объект {obj.name} не содержит Rigidbody или Rigidbody2D. Скорость не установлена.");
        }
    }

        // added by slv --
    private string GetRandomWordForWave(WaveDefinition wave)
    {
        List<char> letterPool = new List<char>();

        // Собираем буквы из нужных групп
        if (wave.useAllLetterGroups || wave.letterGroupNames.Count == 0)
        {
            foreach (LetterGroup group in letterGroups)
                letterPool.AddRange(group.letters);
        }
        else
        {
            foreach (string groupName in wave.letterGroupNames)
            {
                LetterGroup group = letterGroups.Find(x => x.groupName == groupName);
                if (group != null)
                    letterPool.AddRange(group.letters);
                else
                    Debug.LogWarning($"Группа букв '{groupName}' не найдена для волны '{wave.waveName}'.");
            }
        }

        if (letterPool.Count == 0)
        {
            Debug.LogWarning("Пул букв пуст, используем дефолтный алфавит A-Z.");
            for (char c = 'A'; c <= 'Z'; c++)
                letterPool.Add(c);
        }

        int wordLength = Random.Range(2, 3);

        // Если включён флаг "без повторов", ограничиваем длину слов пулом
        if (wave.avoidLetterRepeats && wordLength > letterPool.Count)
            wordLength = letterPool.Count;

        List<char> availableLetters = new List<char>(letterPool);
        char[] chars = new char[wordLength];

        for (int i = 0; i < wordLength; i++)
        {
            int index = Random.Range(0, availableLetters.Count);
            chars[i] = availableLetters[index];

            if (wave.avoidLetterRepeats)
                availableLetters.RemoveAt(index); // удаляем, чтобы буква не повторилась
        }

        return new string(chars);
    }
        // -- slv
}
