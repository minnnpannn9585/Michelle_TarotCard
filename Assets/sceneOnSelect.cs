using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneOnSelect : MonoBehaviour
{
    [SerializeField] string nextSceneName = "StartGame";

    Collider2D box2D;
    Collider box3D;
    Camera worldCamera;

    void Awake()
    {
        box2D = GetComponent<Collider2D>();
        if (box2D == null)
            box2D = GetComponentInChildren<Collider2D>();

        box3D = GetComponent<Collider>();
        worldCamera = Camera.main;
    }

    void Update()
    {
        if (!Input.GetMouseButtonDown(0) || string.IsNullOrEmpty(nextSceneName))
            return;

        if (worldCamera == null)
            worldCamera = Camera.main;
        if (worldCamera == null)
            return;

        if (IsClickInsideCollider())
            SceneManager.LoadScene(nextSceneName);
    }

    bool IsClickInsideCollider()
    {
        Vector3 screen = Input.mousePosition;

        if (box2D != null)
        {
            float depth = Mathf.Abs(worldCamera.transform.position.z - box2D.transform.position.z);
            screen.z = depth;
            Vector3 world = worldCamera.ScreenToWorldPoint(screen);
            return box2D.OverlapPoint(world);
        }

        if (box3D != null)
        {
            Ray ray = worldCamera.ScreenPointToRay(screen);
            return box3D.Raycast(ray, out _, 1000f);
        }

        return false;
    }
}
