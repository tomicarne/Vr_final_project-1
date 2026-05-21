using System.Collections;
using UnityEngine;

/// <summary>
/// HintSystem — Caránimas VR
/// 
/// Si el jugador no interactúa con ningún objeto durante HINT_DELAY segundos (default: 300 = 5 min),
/// reproduce un AudioClip de la Sombra como audio espacial 3D posicionado en la dirección
/// del puzzle activo. No hay texto, no hay flecha.
/// 
/// Dependencias:
///   - ProgressTracker.cs  → informa cuál puzzle está activo
///   - AudioManager.cs     → reproduce el clip (opcional: si no existe, usa AudioSource local)
/// 
/// Setup en Unity:
///   1. Añadir este componente a un GameObject vacío llamado "HintSystem" en la escena.
///   2. Asignar en Inspector los 4 PuzzleAnchor (Transform) y los 4 AudioClip de la Sombra.
///   3. Asignar referencia a ProgressTracker.
///   4. (Opcional) Asignar referencia a AudioManager si Angel ya lo tiene implementado.
/// </summary>
public class HintSystem : MonoBehaviour
{
    // ─── Configuración ────────────────────────────────────────────────────────

    [Header("Tiempo de inactividad antes del hint (segundos)")]
    [SerializeField] private float hintDelay = 300f;   // 5 minutos

    [Header("Tiempo mínimo entre hints consecutivos (segundos)")]
    [SerializeField] private float cooldownBetweenHints = 60f;

    // ─── Referencias de escena ────────────────────────────────────────────────

    [Header("Transform de cada puzzle (posición en el cuarto)")]
    [Tooltip("El AudioSource del hint se moverá aquí antes de reproducir.")]
    [SerializeField] private Transform puzzleAnchor_Espejo;    // Puzzle 1 — Norte
    [SerializeField] private Transform puzzleAnchor_Velas;     // Puzzle 2 — Este
    [SerializeField] private Transform puzzleAnchor_Diario;    // Puzzle 3 — Sur
    [SerializeField] private Transform puzzleAnchor_Silueta;   // Puzzle 4 — Oeste

    [Header("AudioClips de la Sombra (uno por puzzle)")]
    [Tooltip("Clips de voz — grabados en tono suave, íntimo, sin reverb excesivo.")]
    [SerializeField] private AudioClip hintClip_Espejo;
    [SerializeField] private AudioClip hintClip_Velas;
    [SerializeField] private AudioClip hintClip_Diario;
    [SerializeField] private AudioClip hintClip_Silueta;

    [Header("Referencias a otros sistemas")]
    [SerializeField] private ProgressTracker progressTracker;

    /// <summary>
    /// AudioManager de Angel (opcional).
    /// Si está asignado, el hint se delega a AudioManager.PlaySpatialOneShot().
    /// Si no, HintSystem usa su propio AudioSource interno.
    /// </summary>
    [SerializeField] private AudioManager audioManager;   // puede quedar null

    [Header("Volumen y mezcla espacial del hint")]
    [Range(0f, 1f)]
    [SerializeField] private float hintVolume    = 0.8f;
    [Range(0f, 1f)]
    [SerializeField] private float spatialBlend  = 1f;    // 1 = audio 3D puro
    [SerializeField] private float maxDistance   = 12f;   // cubre la habitación 5×5

    // ─── Estado interno ───────────────────────────────────────────────────────

    private float   _inactivityTimer   = 0f;
    private float   _cooldownTimer     = 0f;
    private bool    _hintsEnabled      = true;
    private bool    _isOnCooldown      = false;

    // AudioSource propio (fallback si no hay AudioManager)
    private AudioSource _localSource;

    // ─── Unity Lifecycle ──────────────────────────────────────────────────────

    private void Awake()
    {
        // Crear AudioSource local como fallback
        _localSource                  = gameObject.AddComponent<AudioSource>();
        _localSource.playOnAwake      = false;
        _localSource.spatialBlend     = spatialBlend;
        _localSource.maxDistance      = maxDistance;
        _localSource.rolloffMode      = AudioRolloffMode.Logarithmic;
        _localSource.volume           = hintVolume;
    }

    private void OnEnable()
    {
        ResetInactivityTimer();
    }

