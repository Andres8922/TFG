using UnityEngine;

public class EfectosBoton : MonoBehaviour
{
    [Header("Configuración de Sonido")]
    public AudioSource altavozSFX;
    public AudioClip sonidoClic;

    public void ReproducirClic()
    {
        if (altavozSFX != null && sonidoClic != null)
        {
            altavozSFX.PlayOneShot(sonidoClic);
        }
    }
}