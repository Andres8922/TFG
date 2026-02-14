using UnityEngine;
using UnityEngine.SceneManagement;

public class Cambiar_pantallas : MonoBehaviour //Como no lo deribe no me deja pasar de una carpeta a otra el script
{
    
    public void VolverInicio()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }


    public void Pantalla_Ajustes()
    {
        SceneManager.LoadScene("Ajustes");
    }

    public void Pantalla_Perfil()
    {
        SceneManager.LoadScene("Perfil");
    }

    public void Pantalla_Info()
    {
        SceneManager.LoadScene("Info");
    }

    public void Pantalla_Dificultad() 
    { 
        SceneManager.LoadScene("Dificultad");
    }

    public void Pantalla_Heroe()
    {
        SceneManager.LoadScene("SeleccionHeroe");
    }

    public void Cerrar()
    {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

}
