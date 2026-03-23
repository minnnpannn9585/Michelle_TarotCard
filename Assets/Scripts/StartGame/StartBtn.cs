using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartBtn : MonoBehaviour
{
    public string nextSceneName;

    private Button _button;
    void Awake()
    {
        _button = GetComponent<Button>();

        // 不用在 Inspector 挂 OnClick，这里直接用代码绑定
        _button.onClick.AddListener(LoadNextScene);
    }

    public void LoadNextScene()
    {

        SceneManager.LoadScene(nextSceneName);
    }
}
