using UnityEngine;

public class Tela : MonoBehaviour
{
    public MirrorCrack mirrorCrack;

    // Llamar desde el evento del XR Interactable (OnSelectEntered)
    public void AlTocar()
    {
        if (mirrorCrack != null)
            mirrorCrack.TriggerCrack();

        gameObject.SetActive(false);
    }
}
