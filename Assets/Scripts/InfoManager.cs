using UnityEngine;
using UnityEngine.UI;

public class InfoManager : MonoBehaviour
{
    [Header("Paneles de contenido")]
    public GameObject panelInfo;
    public GameObject panelCombate;
    public GameObject panelObjetos;
    public GameObject panelPoderes;
    public GameObject panelTienda;

    [Header("Botones")]
    public Button btnPoderes;
    public Button btnObjetos;
    public Button btnTienda;
    public Button btnCombate;

    private GameObject panelActivo = null;

    void Start()
    {
        btnPoderes.onClick.AddListener(() => MostrarPanel(panelPoderes));
        btnObjetos.onClick.AddListener(() => MostrarPanel(panelObjetos));
        btnCombate.onClick.AddListener(() => MostrarPanel(panelCombate));
        btnTienda.onClick.AddListener(() => MostrarPanel(panelTienda));

        OcultarTodos();
        panelInfo.SetActive(true); // Panel por defecto
    }

    void MostrarPanel(GameObject panelNuevo)
    {
        if (panelActivo == panelNuevo)
        {
            panelActivo.SetActive(false);
            panelActivo = null;
            panelInfo.SetActive(true); // Al cerrar, vuelve el panel por defecto
            return;
        }

        OcultarTodos();
        panelNuevo.SetActive(true);
        panelActivo = panelNuevo;
    }

    void OcultarTodos()
    {
        panelInfo.SetActive(false); // También se oculta el panel por defecto
        panelPoderes.SetActive(false);
        panelObjetos.SetActive(false);
        panelCombate.SetActive(false);
        panelTienda.SetActive(false);
    }
}