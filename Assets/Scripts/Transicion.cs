using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Transicion : MonoBehaviour
{
    public static Transicion Instance;
    public CanvasGroup telonNegro;
    public float velocidad = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        telonNegro.alpha = 1f;
        telonNegro.blocksRaycasts = true;
        StartCoroutine(FadeInInicial());
    }

    IEnumerator FadeInInicial()
    {
        yield return StartCoroutine(Fade(0));
        telonNegro.blocksRaycasts = false;
    }

    public void FadeSoloYAccion(System.Action accionEnNegro)
    {
        StopAllCoroutines();
        StartCoroutine(FadeAccion(accionEnNegro));
    }

    IEnumerator FadeAccion(System.Action accionEnNegro)
    {
        telonNegro.blocksRaycasts = true;
        yield return StartCoroutine(Fade(1));
        accionEnNegro?.Invoke();
        yield return StartCoroutine(Fade(0));
        telonNegro.blocksRaycasts = false;
    }

    public void CargarEscena(string nombreEscena)
    {
        // StopAllCoroutines evita que un FadeIn previo compita con la carga
        StopAllCoroutines();
        StartCoroutine(ProcesoCarga(nombreEscena));
    }

    IEnumerator ProcesoCarga(string escena)
    {
        telonNegro.blocksRaycasts = true;
        yield return StartCoroutine(Fade(1));
        SceneManager.LoadScene(escena);
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(Fade(0));
        telonNegro.blocksRaycasts = false;
    }

    IEnumerator Fade(float objetivoAlpha)
    {
        while (Mathf.Abs(telonNegro.alpha - objetivoAlpha) > 0.01f)
        {
            telonNegro.alpha = Mathf.MoveTowards(telonNegro.alpha, objetivoAlpha, velocidad * Time.deltaTime);
            yield return null;
        }
        telonNegro.alpha = objetivoAlpha;
    }
}
