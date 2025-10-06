using UnityEngine;
using System.Collections;

public class Target : MonoBehaviour
{
    [Header("Параметры мишени")]
    public float minMass = 0.3f;
    public float maxMass = 2f;
    public float minRadius = 0.2f;
    public float maxRadius = 0.5f;
    public float moveSpeed = 3f;
    public float flightHeight = 3f;
    public float movementRadius = 8f;
    public int scoreValue = 100;

    private Rigidbody rb;
    private float currentMass;
    private float currentRadius;
    private Vector3 centerPoint;
    private float angle;
    private float flightDirection;
    private bool isHit = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        centerPoint = transform.position;
        InitializeTarget();
    }

    private void InitializeTarget()
    {
        currentMass = Random.Range(minMass, maxMass);
        currentRadius = Random.Range(minRadius, maxRadius);
        
        rb.mass = currentMass;
        transform.localScale = Vector3.one * (currentRadius * 2f);
        
        angle = Random.Range(0f, 360f);
        flightDirection = Random.value > 0.5f ? 1f : -1f;
        
        rb.useGravity = false;
        rb.isKinematic = true;
        
        Vector3 pos = transform.position;
        pos.y = flightHeight;
        transform.position = pos;
    }

    private void Update()
    {
        if (!isHit)
        {
            FlyInCircle();
        }
    }

    private void FlyInCircle()
    {
        angle += flightDirection * moveSpeed * Time.deltaTime;
        
        float x = Mathf.Cos(angle) * movementRadius;
        float z = Mathf.Sin(angle) * movementRadius;
        
        Vector3 targetPosition = centerPoint + new Vector3(x, flightHeight, z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isHit && collision.gameObject.CompareTag("Projectile"))
        {
            Debug.Log("ПОПАДАНИЕ ЗАРЕГИСТРИРОВАНО!");
            isHit = true;
            OnTargetHit();
        }
    }

    private void OnTargetHit()
    {

        rb.isKinematic = false;
        rb.useGravity = true;
        
        rb.angularVelocity = new Vector3(
            Random.Range(-5f, 5f),
            Random.Range(-5f, 5f),
            Random.Range(-5f, 5f)
        );
        
        rb.AddForce(new Vector3(
            Random.Range(-2f, 2f),
            Random.Range(1f, 3f),
            Random.Range(-2f, 2f)
        ), ForceMode.Impulse);
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue, currentMass, currentRadius);
        }
        
        Debug.Log($"Попадание! Мишень падает: масса={currentMass:F2}, радиус={currentRadius:F2}");
        
        StartCoroutine(DestroyAfterFall());
    }

    private IEnumerator DestroyAfterFall()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(centerPoint, movementRadius);
    }
}