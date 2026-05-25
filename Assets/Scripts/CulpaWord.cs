using UnityEngine;

public class CulpaWord : MonoBehaviour
{
    [Header("Referencias")]
    public MirrorCrack mirrorCrack;   // el script del espejo
    public KeySpawner  keySpawner;    // el script que spawna la llave

    [Header("Feedback visual (opcional)")]
    public Renderer wordRenderer;     // el renderer de la palabra
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
    }
}
