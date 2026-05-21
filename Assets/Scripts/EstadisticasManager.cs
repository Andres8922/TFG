using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EstadisticasManager : MonoBehaviour
{
    [Header("Iconos de Héroe (índice = heroeSeleccionado)")]
    public GameObject[] iconosHeroes = new GameObject[4];

    [Header("Imagen Victoria / Derrota")]
    public GameObject imagenVictoria;
    public GameObject imagenDerrota;

    [Header("Textos de Estadísticas")]
    public TMP_Text textoDañoTotal;
    public TMP_Text textoManaTotal;
    public TMP_Text textoHoras;
    public TMP_Text textoMinutos;
    public TMP_Text textoTurnosTotales;
    public TMP_Text textoXPTotal;
    public TMP_Text textoOroTotal;

    void Start()
    {
        if (GameManager.Instance == null) return;

        for (int i = 0; i < iconosHeroes.Length; i++)
        {
            if (iconosHeroes[i] != null)
                iconosHeroes[i].SetActive(i == GameManager.Instance.heroeSeleccionado);
        }

        bool victoria = GameManager.Instance.victoriaJefe;
        if (imagenVictoria != null) imagenVictoria.SetActive(victoria);
        if (imagenDerrota != null)  imagenDerrota.SetActive(!victoria);

        float tiempo = GameManager.Instance.ObtenerTiempo();
        int horas    = (int)(tiempo / 3600);
        int minutos  = (int)((tiempo % 3600) / 60);
        if (textoHoras != null)   textoHoras.text   = horas.ToString("00");
        if (textoMinutos != null) textoMinutos.text = minutos.ToString("00");

        if (textoDañoTotal != null)    textoDañoTotal.text    = GameManager.Instance.dañoTotalPartida.ToString();
        if (textoManaTotal != null)    textoManaTotal.text    = GameManager.Instance.manaTotalPartida.ToString();
        if (textoTurnosTotales != null) textoTurnosTotales.text = GameManager.Instance.turnosTotalesPartida.ToString();
        if (textoXPTotal != null)      textoXPTotal.text      = GameManager.Instance.xpTotalPartida.ToString();
        if (textoOroTotal != null)     textoOroTotal.text     = GameManager.Instance.oroTotal.ToString();
    }
}
