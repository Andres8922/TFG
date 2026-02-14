using UnityEngine;
using UnityEngine.UI;

public class ControlIconosFuncional : MonoBehaviour
{
    
    private Image imagenIcono;

    void Start()
    {
        
        imagenIcono = GameObject.Find("Icono_Actual")?.GetComponent<Image>();
    }

    // ----------------- Funciones Botones -----------

    public void MostrarLista()
    {
        // Busco objeto oculto
        GameObject[] todos = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject obj in todos)
        {
            if (obj.name == "Lista_iconos")
            {
                obj.SetActive(true);
                return;
            }
        }
    }

    // Cierro lista
    public void OcultarLista()
    {
        GameObject[] todos = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject obj in todos)
        {
            if (obj.name == "Lista_iconos")
            {
                obj.SetActive(false);
                return;
            }
        }
    }

    // SELECCIONAR Icono 1
    public void SeleccionarIcono1()
    {
        
        if (imagenIcono != null)
        {
            
            Sprite sprite = Resources.Load<Sprite>("Iconos/Icon_1");
            if (sprite != null)
            {
                imagenIcono.sprite = sprite;
            }
        }

        
        OcultarLista();
    }

    // SELECCIONAR Icono 2
    public void SeleccionarIcono2()
    {
        if (imagenIcono != null)
        {
            Sprite sprite = Resources.Load<Sprite>("Iconos/Icon_2");
            if (sprite != null) imagenIcono.sprite = sprite;
        }
        OcultarLista();
    }

    // SELECCIONAR Icono 3
    public void SeleccionarIcono3()
    {
        if (imagenIcono != null)
        {
            Sprite sprite = Resources.Load<Sprite>("Iconos/Icon_3");
            if (sprite != null) imagenIcono.sprite = sprite;
        }
        OcultarLista();
    }

    // SELECCIONAR Icono A
    public void SeleccionarIconoA()
    {
        if (imagenIcono != null)
        {
            Sprite sprite = Resources.Load<Sprite>("Iconos/Icon_A");
            if (sprite != null) imagenIcono.sprite = sprite;
        }
        OcultarLista();
    }

    // SELECCIONAR Icono 5
    public void SeleccionarIcono5()
    {
        if (imagenIcono != null)
        {
            Sprite sprite = Resources.Load<Sprite>("Iconos/Icon_5");
            if (sprite != null) imagenIcono.sprite = sprite;
        }
        OcultarLista();
    }

    // SELECCIONAR Icono 6
    public void SeleccionarIcono6()
    {
        if (imagenIcono != null)
        {
            Sprite sprite = Resources.Load<Sprite>("Iconos/Icon_6");
            if (sprite != null) imagenIcono.sprite = sprite;
        }
        OcultarLista();
    }
}