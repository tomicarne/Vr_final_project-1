using UnityEngine;

public class CulpaWord : MonoBehaviour
{
    [Header("Referencias")]
    public MirrorCrack mirrorCrack;
    public KeySpawner  keySpawner;

    [Header("Al resolver el puzzle")]
    public GameObject llave1;
    public GameObject notaLore;
    public GameObject piezaOjo;

    [Header("Feedback visual (opcional)")]
    public Renderer wordRenderer;
    public Color    selectedColor = Color.red;

    private bool _used = false;

    // ── Llama esto desde tu sistema VR al seleccionar la palabra ──
    public void OnCulpaSelected()
    {
        if (_used) return;
        _used = true;

        // Feedback visual: cambia color de la palabra
        if (wordRenderer != null)
            wordRenderer.material.color = selectedColor;

        // Activa el flash rojo y abre el compartimento en el espejo
        if (mirrorCrack != null)
            mirrorCrack.TriggerCulpa();

        // Spawna la llave junto a la puerta
        if (keySpawner != null)
            keySpawner.SpawnKey();

        // Activa llave, nota lore y pieza del ojo
        if (llave1 != null)   llave1.SetActive(true);
        if (notaLore != null) notaLore.SetActive(true);
        if (piezaOjo != null) piezaOjo.SetActive(true);

        // Marcar puzzle 1 como resuelto
        ProgressTracker.Instance?.SetPuzzleSolved(PuzzleID.Espejo);
    }

    public void OnTriggerEnter(Collider other)
    {
        OnCulpaSelected();
    }
}
