using UnityEngine;
using UnityEngine.Audio;

public class InicializadorAudio : MonoBehaviour
{
    [Header("Referencias")]
    public AudioMixer mesaDeMezclas;

    void Start()
    {
        float volGeneral = PlayerPrefs.GetFloat("VolumenMasterGuardado", 0.5f);
        float volMusica  = PlayerPrefs.GetFloat("VolumenMusicaGuardado", 0.5f);

        // Log10 convierte el valor lineal del slider a decibelios
        mesaDeMezclas.SetFloat("VolumenMaster", Mathf.Log10(volGeneral) * 20f);
        mesaDeMezclas.SetFloat("VolumenMusica",  Mathf.Log10(volMusica)  * 20f);
    }
}
