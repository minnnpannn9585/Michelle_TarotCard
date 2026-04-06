using System;
using System.Linq;
using UnityEngine;

public class CutsceneClick : MonoBehaviour
{
    [Header("自动生成的序列（从本物体的直接子物体收集，运行时填充）")]
    [SerializeField] private GameObject[] sequenceObjects;

    [Header("可选")]
    [Tooltip("播完最后一项后，是否停在最后一项；关闭则会全部隐藏")]
    public bool keepLastVisible = true;

    private int _index;

    void Awake()
    {
        // 父物体就是自己：直接从 transform 收集直接子物体
        sequenceObjects = CollectDirectChildren(transform);
        HideAll();
    }

    void Start()
    {
        _index = 0;
        ShowIndex(_index);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Next();
    }

    private void Next()
    {
        if (sequenceObjects == null || sequenceObjects.Length == 0)
            return;

        HideIndex(_index);
        _index++;

        if (_index >= sequenceObjects.Length)
        {
            if (keepLastVisible)
            {
                _index = sequenceObjects.Length - 1;
                ShowIndex(_index);
            }
            // 不保留最后：播完就停在全隐藏状态
            return;
        }

        ShowIndex(_index);
    }

    private void ShowIndex(int i)
    {
        if (i >= 0 && i < sequenceObjects.Length && sequenceObjects[i] != null)
            sequenceObjects[i].SetActive(true);
    }

    private void HideIndex(int i)   
    {
        if (i >= 0 && i < sequenceObjects.Length && sequenceObjects[i] != null)
            sequenceObjects[i].SetActive(false);
    }

    private void HideAll()
    {
        if (sequenceObjects == null) return;

        for (int i = 0; i < sequenceObjects.Length; i++)
        {
            if (sequenceObjects[i] != null)
                sequenceObjects[i].SetActive(false);
        }
    }

    private static GameObject[] CollectDirectChildren(Transform parent)
    {
        if (parent == null) return Array.Empty<GameObject>();

        // Transform 枚举顺序就是 Hierarchy 顺序
        return parent.Cast<Transform>()
            .Where(t => t != null)
            .Select(t => t.gameObject)
            .ToArray();
    }
}