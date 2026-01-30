using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SymbolSlot : MonoBehaviour
{
    // 标记卡牌是否被角色占用
    public bool isFilled = false;
    // 可选：记录当前在卡牌上的角色
    public GameObject currentCharacter;

    // 新增：期望放入该卡位的角色ID（在 Inspector 设置）
    public int expectedId = -1;

    // 新增：记录当前放入角色的ID（由角色在放置时写入）
    public int currentCharacterId = -1;

    // 判断当前卡位是否放对（使用 int id 比较）
    public bool IsCorrectPlacement()
    {
        return isFilled && currentCharacter != null && currentCharacterId == expectedId;
    }
}
