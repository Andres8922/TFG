using UnityEngine;

public class EstadisticasBotones : MonoBehaviour
{
    public void BotonSalir()
    {
        if (GameManager.Instance != null) GameManager.Instance.ResetearPartida();
        Transicion.Instance.CargarEscena("MenuPrincipal");
    }

    public void BotonJugarDeNuevo()
    {
        if (GameManager.Instance != null) GameManager.Instance.ResetearPartida();
        Transicion.Instance.CargarEscena("Dificultad");
    }
}
