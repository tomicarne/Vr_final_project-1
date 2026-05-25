using System.Collections;
using UnityEngine;
using TMPro;

public class MirrorCrack : MonoBehaviour
{
    [Header("Referencias del espejo")]
    public Renderer mirrorRenderer;

    [Header("Texto de condensación")]
    public CanvasGroup condensacionText;
    public float condensacionSpeed = 0.25f;

    [Header("Grietas")]
    public float crackDuration = 2.5f;

    [Header("Flash rojo")]
    public Light flashLight;

    [Header("Compartimento")]
    public GameObject compartmentDoor;

    private Material _mat;
    private static readonly int CrackAmount = Shader.PropertyToID("_CrackAmount");
    private bool _crackTriggered = false;

    void Awake()
    {
        // Obtiene una copia del material para no afectar otros objetos
        _mat = mirrorRenderer.material;

        // Asegura que empiece sin grietas
        _mat.SetFloat(CrackAmount, 0f);

        // Texto empieza invisible
        if (condensacionText != null)
            condensacionText.alpha = 0f;
    }

    // ── Trigger de proximidad ──────────────────────────────────
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            StartCoroutine(MostrarCondensacion());
    }

    // ── Texto de condensación (vaho) ───────────────────────────
    IEnumerator MostrarCondensacion()
    {
        if (condensacionText == null) yield break;

        float t = condensacionText.alpha;
        while (t < 1f)
        {
            t += Time.deltaTime * condensacionSpeed;
            condensacionText.alpha = Mathf.Clamp01(t);
            yield return null;
        }
    }

    // ── Llamar esto cuando el jugador encuentre CULPA ──────────
    public void TriggerCrack()
    {
        if (_crackTriggered) return;
        _crackTriggered = true;
        StartCoroutine(CrackSequence());
    }

    IEnumerator CrackSequence()
    {
        // 1. Agrieta desde el centro hacia afuera
        float t = 0f;
        while (t < crackDuration)
        {
            t += Time.deltaTime;
            float progress = Mathf.SmoothStep(0f, 1f, t / crackDuration);
            _mat.SetFloat(CrackAmount, progress);
            yield return null;
        }
        _mat.SetFloat(CrackAmount, 1f);

        // 2. Flash rojo breve
        yield return StartCoroutine(RedFlash());

        // 3. Abre el compartimento
        if (compartmentDoor != null)
            compartmentDoor.SetActive(true);
    }

    IEnumerator RedFlash()
    {
        if (flashLight != null)
        {
            flashLight.color     = Color.red;
            flashLight.intensity = 3f;
            flashLight.enabled   = true;
        }
        yield return new WaitForSeconds(0.15f);
        if (flashLight != null)
            flashLight.enabled = false;
    }
    public void TriggerCulpa()
    {
        StartCoroutine(CulpaSequence());
    }

    IEnumerator CulpaSequence()
    {
        // Flash rojo breve
        yield return StartCoroutine(RedFlash());

        // Abre el compartimento del marco
        if (compartmentDoor != null)
            compartmentDoor.SetActive(true);
    }
}