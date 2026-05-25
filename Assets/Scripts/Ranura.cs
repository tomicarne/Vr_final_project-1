// Ranura.cs
using UnityEngine;

public class Ranura : MonoBehaviour
{
    [Header("Posición en la palabra (0=M, 1=I, 2=E, 3=D, 4=O)")]
    public int indice;

    private Papel papelActual = null;

    public char? LetraActual => papelActual != null ? papelActual.letra : (char?)null;

    private void OnTriggerEnter(Collider other)
    {
        Papel papel = other.GetComponent<Papel>();
        if (papel != null)
        {
            papelActual = papel;
            PuzzleManager.Instance.VerificarPuzzle();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Papel papel = other.GetComponent<Papel>();
        if (papel != null && papel == papelActual)
        {
            papelActual = null;
            PuzzleManager.Instance.VerificarPuzzle();
        }
    }

    public bool TieneLetraCorrecta()
    {
        string palabra = "MIEDO";
        if (papelActual == null) return false;
        return char.ToUpper(papelActual.letra) == palabra[indice];
    }
}
