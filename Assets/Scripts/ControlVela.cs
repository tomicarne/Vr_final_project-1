using System;
using UnityEngine;
using System.Collections;

public class ControlVela : MonoBehaviour
{
    public bool velaPrendida = false;
    public int numeroVela = 0;
    public GameObject mecha;

    public static event Action<ControlVela> OnVelaPrendida;

    public void Start()
    {
        mecha = this.GetComponentInChildren<ParticleSystem>().gameObject;
    }

    public void Prender()
    {
        if (velaPrendida == true)
        {
            Debug.Log("ya estaba prendida");
        }
        else
        {
            velaPrendida = true;
            mecha.SetActive(true);
            OnVelaPrendida?.Invoke(this);
        }
    }

    public void Apagar()
    {
        if (velaPrendida == false)
        {
            Debug.Log("La vela ya estaba apagada");
        }
        else
        {
            StartCoroutine(ApagarConEfecto());
        }
    }

    private IEnumerator ApagarConEfecto()
    {
        velaPrendida = false; 
        yield return new WaitForSeconds(2f);
        mecha.SetActive(false);
    }
}