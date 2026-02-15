using UnityEngine;
using UnityEngine.SceneManagement;

public class Cambiar_pantallas : MonoBehaviour //Como no lo deribe no me deja pasar de una carpeta a otra el script
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

    public void Cerrar()
    {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

}
