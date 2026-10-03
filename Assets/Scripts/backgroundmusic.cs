using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class backgroundmusic : MonoBehaviour
{
    [SerializeField] AudioClip musicClip;
    [SerializeField, Range(0f, 1f)] float volume = 0.6f;
    [Tooltip("Skip silence MP3 encoders add at the start. 0.025 is typical.")]
    [SerializeField] float startTrim = 0.025f;
    [Tooltip("Skip silence MP3 encoders add at the end. Raise this if a pause remains.")]
    [SerializeField] float endTrim = 0.08f;
    [Tooltip("Scene names where this track should loop. Leave empty to play in every scene.")]
    [SerializeField] string[] playInScenes = { "MainMenu", "StartGame" };

    AudioSource[] sources;
    int flip;
    double nextStartTime;
    bool looping;

    void Awake()
    {
        if (FindObjectsOfType<backgroundmusic>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        AudioSource existing = GetComponent<AudioSource>();
        AudioSource extra = gameObject.AddComponent<AudioSource>();
        sources = new[] { existing, extra };

        for (int i = 0; i < sources.Length; i++)
            ConfigureSource(sources[i]);

        SceneManager.sceneLoaded += OnSceneLoaded;
        ApplyForScene(SceneManager.GetActiveScene().name);
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        if (!looping || musicClip == null)
            return;

        if (AudioSettings.dspTime < nextStartTime - 0.5)
            return;

        nextStartTime += LoopDuration();
        Schedule(sources[flip], nextStartTime);
        flip = 1 - flip;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyForScene(scene.name);
    }

    void ApplyForScene(string sceneName)
    {
        if (sources == null)
            return;

        if (ShouldPlayInScene(sceneName))
        {
            if (!looping)
                BeginLoop();
        }
        else if (looping)
        {
            StopLoop();
        }
    }

    void BeginLoop()
    {
        if (musicClip == null)
            return;

        for (int i = 0; i < sources.Length; i++)
        {
            ConfigureSource(sources[i]);
            sources[i].clip = musicClip;
        }

        double duration = LoopDuration();
        double start = AudioSettings.dspTime + 0.05;
        Schedule(sources[0], start);
        nextStartTime = start + duration;
        Schedule(sources[1], nextStartTime);
        flip = 0;
        looping = true;
    }

    void StopLoop()
    {
        looping = false;
        for (int i = 0; i < sources.Length; i++)
            sources[i].Stop();
    }

    void ConfigureSource(AudioSource source)
    {
        source.playOnAwake = false;
        source.loop = false;
        source.spatialBlend = 0f;
        source.volume = volume;
    }

    void Schedule(AudioSource source, double dspTime)
    {
        source.Stop();
        source.clip = musicClip;
        source.PlayScheduled(dspTime);
        source.time = Mathf.Max(0f, startTrim);
        source.SetScheduledEndTime(dspTime + LoopDuration());
    }

    double LoopDuration()
    {
        double full = (double)musicClip.samples / musicClip.frequency;
        double duration = full - startTrim - endTrim;
        return duration > 0.05 ? duration : full;
    }

    bool ShouldPlayInScene(string sceneName)
    {
        if (playInScenes == null || playInScenes.Length == 0)
            return true;

        for (int i = 0; i < playInScenes.Length; i++)
        {
            if (playInScenes[i] == sceneName)
                return true;
        }

        return false;
    }
}
