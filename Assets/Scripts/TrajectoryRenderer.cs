using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrajectoryRenderer : MonoBehaviour
{
    [Header("Отрисовка")]
    public int pointsCount = 30;
    public float timeStep = 0.1f;
    public float widthLine = 0.02f;

    [Header("Физика воздуха")]
    public float mass = 1f;
    public float radius = 0.1f;
    public float dragCoefficient = 0.47f;
    public float airDensity = 1.225f;
    public Vector3 wind = Vector3.zero;

    [Header("Обрезка траектории")]
    public LayerMask collisionMask = -1; // С какими слоями сталкиваться
    public bool useSphereCast = true;
    private LineRenderer line;
    private float area;
    private bool isInitialized = false;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        line = GetComponent<LineRenderer>();

        area = Mathf.PI * radius * radius;
        isInitialized = true;
    }

    public void UpdateArea()
    {
        area = Mathf.PI * radius * radius;
    }

    public void DrawWithAirEuler(Vector3 startPosition, Vector3 startVelocity)
    {
        if (line == null) return;

        Vector3 p = startPosition;
        Vector3 v = startVelocity;
        
        Vector3[] points = new Vector3[pointsCount];
        points[0] = p;
        int actualPointsCount = pointsCount;

        for (int i = 1; i < pointsCount; i++)
        {
            Vector3 vRel = v - wind;
            float speed = vRel.magnitude;
            Vector3 drag = speed > 1e-6f ? 
                (-0.5f * airDensity * dragCoefficient * area * speed) * vRel : 
                Vector3.zero;
                
            Vector3 a = Physics.gravity + drag / mass;
            v += a * timeStep;
            Vector3 newP = p + v * timeStep;

            if (useSphereCast && CheckCollision(p, newP, radius, out RaycastHit hit))
            {
                points[i] = hit.point;
                actualPointsCount = i + 1;
                break;
            }

            p = newP;
            points[i] = p;
        }

        line.positionCount = actualPointsCount;
        for (int i = 0; i < actualPointsCount; i++)
        {
            line.SetPosition(i, points[i]);
        }
    }

    private bool CheckCollision(Vector3 from, Vector3 to, float sphereRadius, out RaycastHit hit)
    {
        Vector3 direction = to - from;
        float distance = direction.magnitude;
        
        if (distance > 0)
        {
            direction.Normalize();
            
            if (Physics.SphereCast(from, sphereRadius, direction, out hit, distance, collisionMask))
            {
                return true;
            }
        }
        
        hit = new RaycastHit();
        return false;
    }
}