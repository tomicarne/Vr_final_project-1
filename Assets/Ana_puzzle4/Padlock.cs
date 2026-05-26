using UnityEngine;
using System.Collections;

// Poner este componente en cada GameObject de candado.
// Requiere un Collider en modo Trigger que cubra la zona del ojo del candado (el hueco donde entra la llave).
// Requiere un Rigidbody en el mismo GameObject (empieza kinematic, se suelta al abrirse).
public class Padlock : MonoBehaviour
{
    [Header("Identidad")]
    [Tooltip("Debe coincidir con el PuzzleID de la llave que lo abre.")]
    public PuzzleID puzzleID;

    [Header("Grillete (la parte en U que se abre)")]
    [Tooltip("Transform del arco superior del candado.")]
    public Transform grillete;
    public Vector3 rotacionGrilleteAbierto = new Vector3(90f, 0f, 0f);

    [Header("Audio")]
    [Tooltip("Sonido metálico único para este candado.")]
    public AudioClip sonidoApertura;

    // Evento que escucha PadlockSystem (y en el futuro LoreManager).
    public static event System.Action<Padlock> OnCandadoAbierto;

    private bool _abierto = false;
    private AudioSource _audio;
    private Rigidbody _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb != null) _rb.isKinematic = true;

        _audio = gameObject.AddComponent<AudioSource>();
        _audio.playOnAwake  = false;
        _audio.spatialBlend = 1f;
    }

    void OnTriggerEnter(Collider other)
    {
        if (_abierto) return;

        PadlockKey llave = other.GetComponent<PadlockKey>();
        if (llave == null || llave.usada) return;
        if (llave.abreCandado != puzzleID) return;

        llave.usada = true;
        llave.gameObject.SetActive(false);

        StartCoroutine(SecuenciaApertura());
    }

    private IEnumerator SecuenciaApertura()
    {
        _abierto = true;

        // Sonido metálico
        if (sonidoApertura != null)
            _audio.PlayOneShot(sonidoApertura);

        // Animación del grillete abriéndose
        if (grillete != null)
        {
            Quaternion inicio  = grillete.localRotation;
            Quaternion destino = Quaternion.Euler(rotacionGrilleteAbierto);
            float elapsed = 0f, duracion = 0.6f;

            while (elapsed < duracion)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / duracion);
                grillete.localRotation = Quaternion.Slerp(inicio, destino, t);
                yield return null;
            }
        }

        yield return new WaitForSeconds(0.3f);

        // Caída física al suelo
        transform.SetParent(null);
        if (_rb != null) _rb.isKinematic = false;

        OnCandadoAbierto?.Invoke(this);

        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    [ContextMenu("DEBUG — Abrir candado ahora")]
    private void Debug_Abrir() => StartCoroutine(SecuenciaApertura());
#endif
}
