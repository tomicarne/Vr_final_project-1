using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Enum compartido entre ProgressTracker, HintSystem, PadlockSystem y LoreManager.
/// Definirlo aquí evita duplicados; todos los otros scripts lo importan de este namespace.
/// </summary>
public enum PuzzleID
{
    None     = 0,   // Estado inicial / todos resueltos
    Espejo   = 1,   // Puzzle 1 — Verdad
    Velas    = 2,   // Puzzle 2 — Emoción
    Diario   = 3,   // Puzzle 3 — Miedo
    Silueta  = 4,   // Puzzle 4 — Identidad
}

/// <summary>
/// ProgressTracker — Caránimas VR
/// 
/// Fuente de verdad del progreso del jugador. Sabe:
///   - Qué puzzles están resueltos.
///   - Cuál es el puzzle activo (el más bajo no resuelto, en orden lineal).
/// 
/// Otros sistemas leen de aquí; nunca escriben directamente al estado.
///   - HintSystem.cs      → lee ActivePuzzle para saber hacia dónde apuntar el susurro.
///   - PadlockSystem.cs   → llama SetPuzzleSolved() al caer cada candado.
///   - LoreManager.cs     → se suscribe a OnPuzzleSolved para activar el lore.
///   - AudioManager.cs    → puede suscribirse a OnAllPuzzlesSolved para la música final.
/// 
/// Setup en Unity:
///   1. Añadir a un GameObject llamado "ProgressTracker" (singleton de escena).
///   2. Asignar en Inspector el orden lineal si se quiere cambiar (por defecto: 1-2-3-4).
/// </summary>
public class ProgressTracker : MonoBehaviour
{
    // ─── Singleton ────────────────────────────────────────────────────────────

    public static ProgressTracker Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // ─── Eventos públicos ─────────────────────────────────────────────────────

    /// <summary>
    /// Disparado cada vez que un puzzle se marca como resuelto.
    /// PadlockSystem y LoreManager deben suscribirse aquí.
    /// </summary>
    public event Action<PuzzleID> OnPuzzleSolved;

    /// <summary>
    /// Disparado cuando los 4 puzzles están resueltos.
    /// AudioManager puede suscribirse para la música/silencio del final.
    /// </summary>
    public event Action OnAllPuzzlesSolved;

    /// <summary>
    /// UnityEvent alternativo (para conectar desde el Inspector sin código).
    /// </summary>
    [Header("UnityEvent — alternativa visual al evento C#")]
    public UnityEvent<PuzzleID> onPuzzleSolvedUnityEvent;
    public UnityEvent           onAllPuzzlesSolvedUnityEvent;

    // ─── Estado interno ───────────────────────────────────────────────────────

    // true = resuelto
    private bool _espejaSolved  = false;
    private bool _velasSolved   = false;
    private bool _diarioSolved  = false;
    private bool _siluetaSolved = false;

    // ─── Propiedades de lectura ───────────────────────────────────────────────

    /// <summary>
    /// El puzzle activo es el primer puzzle no resuelto en orden narrativo (1→2→3→4).
    /// HintSystem lo usa para posicionar el susurro.
    /// Devuelve PuzzleID.None si todos están resueltos.
    /// </summary>
    public PuzzleID ActivePuzzle
    {
        get
        {
            if (!_espejaSolved)  return PuzzleID.Espejo;
            if (!_velasSolved)   return PuzzleID.Velas;
            if (!_diarioSolved)  return PuzzleID.Diario;
            if (!_siluetaSolved) return PuzzleID.Silueta;
            return PuzzleID.None;
        }
    }

    /// <summary>True si los 4 puzzles están resueltos.</summary>
    public bool AllSolved =>
        _espejaSolved && _velasSolved && _diarioSolved && _siluetaSolved;

    public bool EspejoSolved  => _espejaSolved;
    public bool VelasSolved   => _velasSolved;
    public bool DiarioSolved  => _diarioSolved;
    public bool SiluetaSolved => _siluetaSolved;

    // ─── API pública ──────────────────────────────────────────────────────────

