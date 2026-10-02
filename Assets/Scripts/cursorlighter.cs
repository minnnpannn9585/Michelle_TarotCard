using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class cursorlighter : MonoBehaviour
{
    [Header("Sprites")]
    [Tooltip("Bright / light version of this sprite. The SpriteRenderer should show the dark version.")]
    public Sprite lightForm;

    [Header("Reveal")]
    [Tooltip("World-space radius of the fully-lit circle.")]
    public float radius = 1.75f;
    [Tooltip("How far the light fades out past the inner radius.")]
    public float softness = 1.1f;
    [Tooltip("If on, the light circle only appears while the cursor is over this sprite.")]
    public bool onlyWhenCursorOnSprite = true;

    SpriteRenderer spriteRenderer;
    Collider2D spriteCollider;
    MaterialPropertyBlock propertyBlock;
    Camera worldCamera;

    static readonly int CursorPosId = Shader.PropertyToID("_CursorPos");
    static readonly int RadiusId = Shader.PropertyToID("_Radius");
    static readonly int SoftnessId = Shader.PropertyToID("_Softness");
    static readonly int LightTexId = Shader.PropertyToID("_LightTex");

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteCollider = GetComponent<Collider2D>();
        propertyBlock = new MaterialPropertyBlock();
        EnsureRevealMaterial();
    }

    void Start()
    {
        worldCamera = Camera.main;
        ApplyReveal(Vector2.zero, 0f);
    }

    void Update()
    {
        if (worldCamera == null)
            worldCamera = Camera.main;
        if (worldCamera == null)
            return;

        Vector3 world = ScreenToWorldOnSprite(Input.mousePosition);
        float revealRadius = radius;

        if (onlyWhenCursorOnSprite && !IsCursorOnSprite(world))
            revealRadius = 0f;

        ApplyReveal(world, revealRadius);
    }

    void EnsureRevealMaterial()
    {
        Material current = spriteRenderer.sharedMaterial;
        if (current != null && current.HasProperty(LightTexId))
            return;

        Shader shader = Shader.Find("Custom/SpriteCursorReveal");
        if (shader == null)
        {
            Debug.LogError("cursorlighter: could not find shader Custom/SpriteCursorReveal.", this);
            return;
        }

        spriteRenderer.material = new Material(shader);
    }

    void ApplyReveal(Vector2 worldPos, float revealRadius)
    {
        spriteRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetVector(CursorPosId, new Vector4(worldPos.x, worldPos.y, 0f, 0f));
        propertyBlock.SetFloat(RadiusId, Mathf.Max(0f, revealRadius));
        propertyBlock.SetFloat(SoftnessId, Mathf.Max(0.0001f, softness));

        Texture lightTex = lightForm != null ? lightForm.texture : null;
        if (lightTex != null)
            propertyBlock.SetTexture(LightTexId, lightTex);

        spriteRenderer.SetPropertyBlock(propertyBlock);
    }

    Vector3 ScreenToWorldOnSprite(Vector3 screenPos)
    {
        float depth = Mathf.Abs(worldCamera.transform.position.z - transform.position.z);
        screenPos.z = depth;
        Vector3 world = worldCamera.ScreenToWorldPoint(screenPos);
        world.z = transform.position.z;
        return world;
    }

    bool IsCursorOnSprite(Vector3 world)
    {
        if (spriteCollider != null)
            return spriteCollider.OverlapPoint(world);

        Bounds bounds = spriteRenderer.bounds;
        return world.x >= bounds.min.x && world.x <= bounds.max.x
            && world.y >= bounds.min.y && world.y <= bounds.max.y;
    }
}
