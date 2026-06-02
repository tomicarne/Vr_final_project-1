using System.Collections;
using UnityEngine;

public class FinalManager : MonoBehaviour
{
    [Header("Pantalla negra")]
    public CanvasGroup pantallaNegra;
    public float velocidadFade = 0.5f;

    [Header("Audio")]
    public AudioSource musicaFondo;
    public AudioSource vozFinal1;
    public AudioSource vozFinal2;

    [Header("Final 2 — tiempo en el cuarto sin entrar (segundos)")]
    public float tiempoFinal2 = 120f;

    private bool _finalActivado = false;
    private float _timerCuarto = 0f;
    private bool _jugadorEnCuarto = false;

    void Update()
    {
        if (_finalActivado) return;

        if (_jugadorEnCuarto)
        {
            _timerCuarto += Time.deltaTime;
            if (_timerCuarto >= tiempoFinal2)
                ActivarFinal(2);
        }
    }

    // Llamar desde el trigger del cuarto principal
    public void JugadorEntroCuarto()
    {
        _jugadorEnCuarto = true;
    }

    public void JugadorSalioCuarto()
    {
        _jugadorEnCuarto = false;
    }

    // Llamar desde el trigger de la puerta nueva
    public void JugadorEntroPuerta()
    {
        ActivarFinal(1);
    }

    private void ActivarFinal(int numero)
    {
        if (_finalActivado) return;
        _finalActivado = true;
        StartCoroutine(SecuenciaFinal(numero));
    }

    IEnumerator SecuenciaFinal(int numero)
    {
        yield return StartCoroutine(FadeANegro());

        if (musicaFondo != null) musicaFondo.Play();

        yield return new WaitForSeconds(5f);

        if (numero == 1 && vozFinal1 != null) vozFinal1.Play();
        if (numero == 2 && vozFinal2 != null) vozFinal2.Play();
    }

    IEnumerator FadeANegro()
    {
        if (pantallaNegra == null) yield break;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * velocidadFade;
            pantallaNegra.alpha = Mathf.Clamp01(t);
            yield return null;
        }
    }
    
    #if UNITY_EDITOR
[ContextMenu("DEBUG — Simular Final A")]
private void Debug_FinaA() => ActivarFinal(1);

[ContextMenu("DEBUG — Simular Final B")]
private void Debug_FinalB() => ActivarFinal(2);
#endif
}
