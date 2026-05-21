using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ControladorAudio : MonoBehaviour
{
    [Header("Referencias")]
    public AudioMixer mesaDeMezclas;
    public Slider sliderGeneral;
    public Slider sliderMusica;

    void Start()
    {
        sliderGeneral.minValue = 0.0001f;
        sliderGeneral.maxValue = 1f;
        sliderMusica.minValue = 0.0001f;
        sliderMusica.maxValue = 1f;

        sliderGeneral.value = PlayerPrefs.GetFloat("VolumenMasterGuardado", 0.5f);
        sliderMusica.value  = PlayerPrefs.GetFloat("VolumenMusicaGuardado", 0.5f);

        CambiarVolumenGeneral(sliderGeneral.value);
        CambiarVolumenMusica(sliderMusica.value);

        sliderGeneral.onValueChanged.AddListener(CambiarVolumenGeneral);
        sliderMusica.onValueChanged.AddListener(CambiarVolumenMusica);
    }

    public void CambiarVolumenGeneral(float valorSlider)
    {
        mesaDeMezclas.SetFloat("VolumenMaster", Mathf.Log10(valorSlider) * 20f);
        PlayerPrefs.SetFloat("VolumenMasterGuardado", valorSlider);
        PlayerPrefs.Save();
    }

    public void CambiarVolumenMusica(float valorSlider)
    {
        mesaDeMezclas.SetFloat("VolumenMusica", Mathf.Log10(valorSlider) * 20f);
        PlayerPrefs.SetFloat("VolumenMusicaGuardado", valorSlider);
        PlayerPrefs.Save();
    }
}
