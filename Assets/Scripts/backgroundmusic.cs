using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class backgroundmusic : MonoBehaviour
{
    [SerializeField] AudioClip musicClip;
    [SerializeField, Range(0f, 1f)] float volume = 0.6f;
    [Tooltip("Scene names where this track should loop. Leave empty to play in every scene.")]
    [SerializeField] string[] playInScenes = { "MainMenu" };

    AudioSource audioSource;

    void Awake()
    {
        if (FindObjectsOfType<backgroundmusic>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
        audioSource.volume = volume;

        if (musicClip != null)
            audioSource.clip = musicClip;

        SceneManager.sceneLoaded += OnSceneLoaded;
        ApplyForScene(SceneManager.GetActiveScene().name);
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyForScene(scene.name);
    }

    void ApplyForScene(string sceneName)
    {
        if (audioSource == null)
            return;

        if (musicClip != null && audioSource.clip != musicClip)
            audioSource.clip = musicClip;

        audioSource.volume = volume;
        audioSource.loop = true;

        if (audioSource.clip == null)
            return;

        if (ShouldPlayInScene(sceneName))
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
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
