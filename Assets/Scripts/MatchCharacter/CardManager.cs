using UnityEngine;
using UnityEngine.SceneManagement;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }

    private CardSlot[] cardSlots;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // 缓存场景中的所有卡位
        cardSlots = FindObjectsOfType<CardSlot>();
    }

    public void CheckWin()
    {
        if (cardSlots == null || cardSlots.Length == 0) return;

        // 1) 是否所有卡位都已填满
        foreach (var slot in cardSlots)
        {
            if (!slot.isFilled)
                return; // 还没全部放好，直接返回
        }

        // 2) 所有卡位已填满，逐个检查是否为期望角色（使用 int id 比较）
        foreach (var slot in cardSlots)
        {
            if (slot.currentCharacter == null)
            {
                Debug.Log("All slots filled but a slot has no currentCharacter reference.");
                return;
            }

            // 使用 CardSlot.IsCorrectPlacement()（基于 int id） 做比较
            if (!slot.IsCorrectPlacement())
            {
                Debug.Log("All slots filled but placement incorrect (ID mismatch).");
                return;
            }
        }

        // 全部填满且全部放对 -> 胜利
        Debug.Log("Victory! All cards placed correctly.");
        SceneManager.LoadScene(1);
    }
}