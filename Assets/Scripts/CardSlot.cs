using UnityEngine;

public class CardSlot : MonoBehaviour
{
    // 标记卡牌是否被角色占用
    public bool isFilled = false;
    // 可选：记录当前在卡牌上的角色
    public GameObject currentCharacter;
}