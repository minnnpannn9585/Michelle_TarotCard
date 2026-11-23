using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject cardGO;
    [SerializeField] GameObject completionOverlay;

    static CardBtn openingCard;
    RectTransform rect;
    Vector2 restPosition;
    Vector3 restScale;
    float phaseOffset;
    float hoverAmount;
    bool hovered;

    bool IsComplete => completionOverlay != null && completionOverlay.activeSelf;

    void Awake()
    {
        rect = (RectTransform)transform;
        restPosition = rect.anchoredPosition;
        restScale = rect.localScale;
        phaseOffset = rect.GetSiblingIndex() * Mathf.PI * 2f / 3f;
    }

    void Update()
    {
        if (openingCard == this || IsComplete)
            return;

        hoverAmount = Mathf.MoveTowards(hoverAmount, hovered ? 1f : 0f, Time.unscaledDeltaTime / 0.16f);
        float blend = Mathf.SmoothStep(0f, 1f, hoverAmount);
        float wave = Mathf.Sin(Time.unscaledTime * 2f + phaseOffset);
        float lift = Mathf.Lerp(3.5f * wave, 6f, blend);
        float scale = Mathf.Lerp(1f + 0.012f * wave, 1.08f, blend);
        rect.anchoredPosition = restPosition + Vector2.up * lift;
        rect.localScale = restScale * scale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
    }

    public void ClickCard()
    {
        if (openingCard != null || IsComplete || !isActiveAndEnabled || cardGO == null)
            return;

        openingCard = this;
        StartCoroutine(OpenPuzzle());
    }

    IEnumerator OpenPuzzle()
    {
        yield return AnimateScale(restScale * 0.95f, 0.09f);
        yield return AnimateScale(restScale * 1.09f, 0.14f);
        cardGO.SetActive(true);
        rect.parent.gameObject.SetActive(false);
    }

    IEnumerator AnimateScale(Vector3 target, float duration)
    {
        Vector3 start = rect.localScale;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            rect.localScale = Vector3.Lerp(start, target, Mathf.SmoothStep(0f, 1f, elapsed / duration));
            yield return null;
        }
    }

    void OnDisable()
    {
        StopAllCoroutines();
        if (openingCard == this)
            openingCard = null;
        hovered = false;
        hoverAmount = 0f;
        rect.anchoredPosition = restPosition;
        rect.localScale = restScale;
    }
}
