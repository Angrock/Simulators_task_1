using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    [Header("Настройки спавна")]
    public GameObject targetPrefab;
    public int maxTargets = 5;
    public float spawnInterval = 3f;
    public Vector3 spawnArea = new Vector3(20, 0, 20);

    [Header("Параметры мишеней")]
    public float targetMinMass = 0.3f;
    public float targetMaxMass = 2f;
    public float targetMinRadius = 0.2f;
    public float targetMaxRadius = 0.5f;
    public float flightHeight = 5f;

    private float spawnTimer;

    private void Update()
    {
        spawnTimer -= Time.deltaTime;
        
        if (spawnTimer <= 0 && CountTargets() < maxTargets)
        {
            SpawnTarget();
            spawnTimer = spawnInterval;
        }
    }

    private void SpawnTarget()
    {
        if (targetPrefab == null) return;

        Vector3 spawnPosition = new Vector3(
            Random.Range(-spawnArea.x, spawnArea.x),
            flightHeight,
            Random.Range(-spawnArea.z, spawnArea.z)
        );

        GameObject target = Instantiate(targetPrefab, spawnPosition, Quaternion.identity);
        Target targetScript = target.GetComponent<Target>();
        
        if (targetScript != null)
        {
            targetScript.minMass = targetMinMass;
            targetScript.maxMass = targetMaxMass;
            targetScript.minRadius = targetMinRadius;
            targetScript.maxRadius = targetMaxRadius;
            targetScript.flightHeight = flightHeight;
        }
    }

    private int CountTargets()
    {
        return GameObject.FindObjectsOfType<Target>().Length;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + Vector3.up * flightHeight, new Vector3(spawnArea.x * 2, 0.1f, spawnArea.z * 2));
    }
}