    /// <summary>
    /// Marcar un puzzle como resuelto.
    /// Llamar desde el script de cada puzzle individual al completarse
    /// (MirrorPuzzle, CandlePuzzle, DiaryPuzzle, SilhouettePuzzle).
    /// 
    /// Es idempotente: llamar dos veces con el mismo puzzle no dispara el evento dos veces.
    /// </summary>
    public void SetPuzzleSolved(PuzzleID puzzle)
    {
        bool alreadySolved = IsSolved(puzzle);
        if (alreadySolved)
        {
            Debug.LogWarning($"[ProgressTracker] Puzzle {puzzle} ya estaba marcado como resuelto.");
            return;
        }

        Apply(puzzle);

        Debug.Log($"[ProgressTracker] Puzzle resuelto: {puzzle} | Activo ahora: {ActivePuzzle}");

        OnPuzzleSolved?.Invoke(puzzle);
        onPuzzleSolvedUnityEvent?.Invoke(puzzle);

        if (AllSolved)
        {
            Debug.Log("[ProgressTracker] Todos los puzzles resueltos. Activando final.");
            OnAllPuzzlesSolved?.Invoke();
            onAllPuzzlesSolvedUnityEvent?.Invoke();
        }
    }

    /// <summary>
    /// Devuelve true si el puzzle indicado ya fue resuelto.
    /// </summary>
    public bool IsSolved(PuzzleID puzzle)
    {
        return puzzle switch
        {
            PuzzleID.Espejo   => _espejaSolved,
            PuzzleID.Velas    => _velasSolved,
            PuzzleID.Diario   => _diarioSolved,
            PuzzleID.Silueta  => _siluetaSolved,
            _                 => false
        };
    }

    /// <summary>
    /// Resetea todo el progreso. Útil para playtesting sin reiniciar la escena.
    /// </summary>
    public void ResetAll()
    {
        _espejaSolved  = false;
        _velasSolved   = false;
        _diarioSolved  = false;
        _siluetaSolved = false;
        Debug.Log("[ProgressTracker] Progreso reseteado.");
    }

    // ─── Privado ──────────────────────────────────────────────────────────────

    private void Apply(PuzzleID puzzle)
    {
        switch (puzzle)
        {
            case PuzzleID.Espejo:   _espejaSolved  = true; break;
            case PuzzleID.Velas:    _velasSolved   = true; break;
            case PuzzleID.Diario:   _diarioSolved  = true; break;
            case PuzzleID.Silueta:  _siluetaSolved = true; break;
        }
    }

#if UNITY_EDITOR
    // ─── Debug en Editor ──────────────────────────────────────────────────────

    [ContextMenu("DEBUG — Resolver Puzzle 1 (Espejo)")]
    private void Debug_SolveEspejo()   => SetPuzzleSolved(PuzzleID.Espejo);

    [ContextMenu("DEBUG — Resolver Puzzle 2 (Velas)")]
    private void Debug_SolveVelas()    => SetPuzzleSolved(PuzzleID.Velas);

    [ContextMenu("DEBUG — Resolver Puzzle 3 (Diario)")]
    private void Debug_SolveDiario()   => SetPuzzleSolved(PuzzleID.Diario);

    [ContextMenu("DEBUG — Resolver Puzzle 4 (Silueta)")]
    private void Debug_SolveSilueta()  => SetPuzzleSolved(PuzzleID.Silueta);

    [ContextMenu("DEBUG — Resetear todo")]
    private void Debug_Reset()         => ResetAll();

    [ContextMenu("DEBUG — Ver estado actual")]
    private void Debug_PrintState()
    {
        Debug.Log($"[ProgressTracker] Estado actual:\n" +
                  $"  Espejo:  {_espejaSolved}\n" +
                  $"  Velas:   {_velasSolved}\n" +
                  $"  Diario:  {_diarioSolved}\n" +
                  $"  Silueta: {_siluetaSolved}\n" +
                  $"  Activo:  {ActivePuzzle}");
    }
#endif
}
