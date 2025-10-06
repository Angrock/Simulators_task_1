using UnityEngine;

public class CannonController : MonoBehaviour
{
    [Header("Ссылки")]
    public Transform launchPoint;       // Launch Point (для позиции выстрела)
    public Transform cannonRoot;        // CannonRoot (двигается относительно направления)
    public Transform smallCannon;       // Small cannon (поворачивается по X)
    // CannonBase удален - он не нужен!
    
    [Header("Движение корня")]
    public float rootMoveSpeed = 5f;
    
    [Header("Поворот дула")]
    public float cannonRotateSpeed = 30f;
    public float minLaunchAngle = -10f;
    public float maxLaunchAngle = 80f;
    
    [Header("Поворот основания")]
    public float baseRotateSpeed = 90f;
    
    [Header("Выстрел")]
    public float initialSpeed = 20f;
    
    [Header("Случайные параметры снаряда")]
    public float minMass = 0.5f;
    public float maxMass = 3f;
    public float minRadius = 0.05f;
    public float maxRadius = 0.2f;
    
    [Header("Префабы")]
    public GameObject projectilePrefab;
    
    private TrajectoryRenderer trajectoryRenderer;
    private Vector3 currentVelocity;
    private float currentPreviewMass;
    private float currentPreviewRadius;
    private float currentLaunchAngle;

    private void Start()
    {
        trajectoryRenderer = GetComponent<TrajectoryRenderer>();
        currentLaunchAngle = 45f;
        GenerateNewPreviewParams();
        UpdateTrajectory();
    }

    private void Update()
    {
        HandleInput();
        
        if (trajectoryRenderer != null)
        {
            UpdateTrajectory();
        }
    }

    private void HandleInput()
    {
        HandleRootMovement();
        HandleCannonRotation();
        HandleBaseRotation();
        
        if (Input.GetKeyDown(KeyCode.Space)) Fire();
    }

    private void HandleRootMovement()
    {
        float moveForward = Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0;
        float moveRight = Input.GetKey(KeyCode.D) ? 1 : Input.GetKey(KeyCode.A) ? -1 : 0;
        
        Vector3 movement = (cannonRoot.forward * moveForward + cannonRoot.right * moveRight) * rootMoveSpeed * Time.deltaTime;
        cannonRoot.Translate(movement, Space.World);
    }

    private void HandleCannonRotation()
    {
        float rotateInput = 0f;
        
        if (Input.GetKey(KeyCode.UpArrow)) rotateInput = 1f;
        if (Input.GetKey(KeyCode.DownArrow)) rotateInput = -1f;
        
        currentLaunchAngle += rotateInput * cannonRotateSpeed * Time.deltaTime;
        currentLaunchAngle = Mathf.Clamp(currentLaunchAngle, minLaunchAngle, maxLaunchAngle);
        
        // Поворачиваем ТОЛЬКО Small cannon по локальной оси X
        if (smallCannon != null)
        {
            smallCannon.localEulerAngles = new Vector3(-currentLaunchAngle, 0, 0);
        }
    }

    private void HandleBaseRotation()
    {
        if (Input.GetKey(KeyCode.E)) cannonRoot.Rotate(0, baseRotateSpeed * Time.deltaTime, 0);
        if (Input.GetKey(KeyCode.Q)) cannonRoot.Rotate(0, -baseRotateSpeed * Time.deltaTime, 0);
    }

    private void GenerateNewPreviewParams()
    {
        currentPreviewMass = Random.Range(minMass, maxMass);
        currentPreviewRadius = Random.Range(minRadius, maxRadius);
    }

    private void UpdateTrajectory()
    {
        if (trajectoryRenderer == null || launchPoint == null) return;
        
        trajectoryRenderer.mass = currentPreviewMass;
        trajectoryRenderer.radius = currentPreviewRadius;
        trajectoryRenderer.UpdateArea();
        
        Vector3 direction = launchPoint.forward;
        currentVelocity = direction * initialSpeed;
        
        trajectoryRenderer.DrawWithAirEuler(launchPoint.position, currentVelocity);
    }

    private void Fire()
{
    if (projectilePrefab == null || trajectoryRenderer == null || launchPoint == null) return;
    
    GameObject projectile = Instantiate(projectilePrefab, launchPoint.position, launchPoint.rotation);
    
    QuadraticDrag quadraticDrag = projectile.GetComponent<QuadraticDrag>();
    if (quadraticDrag != null)
    {
        quadraticDrag.SetPhysicalParams(
            currentPreviewMass,
            currentPreviewRadius, 
            trajectoryRenderer.dragCoefficient, 
            trajectoryRenderer.airDensity, 
            trajectoryRenderer.wind, 
            currentVelocity
        );
    }
    
    if (GameManager.Instance != null)
    {
        GameManager.Instance.RegisterShot();
    }
    
    Debug.Log($"Выстрел: масса={currentPreviewMass:F2}, радиус={currentPreviewRadius:F2}, угол={currentLaunchAngle:F1}°");
    GenerateNewPreviewParams();
}

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 20), $"Угол дула: {currentLaunchAngle:F1}°");
        GUI.Label(new Rect(10, 30, 300, 20), $"Масса снаряда: {currentPreviewMass:F2}");
        GUI.Label(new Rect(10, 50, 300, 20), $"Радиус снаряда: {currentPreviewRadius:F2}");
    }
}