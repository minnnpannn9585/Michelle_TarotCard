using UnityEngine;
using UnityEngine.SceneManagement;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }
    // 指向一个胜利面板（在Inspector中指定），可为空（会至少输出日志）
    //public GameObject victoryPanel;

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

    // 每次有卡片放/移除时调用：先检查是否所有卡位都被填满，再判断是否全部放对
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