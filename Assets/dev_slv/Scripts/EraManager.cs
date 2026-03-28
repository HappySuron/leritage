using UnityEngine;

public enum EraType
{
    Doistoricheskiy,
    DrevniyMir,
    Srednevekovye,

    ModernNovyyMir,
    NoveyshiyMir,
    Budushcheye
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
            case EraType.Doistoricheskiy:
                ApplyDoistoricheskiy();
                break;

            case EraType.DrevniyMir:
                ApplyDrevniyMir();
                break;

            case EraType.Srednevekovye:
                ApplySrednevekovye();
                break;

            case EraType.ModernNovyyMir:
                ApplyModernNovyyMir();
                break;

            case EraType.NoveyshiyMir:
                ApplyNoveyshiyMir();
                break;

            case EraType.Budushcheye:
                ApplyBudushcheye();
                break;
        }
    }
    void ApplyDoistoricheskiy()
    {
        Debug.Log("Доисторический");
    }

    void ApplyDrevniyMir()
    {
        Debug.Log("Древний мир");
    }

    void ApplySrednevekovye()
    {
        Debug.Log("Средневековье");
    }

    void ApplyModernNovyyMir()
    {
        Debug.Log("Раннее новое время");
    }

    void ApplyNoveyshiyMir()
    {
        Debug.Log("Новейшее время");
    }

    void ApplyBudushcheye()
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

    public void PlayEraMusic()
    {
        AudioClip clip = null;

        switch (currentEra)
        {
            case EraType.Doistoricheskiy:
                clip = stoneAgeMusic;
                break;

            case EraType.DrevniyMir:
                clip = egyptMusic;
                break;

            case EraType.Srednevekovye:
                clip = medievalMusic;
                break;

            case EraType.ModernNovyyMir:
                clip = newAgeMusic;
                break;

            case EraType.NoveyshiyMir:
                clip = modernMusic;
                break;

            case EraType.Budushcheye:
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

    public void StopEraMusic()
    {
        musicSource.Stop();
    }


}