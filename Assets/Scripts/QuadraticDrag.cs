using UnityEngine;

public class QuadraticDrag : MonoBehaviour
{
    public float mass = 1f;
    public float radius = 0.1f;
    public float dragCoefficient = 0.47f;
    public float airDensity = 1.225f;
    public Vector3 wind = Vector3.zero;

    private Rigidbody rb;
    private float area;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 vRel = rb.linearVelocity - wind;
        float speed = vRel.magnitude;
        if (speed < 1e-6f) return;

        Vector3 drag = -0.5f * airDensity * dragCoefficient * area * speed * vRel;
        rb.AddForce(drag, ForceMode.Force);
    }

    public void SetPhysicalParams(float mass, float radius, float dragCoefficient, float airDensity, Vector3 wind, Vector3 initialVelocity)
    {
        this.mass = Mathf.Max(0.001f, mass);
        this.radius = Mathf.Max(0.001f, radius);
        this.dragCoefficient = dragCoefficient;
        this.airDensity = airDensity;
        this.wind = wind;
        
        rb.mass = this.mass;
        rb.linearDamping = 0f; // Важно: отключаем линейное сопротивление
        rb.angularDamping = 0f;
        rb.useGravity = true;
        rb.linearVelocity = initialVelocity;
        
        area = Mathf.PI * radius * radius;
        transform.localScale = Vector3.one * (radius * 2f);
        
        Debug.Log($"Снаряд: масса={mass:F2}, радиус={radius:F2}, скорость={initialVelocity.magnitude:F2}");
    }
}