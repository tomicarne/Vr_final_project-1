using System;
using UnityEngine;
using System.Collections;

public class ControlVela : MonoBehaviour
{
    public bool velaPrendida = false;
    public int numeroVela = 0;
    public GameObject mecha;

    [Header("Estado derretida (solo para la vela del miedo)")]
    public bool esVelaDelMiedo = false;
    public GameObject modeloNormal;    // el GameObject del modelo entero
    public GameObject modeloDerretido; // el GameObject del modelo derretido
    public GameObject notaLore;
    public GameObject llave;
    public GameObject piezaBoca;

    public static event Action<ControlVela> OnVelaPrendida;

    public void Start()
    {
        if (mecha == null)
        {
            var ps = GetComponentInChildren<ParticleSystem>();
            if (ps != null) mecha = ps.gameObject;
        }
    }

    public void Prender()
    {
        if (velaPrendida) return;
        velaPrendida = true;
        mecha.SetActive(true);
        OnVelaPrendida?.Invoke(this);
    }

    public void Apagar()
    {
        if (!velaPrendida) return;
        StartCoroutine(ApagarConEfecto());
    }

    public void Derretir()
    {
        velaPrendida = false;
        if (mecha != null) mecha.SetActive(false);
        if (modeloNormal != null)    modeloNormal.SetActive(false);
        if (modeloDerretido != null) modeloDerretido.SetActive(true);
        if (notaLore != null)        notaLore.SetActive(true);
        if (llave != null)           llave.SetActive(true);
        if (piezaBoca != null)       piezaBoca.SetActive(true);
    }

    private IEnumerator ApagarConEfecto()
    {
        velaPrendida = false;
        yield return new WaitForSeconds(2f);
        if (mecha != null) mecha.SetActive(false);
    }
}