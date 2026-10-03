using UnityEngine;

public class rotate : MonoBehaviour
{
    [Tooltip("Degrees per second. Negative values reverse direction.")]
    [SerializeField] float speed = 40f;
    [SerializeField] Vector3 axis = new Vector3(0f, 0f, 1f);

    void Update()
    {
        transform.Rotate(axis.normalized, speed * Time.deltaTime, Space.Self);
    }
}
