using UnityEngine;
using UnityEngine.UI;

public class MostrarInformacion : MonoBehaviour
{
    private GameObject imagenControles;

    void Buscar()
    {
        // No encuentro el objeto si no es con código, solucionar más tarde
        imagenControles = GameObject.Find("Imagen_Controles");
    }

    void Abrir()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (imagenControles != null)
                imagenControles.SetActive(true);
        });
    }
}