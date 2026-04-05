using UnityEngine;
using UnityEngine.UI;

public class MenuAccionesManager : MonoBehaviour
{
    [Header("Paneles del Centro")]
    public GameObject textoEstado;
    public GameObject menuAtacar;
    public GameObject menuPociones;
    public GameObject menuPasivas; // Nuevo panel

    [Header("Botones Principales")]
    public Button btnLuchar;
    public Button btnPociones;
    public Button btnPasivas; // Nuevo botón principal

    [Header("Botones de Navegación Lateral")]
    public GameObject btnHuir;
    public GameObject btnVolver;

    void Start()
    {
        // Configuración de botones principales
        btnLuchar.onClick.AddListener(MostrarMenuAtacar);
        btnPociones.onClick.AddListener(MostrarMenuPociones);
        btnPasivas.onClick.AddListener(MostrarMenuPasivas);

        btnVolver.GetComponent<Button>().onClick.AddListener(VolverAlInicio);

        VolverAlInicio();
    }

    public void MostrarMenuAtacar() => ConfigurarInterfaz(menuAtacar);
    public void MostrarMenuPociones() => ConfigurarInterfaz(menuPociones);
    public void MostrarMenuPasivas() => ConfigurarInterfaz(menuPasivas);

    // Método optimizado para no repetir código
    private void ConfigurarInterfaz(GameObject menuActivo)
    {
        textoEstado.SetActive(false);
        menuAtacar.SetActive(false);
        menuPociones.SetActive(false);
        menuPasivas.SetActive(false);

        menuActivo.SetActive(true);
        AlternarBotonesNavegacion(true);
    }

    public void VolverAlInicio()
    {
        menuAtacar.SetActive(false);
        menuPociones.SetActive(false);
        menuPasivas.SetActive(false);

        textoEstado.SetActive(true);
        AlternarBotonesNavegacion(false);
    }

    private void AlternarBotonesNavegacion(bool enSubmenu)
    {
        if (btnHuir != null) btnHuir.SetActive(!enSubmenu);
        if (btnVolver != null) btnVolver.SetActive(enSubmenu);
    }
}