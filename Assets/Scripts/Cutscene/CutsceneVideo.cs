using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class CutsceneVideo : MonoBehaviour
{
    [SerializeField] private string nextSceneName;

    private VideoPlayer player;
    private bool leavingScene;

    private void Start()
    {
        player = GetComponent<VideoPlayer>();
        player.loopPointReached += LoadNextScene;
        player.errorReceived += OnVideoError;
        player.Play();
    }

    private void OnDestroy()
    {
        if (player == null)
            return;

        player.loopPointReached -= LoadNextScene;
        player.errorReceived -= OnVideoError;
    }

    private void OnVideoError(VideoPlayer source, string message)
    {
        Debug.LogError($"Cutscene playback failed: {message}", this);
        LoadNextScene(source);
    }

    private void LoadNextScene(VideoPlayer source)
    {
        if (leavingScene)
            return;

        leavingScene = true;
        SceneManager.LoadScene(nextSceneName);
    }
}
