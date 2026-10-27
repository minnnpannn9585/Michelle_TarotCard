using UnityEngine;

public class StarTwinkle : MonoBehaviour
{
    [Tooltip("Smallest size, as a multiple of the object's scale.")]
    [SerializeField] float minScale = 0.85f;
    [Tooltip("Largest size, as a multiple of the object's scale.")]
    [SerializeField] float maxScale = 1.15f;
    [Tooltip("How fast the star grows and shrinks.")]
    [SerializeField] float speed = 2f;

    Vector3 baseScale;

    void Awake()
    {
        baseScale = transform.localScale;
    }

    void Update()
    {
        float t = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f;
        float scale = Mathf.Lerp(minScale, maxScale, t);
        transform.localScale = baseScale * scale;
    }
}
