using UnityEngine;
using UnityEngine.Audio;

public class InicializadorAudio : MonoBehaviour
{
    [Header("Referencias")]
    public AudioMixer mesaDeMezclas;

    void Start()
    {
        // Leemos la memoria (si no hay nada guardado, ponemos 0.5f por defecto)
        float volGeneralGuardado = PlayerPrefs.GetFloat("VolumenMasterGuardado", 0.5f);
        float volMusicaGuardado = PlayerPrefs.GetFloat("VolumenMusicaGuardado", 0.5f);

        // Aplicamos directamente los valores a la mesa de mezclas convirtiéndolos a decibelios
        mesaDeMezclas.SetFloat("VolumenMaster", Mathf.Log10(volGeneralGuardado) * 20f);
        mesaDeMezclas.SetFloat("VolumenMusica", Mathf.Log10(volMusicaGuardado) * 20f);

        Debug.Log("Audio inicial cargado desde la memoria.");
    }
}