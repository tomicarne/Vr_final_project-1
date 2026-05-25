using UnityEngine;

public class KeySpawner : MonoBehaviour
{
    [Header("Prefab de la llave")]
    public GameObject keyPrefab;

    [Header("Impulso al spawnear (simula caída)")]
    public Vector3 spawnImpulse = new Vector3(0f, 0.8f, 0.2f);

    private bool _spawned = false;

    // ── Llamado por CulpaWord.OnCulpaSelected() ──
    public void SpawnKey()
    {
        if (_spawned) return;
        _spawned = true;

        // Instancia la llave en la posición del empty object
        GameObject key = Instantiate(keyPrefab, transform.position, transform.rotation);

        // Le aplica físicas si tiene Rigidbody
        Rigidbody rb = key.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.AddForce(spawnImpulse, ForceMode.Impulse);
        }
    }
}
