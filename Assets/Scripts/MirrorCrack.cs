using System.Collections;
using UnityEngine;
using TMPro;

public class MirrorCrack : MonoBehaviour
{
    [Header("Referencias del espejo")]
    public Renderer mirrorRenderer;

    [Header("Texto de condensación")]
    public CanvasGroup condensacionText;
    public TextMeshProUGUI textoVaho;
    public float condensacionSpeed = 0.25f;

    [Header("Texto después de romperse")]
    public string textoPostCrack = "La sombra siempre habla al revés.";

    [Header("Grietas")]
    public float crackDuration = 2.5f;

    [Header("Flash rojo")]
    public Light flashLight;

    [Header("Compartimento")]
    public GameObject compartmentDoor;

    [Header("Palabras — se activan DESPUÉS de que el espejo se parte")]
    public GameObject[] palabrasInteractivas;

    private Material _mat;
    private static readonly int CrackAmount = Shader.PropertyToID("_CrackAmount");
    private bool _crackTriggered = false;

    void Awake()
    {
        _mat = mirrorRenderer.material;
        _mat.SetFloat(CrackAmount, 0f);

        if (condensacionText != null)
            condensacionText.alpha = 0f;

        // Las palabras empiezan desactivadas
        foreach (var palabra in palabrasInteractivas)
            if (palabra != null) palabra.SetActive(false);
    }

    // ── Jugador se acerca al espejo ────────────────────────────
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        StartCoroutine(MostrarCondensacion());
    }

    // ── Texto de vaho ──────────────────────────────────────────
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

    // ── Grietas al acercarse ───────────────────────────────────
    public void TriggerCrack()
    {
        if (_crackTriggered) return;
        _crackTriggered = true;
        Debug.Log("[Espejo] TriggerCrack llamado");
        StartCoroutine(CrackSequence());
    }

    IEnumerator CrackSequence()
    {
        // 1. Anima las grietas
        float t = 0f;
        while (t < crackDuration)
        {
            t += Time.deltaTime;
            _mat.SetFloat(CrackAmount, Mathf.SmoothStep(0f, 1f, t / crackDuration));
            yield return null;
        }
        _mat.SetFloat(CrackAmount, 1f);

        // 2. Cambia el texto del vaho a la pista
        if (textoVaho != null)
            textoVaho.text = textoPostCrack;

        // 3. Activa las palabras interactuables
        foreach (var palabra in palabrasInteractivas)
            if (palabra != null) palabra.SetActive(true);
    }

    // ── Llamado por CulpaWord cuando el jugador toca CULPA ─────
    public void TriggerCulpa()
    {
        StartCoroutine(CulpaSequence());
    }

    IEnumerator CulpaSequence()
    {
        yield return StartCoroutine(RedFlash());

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
    

 #if UNITY_EDITOR
[ContextMenu("DEBUG — Reset y Crack")]
private void Debug_ResetCrack()
{
    _crackTriggered = false;
    TriggerCrack();
}

[ContextMenu("DEBUG — Trigger Culpa")]
private void Debug_Culpa()
{
    StartCoroutine(CulpaSequence());
}
#endif
}
