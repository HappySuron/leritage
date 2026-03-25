using UnityEngine;
using UnityEngine.Audio;
public class PerfectLoop : MonoBehaviour
{
    AudioSource source;
    double nextTime;

    void Start()
    {
        source = GetComponent<AudioSource>();
        nextTime = AudioSettings.dspTime;
        PlayLoop();
    }

    void PlayLoop()
    {
        double length = source.clip.length;
        source.PlayScheduled(nextTime);
        nextTime += length;
    }
}
