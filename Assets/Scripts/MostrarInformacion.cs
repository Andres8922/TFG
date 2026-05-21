using UnityEngine;
using UnityEngine.UI;

public class MostrarInformacion : MonoBehaviour
{
    private GameObject imagenControles;

    void Buscar()
    {
        // GameObject.Find no funciona con objetos inactivos, por eso se busca por código
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
