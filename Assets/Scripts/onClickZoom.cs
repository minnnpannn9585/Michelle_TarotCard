using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class onClickZoom : MonoBehaviour
{
    [SerializeField] string nextSceneName = "Cutscene";
    [Tooltip("World point to center the camera on. If empty, Zoom Point is used.")]
    [SerializeField] Transform zoomTarget;
    [SerializeField] Vector3 zoomPoint;
    [Tooltip("How close the camera gets. Smaller orthographic size = more zoom.")]
    [SerializeField] float targetOrthoSize = 1.5f;
    [SerializeField] float zoomDuration = 1.2f;
    [SerializeField] Camera zoomCamera;

    Collider2D clickCollider2D;
    Collider clickCollider;
    bool zooming;

    void Awake()
    {
        clickCollider2D = GetComponent<Collider2D>();
        clickCollider = GetComponent<Collider>();

        if (zoomCamera == null)
            zoomCamera = Camera.main;
    }

    void Update()
    {
        if (zooming || !Input.GetMouseButtonDown(0))
            return;

        if (zoomCamera == null)
            zoomCamera = Camera.main;
        if (zoomCamera == null)
            return;

        if (IsClickInsideCollider())
            StartCoroutine(ZoomThenLoad());
    }

    bool IsClickInsideCollider()
    {
        Vector3 screen = Input.mousePosition;

        if (clickCollider2D != null)
        {
            float depth = Mathf.Abs(zoomCamera.transform.position.z - transform.position.z);
            screen.z = depth;
            Vector3 world = zoomCamera.ScreenToWorldPoint(screen);
            return clickCollider2D.OverlapPoint(world);
        }

        if (clickCollider != null)
        {
            Ray ray = zoomCamera.ScreenPointToRay(screen);
            return clickCollider.Raycast(ray, out _, 1000f);
        }

        return false;
    }

    Vector3 GetZoomWorldPoint()
    {
        return zoomTarget != null ? zoomTarget.position : zoomPoint;
    }

    IEnumerator ZoomThenLoad()
    {
        zooming = true;

        Vector3 startPos = zoomCamera.transform.position;
        Vector3 endPos = GetZoomWorldPoint();
        endPos.z = startPos.z;

        bool ortho = zoomCamera.orthographic;
        float startSize = ortho ? zoomCamera.orthographicSize : zoomCamera.fieldOfView;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.01f, zoomDuration);
            float eased = t * t * (3f - 2f * t);

            zoomCamera.transform.position = Vector3.Lerp(startPos, endPos, eased);
            if (ortho)
                zoomCamera.orthographicSize = Mathf.Lerp(startSize, targetOrthoSize, eased);
            else
                zoomCamera.fieldOfView = Mathf.Lerp(startSize, targetOrthoSize, eased);

            yield return null;
        }

        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
        else
            zooming = false;
    }
}
