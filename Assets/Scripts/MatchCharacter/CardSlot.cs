using System.Collections;
using UnityEngine;

public class CardSlot : MonoBehaviour
{
    // 标记卡牌是否被角色占用
    public bool isFilled = false;
    // 可选：记录当前在卡牌上的角色
    public GameObject currentCharacter;

    // 新增：期望放入该卡位的角色ID（在 Inspector 设置）
    public int expectedId = -1;

    // 新增：记录当前放入角色的ID（由角色在放置时写入）
    public int currentCharacterId = -1;

    Transform cardVisual;
    Vector3 cardVisualBaseScale;
    Coroutine dropRoutine;

    void Awake()
    {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<SpriteRenderer>() == null)
                continue;

            cardVisual = child;
            cardVisualBaseScale = child.localScale;
            break;
        }
    }

    public void PlayDropFeedback()
    {
        if (cardVisual == null)
            return;

        if (dropRoutine != null)
            StopCoroutine(dropRoutine);

        cardVisual.localScale = cardVisualBaseScale;
        dropRoutine = StartCoroutine(DropPunch());
    }

    IEnumerator DropPunch()
    {
        const float duration = 0.16f;
        const float amount = 0.08f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float p = Mathf.Clamp01(time / duration);
            float pop = Mathf.Sin(p * Mathf.PI) * Mathf.Exp(-2.2f * p);
            float x = 1f + pop * amount;
            float y = 1f - pop * amount;
            cardVisual.localScale = new Vector3(
                cardVisualBaseScale.x * x,
                cardVisualBaseScale.y * y,
                cardVisualBaseScale.z);
            yield return null;
        }

        cardVisual.localScale = cardVisualBaseScale;
        dropRoutine = null;
    }

    // 判断当前卡位是否放对（使用 int id 比较）
    public bool IsCorrectPlacement()
    {
        return isFilled && currentCharacter != null && currentCharacterId == expectedId;
    }
}