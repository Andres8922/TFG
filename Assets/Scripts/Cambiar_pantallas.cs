using UnityEngine;
using UnityEngine.SceneManagement;

public class Cambiar_pantallas : MonoBehaviour 
{
    public void VolverInicio()
    {
        Transicion.Instance.CargarEscena("MenuPrincipal");
    }

    public void Pantalla_Ajustes()
    {
        Transicion.Instance.CargarEscena("Ajustes");
    }

    public void Pantalla_Perfil()
    {
        Transicion.Instance.CargarEscena("Perfil");
    }
    
    public void Pantalla_Info()
    {
        Transicion.Instance.CargarEscena("Info");
    }

    public void Pantalla_Dificultad() 
    {
        Transicion.Instance.CargarEscena("Dificultad");
    }

    public void Pantalla_Heroe()
    {
        Transicion.Instance.CargarEscena("SeleccionHeroe");
    }

    public void Pantalla_Mapa()
    {
        Transicion.Instance.CargarEscena("Mapa"); 
    }

    public void Cerrar()
    {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}