using UnityEngine;

// Poner este componente en cada objeto llave de la escena.
// Asignar en Inspector qué candado abre (debe coincidir con el PuzzleID del Padlock).
public class PadlockKey : MonoBehaviour
{
    [Tooltip("Qué candado abre esta llave.")]
    public PuzzleID abreCandado;

    [HideInInspector] public bool usada = false;
}
