using UnityEngine;

public class levelSelect : MonoBehaviour
{
    [Tooltip("Sprite to show after the circle is clicked.")]
    [SerializeField] Sprite otherSprite;
    [Tooltip("Leave empty to use the first SpriteRenderer under this object.")]
    [SerializeField] SpriteRenderer targetRenderer;
    [Tooltip("Shown only while this object is selected. Leave empty to use an inactive child.")]
    [SerializeField] GameObject selectedObject;

    static levelSelect selected;

    CircleCollider2D circle;
    Camera worldCamera;
    Sprite originalSprite;

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

        SetSelectedObjectVisible(false);

        worldCamera = Camera.main;
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
    }

    void RestoreOriginal()
    {
        if (targetRenderer != null)
            targetRenderer.sprite = originalSprite;

        SetSelectedObjectVisible(false);
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
