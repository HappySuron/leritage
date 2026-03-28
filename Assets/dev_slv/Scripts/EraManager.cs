using UnityEngine;

public enum EraType
{
    StoneAge,
    AncientWorld,
    MiddleAges,
    ModernHistory,
    ModernerHistory,
    Future
}

public class EraManager : MonoBehaviour
{
    public static EraManager Instance { get; private set; }

    public EraType currentEra;

    //private AudioSource musicSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // кешируем AudioSource один раз
            //musicSource = SoundMixerManager.Instance.GetComponentInChildren<AudioSource>();
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
                Debug.Log("Первобытная история");
                break;

            case EraType.AncientWorld:
                Debug.Log("Древний мир");
                break;

            case EraType.MiddleAges:
                Debug.Log("Средневековье");
                break;

            case EraType.ModernHistory:
                Debug.Log("Новая история");
                break;

            case EraType.ModernerHistory:
                Debug.Log("Новейшая история");
                break;

            case EraType.Future:
                Debug.Log("Будущее");
                break;
        }
    }

    [ContextMenu("Next Era")]
    public void MoveToNextEra()
    {
        int nextIndex = (int)currentEra + 1;

        if (nextIndex >= System.Enum.GetValues(typeof(EraType)).Length)
        {
            Debug.Log("SLV - index out of range - ERA");
            return;
        }

        WaveSpawner.Instance.NextWave();

        SwitchEra((EraType)nextIndex);
        this.PlayEraMusic();
    }

    void PlayEraMusic()
    {
        AudioClip clip = null;

        switch (currentEra)
        {
            case EraType.StoneAge:
                clip = SoundMixerManager.Instance.stoneAgeMusic;
                break;

            case EraType.AncientWorld:
                clip = SoundMixerManager.Instance.egyptMusic;
                break;

            case EraType.MiddleAges:
                clip = SoundMixerManager.Instance.medievalMusic;
                break;

            case EraType.ModernHistory:
                clip = SoundMixerManager.Instance.modernMusic;
                break;

            case EraType.ModernerHistory:
                clip = SoundMixerManager.Instance.modernMusic;
                break;

            case EraType.Future:
                clip = SoundMixerManager.Instance.futureMusic;
                break;
        }

        if (clip != null && SoundMixerManager.Instance.GetComponentInChildren<AudioSource>() != null)
        {
            SoundMixerManager.Instance.GetComponentInChildren<AudioSource>().clip = clip;
            SoundMixerManager.Instance.GetComponentInChildren<AudioSource>().loop = true;
            SoundMixerManager.Instance.GetComponentInChildren<AudioSource>().Play();
        }
    }

    public void StopMusic()
    {
        if (SoundMixerManager.Instance.GetComponentInChildren<AudioSource>() != null && SoundMixerManager.Instance.GetComponentInChildren<AudioSource>().isPlaying)
        {
            SoundMixerManager.Instance.GetComponentInChildren<AudioSource>().Stop();
        }
    }
}