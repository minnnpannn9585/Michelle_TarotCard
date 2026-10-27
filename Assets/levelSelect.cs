using System.Collections;
using UnityEngine;

public class levelSelect : MonoBehaviour
{
    [Tooltip("Sprite to show after the circle is clicked.")]
    [SerializeField] Sprite otherSprite;
    [Tooltip("Leave empty to use the first SpriteRenderer under this object.")]
    [SerializeField] SpriteRenderer targetRenderer;
    [Tooltip("Shown only while this object is selected. Leave empty to use an inactive child.")]
    [SerializeField] GameObject selectedObject;
    [Tooltip("Select this level as soon as the scene starts.")]
    [SerializeField] bool startSelected;
    [Tooltip("How long the pop lasts when this level is chosen.")]
    [SerializeField] float bounceDuration = 0.4f;
    [Tooltip("How strong the squash is. Keep this small.")]
    [SerializeField] float bounceAmount = 0.08f;

    static levelSelect selected;

    CircleCollider2D circle;
    Camera worldCamera;
    Sprite originalSprite;
    Vector3 cardBaseScale = Vector3.one;
    Vector3 cardBasePosition;
    Coroutine bounceRoutine;

    void Awake()
    {
        circle = GetComponent<CircleCollider2D>();
        if (circle == null)
            circle = GetComponentInChildren<CircleCollider2D>();

        if (targetRenderer == null)
            targetRenderer = FindSpriteUnderParent();

        if (targetRenderer != null)
            originalSprite = targetRenderer.sprite;

        if (selectedObject == null)
            selectedObject = FindInactiveChild();

        if (selectedObject != null)
        {
            cardBaseScale = selectedObject.transform.localScale;
            cardBasePosition = selectedObject.transform.localPosition;
        }

        SetSelectedObjectVisible(false);

        worldCamera = Camera.main;
    }

    void Start()
    {
        if (startSelected)
            Select();
    }

    void OnDestroy()
    {
        if (selected == this)
            selected = null;
    }

    void Update()
    {
        if (!Input.GetMouseButtonDown(0) || otherSprite == null || circle == null)
            return;

        if (worldCamera == null)
            worldCamera = Camera.main;
        if (worldCamera == null)
            return;

        if (IsClickOnCircle())
            Select();
    }

    void Select()
    {
        if (selected != null && selected != this)
            selected.RestoreOriginal();

        selected = this;

        if (targetRenderer != null)
            targetRenderer.sprite = otherSprite;

        SetSelectedObjectVisible(true);
        PlayBounce();
    }

    void RestoreOriginal()
    {
        StopBounce();

        if (targetRenderer != null)
            targetRenderer.sprite = originalSprite;

        SetSelectedObjectVisible(false);
    }

    void PlayBounce()
    {
        if (selectedObject == null)
            return;

        if (bounceRoutine != null)
            StopCoroutine(bounceRoutine);

        bounceRoutine = StartCoroutine(Bounce());
    }

    void StopBounce()
    {
        if (bounceRoutine != null)
        {
            StopCoroutine(bounceRoutine);
            bounceRoutine = null;
        }

        ResetCardTransform();
    }

    void ResetCardTransform()
    {
        if (selectedObject == null)
            return;

        selectedObject.transform.localScale = cardBaseScale;
        selectedObject.transform.localPosition = cardBasePosition;
    }

    IEnumerator Bounce()
    {
        float duration = Mathf.Max(0.05f, bounceDuration);
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float p = Mathf.Clamp01(time / duration);
            float settle = 1f - Mathf.Exp(-8f * p);
            float pop = Mathf.Lerp(1f - bounceAmount * 1.5f, 1f, settle);
            float wobble = Mathf.Sin(p * Mathf.PI * 2.5f) * Mathf.Exp(-3.2f * p);
            float x = pop * (1f + wobble * bounceAmount);
            float y = pop * (1f - wobble * bounceAmount * 1.35f);
            float hop = Mathf.Sin(p * Mathf.PI) * Mathf.Exp(-2.2f * p) * bounceAmount * 2.2f;

            selectedObject.transform.localScale = new Vector3(cardBaseScale.x * x, cardBaseScale.y * y, cardBaseScale.z);
            selectedObject.transform.localPosition = cardBasePosition + new Vector3(0f, hop, 0f);
            yield return null;
        }

        ResetCardTransform();
        bounceRoutine = null;
    }

    void SetSelectedObjectVisible(bool visible)
    {
        if (selectedObject != null && selectedObject.activeSelf != visible)
            selectedObject.SetActive(visible);
    }

    bool IsClickOnCircle()
    {
        Vector3 screen = Input.mousePosition;
        float depth = Mathf.Abs(worldCamera.transform.position.z - circle.transform.position.z);
        screen.z = depth;
        Vector3 world = worldCamera.ScreenToWorldPoint(screen);
        return circle.OverlapPoint(world);
    }

    SpriteRenderer FindSpriteUnderParent()
    {
        foreach (Transform child in transform)
        {
            SpriteRenderer childRenderer = child.GetComponent<SpriteRenderer>();
            if (childRenderer != null)
                return childRenderer;
        }

        return GetComponent<SpriteRenderer>();
    }

    GameObject FindInactiveChild()
    {
        foreach (Transform child in transform)
        {
            if (!child.gameObject.activeSelf
                && (targetRenderer == null || child.gameObject != targetRenderer.gameObject))
                return child.gameObject;
        }

        return null;
    }
}
