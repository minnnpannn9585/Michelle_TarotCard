using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SymbolManager : MonoBehaviour
{
    public SymbolSlot[] symbolSlots;
    public GameObject victoryCanvas;
    public GameObject victoryImage;
    public GameObject victoryBookpage;

    // 每次有卡片放/移除时调用：先检查是否所有卡位都被填满，再判断是否全部放对
    public void CheckWin()
    {

        // 1) 是否所有卡位都已填满
        foreach (var slot in symbolSlots)
        {
            if (!slot.isFilled)
                return; // 还没全部放好，直接返回
        }

        // 2) 所有卡位已填满，逐个检查是否为期望角色（使用 int id 比较）
        foreach (var slot in symbolSlots)
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
        victoryCanvas.SetActive(true);
        victoryBookpage.SetActive(true);
        victoryImage.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
