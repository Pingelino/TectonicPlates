using UnityEngine;

public class SphereCameraMovement : MonoBehaviour
{
    public Camera mainCamera;

    public float distance = 2f;

    private float zoomLevelTimer;
    private float initialZoomLevel;
    private float targetZoomLevel;
    public float zoomLevel = 25f;
    public float localDistanceForZoom = 0f;
    public float zoomSpeed = 1f;
    public float zoomMultiplier = 1f;
    public float zoomDropOffSpeed = 2.5f;
    public float zoomDropOffLevel = 2.5f;
    public Vector3 focusPoint;
    public Vector3 camOffsetDirection;

    public Vector2 currentMousePos;
    private Vector2 previousMousePos;

    public Vector3 initialFocusPoint;
    public Vector3 initialRayFocusPlaneIntersection;
    public void Start()
    {
        camOffsetDirection = (mainCamera.transform.position - focusPoint).normalized;
    }

    public bool mouseIsOutsideScreen;
    public void Update()
    {
        
        if(Input.mousePosition.x < 0 || Input.mousePosition.x > Screen.width)
            return;
        if(Input.mousePosition.y < 0 || Input.mousePosition.y > Screen.height)
            return;
        previousMousePos = currentMousePos;
        currentMousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        if(Input.GetMouseButtonDown(0))
            previousMousePos = currentMousePos;
        Rotate();
        Zoom();
        Move();
        UpdatePosition();
    }
    public void UpdatePosition()
    {
        UpdateZoomLevel();
        mainCamera.transform.position = focusPoint + camOffsetDirection * distance;
        mainCamera.transform.LookAt(focusPoint);
    }

    public void UpdateZoomLevel()
    {
        zoomLevelTimer += Time.deltaTime * zoomSpeed;
        zoomLevelTimer = Mathf.Clamp(zoomLevelTimer, 0, 1f);
        targetZoomLevel = zoomDropOffLevel * (2f * Mathf.Acos(-localDistanceForZoom * 0.5f) / Mathf.PI - 1f);
        float diff = targetZoomLevel - initialZoomLevel;
        zoomLevel = initialZoomLevel + diff * zoomLevelTimer;

        mainCamera.nearClipPlane = DistanceToNearClipPlane();
        distance = ZoomLevelToDistance();
        
    }
    public float DistanceToNearClipPlane()
    {
        float minZoomLevel = zoomDropOffLevel * (2f * Mathf.Acos(-1f * 0.5f) / Mathf.PI - 1f);
        float minSize = Mathf.Clamp(Mathf.Pow(zoomDropOffSpeed, minZoomLevel - 1f), 0.02f, 4f);

        float clipSize = Mathf.Pow(zoomDropOffSpeed, zoomLevel - 1f) / minZoomLevel * 0.02f;
        return Mathf.Clamp(clipSize, 0.02f, 4f);
    }
    public float ZoomLevelToDistance()
    {
        return 0.1f * mainCamera.nearClipPlane + Mathf.Pow(zoomDropOffSpeed, Mathf.Pow(zoomDropOffSpeed, zoomLevel));
        //float k = (zoomLevel + zoomDropOffLevel) / zoomLevel - 1f;
        //return 1f + mainCamera.nearClipPlane + 2f * zoomLevel * Mathf.Pow(zoomDropOffSpeed, -k * k);
    }
    
    public void Move()
    {
        if(Input.GetMouseButtonDown(2))
        {
            initialRayFocusPlaneIntersection = RayFocusPlaneIntersection();
            initialFocusPoint = focusPoint;
        }
        if(Input.GetMouseButton(2))
        {
            Vector3 currentRayFocusPlaneIntersection = RayFocusPlaneIntersection();
            Vector3 deltaIntersection = initialRayFocusPlaneIntersection - currentRayFocusPlaneIntersection;
            focusPoint = initialFocusPoint + deltaIntersection;
        }
    }

    public Vector3 RayFocusPlaneIntersection()
    {
        Vector3 rayDir = PixelToRay(currentMousePos);
        
        float nMagnitude = Vector3.Dot(camOffsetDirection, camOffsetDirection);
        float denom = Vector3.Dot(camOffsetDirection, rayDir);

        float t = -distance * nMagnitude / denom;

        Vector3 hitPoint = camOffsetDirection * distance + rayDir * t;
        return hitPoint;
    }
    public void Rotate()
    {
        if(!Input.GetMouseButton(0))
            return;
        float angleRangeVertical = mainCamera.fieldOfView * Mathf.PI / 180f;
        float d = Screen.height / (Mathf.Tan(angleRangeVertical / 2f) * 2f);
        float angleRangeHorizontal = Mathf.Atan(Screen.height / (2f * mainCamera.aspect * Mathf.PI / 180f) * d) * 2f;

        Vector2 deltaMousePos = currentMousePos - previousMousePos;
        Vector2 deltaDegrees = new Vector2(deltaMousePos.x / (float)Screen.width * angleRangeHorizontal / Mathf.PI * 180f, deltaMousePos.y / (float)Screen.height * angleRangeVertical / Mathf.PI * 180f);
        float deltaDegreesMagnitude = deltaDegrees.magnitude;

        Vector3 rayCurrent = PixelToRay(currentMousePos);
        Vector3 rayPrevious = PixelToRay(previousMousePos);

        Vector3 axisOfRotation = Vector3.Cross(rayPrevious, rayCurrent);

        camOffsetDirection = Quaternion.AngleAxis(deltaDegreesMagnitude, axisOfRotation) * (mainCamera.transform.position - focusPoint).normalized;
        //camPoint = focusPoint + localPos * distance;
        
    }

    private Vector3 PixelToRay(Vector2 mPos)
    {
        Vector3 pixelPointLocal = new Vector3(mPos.x / (float)Screen.width - 0.5f, mPos.y / (float)Screen.height - 0.5f, 1f);

		Vector4 pixelPoint = mainCamera.transform.localToWorldMatrix * new Vector4(pixelPointLocal.x, pixelPointLocal.y, pixelPointLocal.z, 1f);
        Vector3 rayOrigin = mainCamera.transform.position;
		Vector3 rayDir = (rayOrigin - new Vector3(pixelPoint.x, pixelPoint.y, pixelPoint.z)).normalized;

        return rayDir;
    }

    public void Zoom()
    {
        if(Input.mouseScrollDelta.y == 0)
            return;
        localDistanceForZoom -= Input.mouseScrollDelta.y * zoomMultiplier;
        localDistanceForZoom = Mathf.Clamp(localDistanceForZoom, -1f, 1f);

        if(zoomLevelTimer == 1f)
        {
            initialZoomLevel = zoomLevel;
            zoomLevelTimer = 0;
        }    
        
        
    }
}
