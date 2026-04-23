using UnityEngine;
using System.Collections.Generic;

public class VelasManager : MonoBehaviour
{
    public List<ControlVela> todasLasVelas = new List<ControlVela>();

    private int siguienteVelaEsperada = 0;

    private void OnEnable()
    {
        ControlVela.OnVelaPrendida += OnVelaPrendida;
    }

    private void OnDisable()
    {
        ControlVela.OnVelaPrendida -= OnVelaPrendida;
    }

    private void OnVelaPrendida(ControlVela velaQueSeEncendio)
    {
        if (velaQueSeEncendio.numeroVela == siguienteVelaEsperada)
        {
            Debug.Log($"Vela {velaQueSeEncendio.numeroVela} prendida correctamente");
            siguienteVelaEsperada++;
            if (siguienteVelaEsperada >= todasLasVelas.Count)
            {
            }
        }
        else
        {
            ApagarTodas();
        }
    }

    private void ApagarTodas()
    {
        foreach (ControlVela vela in todasLasVelas)
        {
            vela.Apagar();
        }
        siguienteVelaEsperada = 0;
    }
}