    private void Update()
    {
        if (!_hintsEnabled) return;

        // Cooldown entre hints consecutivos
        if (_isOnCooldown)
        {
            _cooldownTimer += Time.deltaTime;
            if (_cooldownTimer >= cooldownBetweenHints)
            {
                _isOnCooldown  = false;
                _cooldownTimer = 0f;
            }
            return; // no acumular inactividad durante cooldown
        }

        _inactivityTimer += Time.deltaTime;

        if (_inactivityTimer >= hintDelay)
        {
            TriggerHint();
        }
    }

    // ─── API pública ──────────────────────────────────────────────────────────

    /// <summary>
    /// Llamar cada vez que el jugador interactúa con cualquier objeto.
    /// Ej: desde HandInteraction.cs, en OnGrab() o OnActivate().
    /// </summary>
    public void OnPlayerInteracted()
    {
        ResetInactivityTimer();
    }

    /// <summary>
    /// Pausar hints (ej: durante una cinemática de lore o menú de pausa).
    /// </summary>
    public void SetHintsEnabled(bool enabled)
    {
        _hintsEnabled = enabled;
        if (enabled) ResetInactivityTimer();
    }

    // ─── Lógica interna ───────────────────────────────────────────────────────

    private void ResetInactivityTimer()
    {
        _inactivityTimer = 0f;
    }

    private void TriggerHint()
    {
        PuzzleID activePuzzle = progressTracker != null
            ? progressTracker.ActivePuzzle
            : PuzzleID.None;

        if (activePuzzle == PuzzleID.None)
        {
            // Todos los puzzles resueltos: no hay hint que dar
            ResetInactivityTimer();
            return;
        }

        Transform anchor = GetAnchorForPuzzle(activePuzzle);
        AudioClip clip   = GetClipForPuzzle(activePuzzle);

        if (anchor == null || clip == null)
        {
            Debug.LogWarning($"[HintSystem] Anchor o clip nulo para puzzle {activePuzzle}. " +
                             "Verifica el Inspector.");
            ResetInactivityTimer();
            return;
        }

        PlaySpatialHint(anchor.position, clip);

        ResetInactivityTimer();
        _isOnCooldown  = true;
        _cooldownTimer = 0f;

        Debug.Log($"[HintSystem] Hint reproducido para puzzle: {activePuzzle}");
    }

    /// <summary>
    /// Reproduce el clip en la posición del puzzle activo.
    /// Si AudioManager está disponible, lo delega; de lo contrario usa _localSource.
    /// </summary>
    private void PlaySpatialHint(Vector3 worldPosition, AudioClip clip)
    {
        // Delegar a AudioManager de Angel si existe
        if (audioManager != null)
        {
            audioManager.PlaySpatialHint(worldPosition, clip, hintVolume);
            return;
        }

        // Fallback: mover el AudioSource local a la posición del puzzle y reproducir
        _localSource.transform.position = worldPosition;
        _localSource.clip               = clip;
        _localSource.Play();
    }

    private Transform GetAnchorForPuzzle(PuzzleID puzzle)
    {
        return puzzle switch
        {
            PuzzleID.Espejo   => puzzleAnchor_Espejo,
            PuzzleID.Velas    => puzzleAnchor_Velas,
            PuzzleID.Diario   => puzzleAnchor_Diario,
            PuzzleID.Silueta  => puzzleAnchor_Silueta,
            _                 => null
        };
    }

    private AudioClip GetClipForPuzzle(PuzzleID puzzle)
    {
        return puzzle switch
        {
            PuzzleID.Espejo   => hintClip_Espejo,
            PuzzleID.Velas    => hintClip_Velas,
            PuzzleID.Diario   => hintClip_Diario,
            PuzzleID.Silueta  => hintClip_Silueta,
            _                 => null
        };
    }

#if UNITY_EDITOR
    // ─── Debug en Editor ──────────────────────────────────────────────────────
    // Permite probar el hint manualmente desde el Inspector sin esperar 5 minutos.

    [ContextMenu("DEBUG — Forzar hint ahora")]
    private void Debug_ForceHint()
    {
        _isOnCooldown    = false;
        _inactivityTimer = hintDelay;
    }
#endif
}
