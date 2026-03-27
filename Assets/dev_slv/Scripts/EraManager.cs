using UnityEngine;

public enum EraType
{
    StoneAge,
    Medieval,
    Modern,
    Future
}

public class EraManager : MonoBehaviour
{
    public static EraManager Instance { get; private set; }

    public EraType currentEra;

    [Header("Музыка")]
    public AudioSource musicSource;
    public AudioClip stoneAgeMusic;
    public AudioClip egyptMusic;
    public AudioClip medievalMusic;
    public AudioClip newAgeMusic;
    public AudioClip modernMusic;
    public AudioClip futureMusic;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SwitchEra(EraType newEra)
    {
        if (currentEra == newEra) return;

        currentEra = newEra;

        Debug.Log("Смена эпохи: " + newEra);

        ApplyEra();
        PlayEraMusic();
    }

    private void ApplyEra()
    {
        switch (currentEra)
        {
            case EraType.StoneAge:
                ApplyStoneAge();
                break;

            case EraType.Medieval:
                ApplyMedieval();
                break;

            case EraType.Modern:
                ApplyModern();
                break;

            case EraType.Future:
                ApplyFuture();
                break;
        }
    }

    void ApplyStoneAge()
    {
        Debug.Log("Каменный век");
        
    }

    void ApplyMedieval()
    {
        Debug.Log("Средневековье");
    }

    void ApplyModern()
    {
        Debug.Log("Современность");
    }

    void ApplyFuture()
    {
        Debug.Log("Будущее");
    }

    [ContextMenu("Next Era")]
    public void MoveToNextEra()
    {
        int nextIndex = (int)currentEra + 1;

        // если вышли за пределы — возвращаемся в начало
        if (nextIndex >= System.Enum.GetValues(typeof(EraType)).Length)
        {
            //nextIndex = 0;
            Debug.Log("SLV - index out of range - ERA");
        }
        
        WaveSpawner.Instance.NextWave();

        EraType nextEra = (EraType)nextIndex;
        SwitchEra(nextEra);
    }

    void PlayEraMusic()
    {
        AudioClip clip = null;

        switch (currentEra)
        {
            case EraType.StoneAge:
                clip = stoneAgeMusic;
                break;

            case EraType.Medieval:
                clip = medievalMusic;
                break;

            case EraType.Modern:
                clip = modernMusic;
                break;

            case EraType.Future:
                clip = futureMusic;
                break;
        }

        if (clip != null)
        {
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }


}