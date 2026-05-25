using UnityEngine;

/// <summary>
/// Añadir este componente a cualquier objeto interactuable de la escena.
/// En el Inspector, conectar los eventos de grab/activate del SDK de VR
/// al método NotifyInteraction().
///
/// Ejemplo con Meta XR SDK:
///   - OVRGrabbable → OnGrabBegin → InteractionNotifier.NotifyInteraction
///
/// Ejemplo con XR Interaction Toolkit:
///   - XRGrabInteractable → Select Entered → InteractionNotifier.NotifyInteraction
/// </summary>
[AddComponentMenu("Caranimas/Interaction Notifier")]
public class InteractionNotifier : MonoBehaviour
{
    public void NotifyInteraction()
    {
        HintSystem.Instance?.OnPlayerInteracted();
    }
}
