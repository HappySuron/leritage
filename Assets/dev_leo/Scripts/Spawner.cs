using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
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
        public bool useAllLetterGroups = true;
        public List<string> letterGroupNames = new List<string>();
        public List<int> spawnLineIndices = new List<int>();
        public float spawnInterval = 2f;
        public bool repeatWave = true;
        
        // Новые параметры для режима случайных линий спавна
        public bool randomizeSpawnLines = false;
        public int minSpawnLines = 1;
        public int maxSpawnLines = 3;
        
        // Параметры для ограничения количества спавнов в волне
        public bool limitSpawns = false;
        public int maxSpawnsInWave = 10;

        // added by slv --
        public bool avoidLetterRepeats = false;
        // -- slv

    }

    [SerializeField] private List<SpawnLine> spawnLines = new List<SpawnLine>();
    [SerializeField] private List<PrefabSpawnData> prefabs = new List<PrefabSpawnData>();
    [SerializeField] private List<LetterGroup> letterGroups = new List<LetterGroup>();
    [SerializeField] private List<WaveDefinition> waves = new List<WaveDefinition>();

    [SerializeField] private bool autoCreateDefaultLetterGroups = true;
    [SerializeField] private bool loopWaves = true;



    private float waveTimer = 0f;
    private int currentWaveIndex = 0;
    private int currentSpawnCount = 0;
    private bool waveEnded = false;

    private void Awake()
    {
        if (autoCreateDefaultLetterGroups && letterGroups.Count == 0)
            CreateDefaultLetterGroups();

        if (waves.Count == 0)
            CreateDefaultWave();
    }

    private void Update()
    {
        if (waves.Count == 0)
            return;

        WaveDefinition currentWave = waves[currentWaveIndex];
        
        // Проверяем, достигнут ли лимит спавнов для волны
        if (currentWave.limitSpawns && currentSpawnCount >= currentWave.maxSpawnsInWave && !waveEnded)
        {
            waveEnded = true;
            Debug.Log($"Волна '{currentWave.waveName}' закончена (лимит {currentWave.maxSpawnsInWave} спавнов достигнут)");
            MoveToNextWave();
            return;
        }

        // Если волна закончена, переходим на следующую
        if (waveEnded)
        {
            return;
        }

        waveTimer += Time.deltaTime;

        if (waveTimer >= currentWave.spawnInterval)
        {
            // Проверяем лимит перед спавном
            if (currentWave.limitSpawns && currentSpawnCount >= currentWave.maxSpawnsInWave)
            {
                if (!waveEnded)
                {
                    waveEnded = true;
                    Debug.Log($"Волна '{currentWave.waveName}' закончена (достигнут лимит {currentWave.maxSpawnsInWave} спавнов)");
                }
                return;
            }

            SpawnWave(currentWave);
            waveTimer = 0f;

            // ПОСЛЕ спавна проверяем, достигнут ли лимит
            if (currentWave.limitSpawns && currentSpawnCount >= currentWave.maxSpawnsInWave)
            {
                waveEnded = true;
                Debug.Log($"Волна '{currentWave.waveName}' закончена (достигнут лимит {currentWave.maxSpawnsInWave} спавнов)");
                MoveToNextWave();
            }
            // Если нет лимита и repeatWave = false, заканчиваем после одного спавна
            else if (!currentWave.limitSpawns && !currentWave.repeatWave)
            {
                waveEnded = true;
                Debug.Log($"Волна '{currentWave.waveName}' закончена (repeatWave = false)");
                MoveToNextWave();
            }
        }
    }

    private void CreateDefaultLetterGroups()
    {
        LetterGroup easy = new LetterGroup
        {
            groupName = "Легкие",
            letters = GetCharRange('A', 'M')
        };

        LetterGroup medium = new LetterGroup
        {
            groupName = "Средние",
            letters = GetCharRange('N', 'Z')
        };

        letterGroups.Add(easy);
        letterGroups.Add(medium);
    }

    private List<char> GetCharRange(char from, char to)
    {
        List<char> list = new List<char>();
        for (char c = from; c <= to; c++)
            list.Add(c);
        return list;
    }

    private void CreateDefaultWave()
    {
        WaveDefinition wave = new WaveDefinition
        {
            waveName = "Wave 1",
            useAllLetterGroups = true,
            spawnLineIndices = new List<int>(),
            spawnInterval = 2f,
            repeatWave = true
        };

        for (int i = 0; i < spawnLines.Count; i++)
            wave.spawnLineIndices.Add(i);

        waves.Add(wave);
    }

    private void MoveToNextWave()
    {
        Debug.Log($"Волна '{waves[currentWaveIndex].waveName}' завершена. Переход к следующей волне...");
        if (waves.Count == 0)
        {
            Debug.Log("Нет волн для перехода");
            return;
        }
        if (!loopWaves && currentWaveIndex == waves.Count - 1)
        {
            // Не переходить, остановить спавн
            Debug.Log("Все волны завершены, спавн остановлен.");
            return; // Не сбрасывать waveEnded, оставить true
        }

        currentWaveIndex = (currentWaveIndex + 1) % waves.Count;
        currentSpawnCount = 0;
        waveTimer = 0f;
        waveEnded = false;
        
        WaveDefinition nextWave = waves[currentWaveIndex];
        Debug.Log($"Переход на волну '{nextWave.waveName}' (индекс {currentWaveIndex})");
    }

    public void StartWave(int index)
    {
        if (waves.Count == 0)
        {
            Debug.LogWarning("Wave list is empty. Add waves in inspector.");
            return;
        }

        currentWaveIndex = Mathf.Clamp(index, 0, waves.Count - 1);
        currentSpawnCount = 0;
        waveTimer = 0f;
        waveEnded = false;
        
        Debug.Log($"Начинаем волну '{waves[currentWaveIndex].waveName}'");
    }

    public void NextWave()
    {
        MoveToNextWave();
    }

    public void SpawnWave(WaveDefinition wave)
    {
        if (spawnLines.Count == 0 || prefabs.Count == 0)
        {
            Debug.LogWarning("Спавнер не настроен: отсутствуют линии спавна или префабы!");
            return;
        }

        List<int> linesToSpawn = GetSpawnLines(wave);

        foreach (int index in linesToSpawn)
        {
            if (index < 0 || index >= spawnLines.Count)
            {
                Debug.LogWarning($"Индекс линии для волны {wave.waveName} вне диапазона: {index}");
                continue;
            }

            SpawnOnLine(index, wave);
        }

        // Увеличиваем счетчик спавнов волны
        currentSpawnCount++;
    }

    private List<int> GetSpawnLines(WaveDefinition wave)
    {
        List<int> linesToSpawn = new List<int>();

        if (wave.randomizeSpawnLines)
        {
            // Случайное количество линий между minSpawnLines и maxSpawnLines
            int lineCount = Random.Range(wave.minSpawnLines, wave.maxSpawnLines + 1);
            lineCount = Mathf.Clamp(lineCount, 1, spawnLines.Count);

            // Случайно выбираем линии без повторений
            List<int> availableLines = new List<int>();
            for (int i = 0; i < spawnLines.Count; i++)
                availableLines.Add(i);

            for (int i = 0; i < lineCount; i++)
            {
                int randomIndex = Random.Range(0, availableLines.Count);
                linesToSpawn.Add(availableLines[randomIndex]);
                availableLines.RemoveAt(randomIndex);
            }
        }
        else
        {
            // Используем предустановленные линии
            if (wave.spawnLineIndices.Count == 0)
            {
                for (int i = 0; i < spawnLines.Count; i++)
                    wave.spawnLineIndices.Add(i);
            }

            linesToSpawn.AddRange(wave.spawnLineIndices);
        }

        return linesToSpawn;
    }

    private void SpawnOnLine(int lineIndex, WaveDefinition wave)
    {
        SpawnLine line = spawnLines[lineIndex];

        if (line.spawnPoint == null)
        {
            Debug.LogWarning($"Точка спавна для линии {lineIndex} не задана.");
            return;
        }

        PrefabSpawnData prefabData = prefabs[Random.Range(0, prefabs.Count)];

        if (prefabData.prefab == null)
        {
            Debug.LogWarning("Префаб в данных спавна не задан!");
            return;
        }

        // commented by slv --
        //GameObject spawnedObject = Instantiate(prefabData.prefab, line.spawnPoint.position, line.spawnPoint.rotation);
        // -- slv

        // added by slv --
        string word = GetRandomWordForWave(wave);
        GameObject spawnedObject = WorldGenerator.Instance.GenerateEnemyWithWord(
        prefabData.prefab,
        line.spawnPoint.position,
        word
        );
        // -- slv
        ApplyInitialSpeed(spawnedObject, prefabData.initialSpeed);
        
        char letter = GetRandomLetterForWave(wave);

        //commented by slv --
        //AssignLetterToObject(spawnedObject, letter);

        //Debug.Log($"Спавнен объект '{spawnedObject.name}' на линии {lineIndex} с буквой '{letter}' ({wave.waveName}).");
        // --slv
    }

    private char GetRandomLetterForWave(WaveDefinition wave)
    {
        List<char> letterPool = new List<char>();

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
            Debug.LogWarning("Пул букв пуст, используется дефолтный алфавит A-Z.");
            return (char)('A' + Random.Range(0, 26));
        }

        return letterPool[Random.Range(0, letterPool.Count)];
    }

    private void AssignLetterToObject(GameObject obj, char letter)
    {
        ILetterReceiver letterReceiver = obj.GetComponent<ILetterReceiver>();

        if (letterReceiver != null)
        {
            letterReceiver.SetLetter(letter);
        }
        else
        {
            LetterHolder holder = obj.AddComponent<LetterHolder>();
            holder.SetLetter(letter);

            Debug.LogWarning($"Объект {obj.name} не содержит ILetterReceiver. Добавлен LetterHolder.");
        }
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
            Debug.LogWarning($"Объект {obj.name} не содержит Rigidbody или Rigidbody2D. Скорость не установлена.");
    }

    public void ManualSpawn()
    {
        if (waves.Count > 0)
            SpawnWave(waves[currentWaveIndex]);
        else
            Debug.LogWarning("Нет настроенных волн для ручного спавна.");
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

public interface ILetterReceiver
{
    void SetLetter(char letter);
    char GetLetter();
}

public class LetterHolder : MonoBehaviour, ILetterReceiver
{
    private char letter;

    public void SetLetter(char newLetter)
    {
        letter = newLetter;
        // Здесь можно добавить логику для отображения буквы (текст, спрайт и т.д.)
    }

    public char GetLetter()
    {
        return letter;
    }
}
