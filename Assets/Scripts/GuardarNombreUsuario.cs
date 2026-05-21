using UnityEngine;
using TMPro;

public class GuardarNombreUsuario : MonoBehaviour
{
    private TMP_InputField campo;

    void Start()
    {
        // FindObjectsOfType porque el InputField no siempre está en la jerarquía directa
        GameObject[] todos = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in todos)
        {
            if (obj.GetComponent<TMP_InputField>() != null)
            {
                campo = obj.GetComponent<TMP_InputField>();
                break;
            }
        }

        if (campo == null)
        {
            Debug.LogError("No se encontró TMP_InputField en la escena.");
        }
        else
        {
            if (PlayerPrefs.HasKey("NombreUsuario"))
                campo.text = PlayerPrefs.GetString("NombreUsuario");
        }
    }

    public void Guardar()
    {
        if (campo != null)
        {
            PlayerPrefs.SetString("NombreUsuario", campo.text);
            PlayerPrefs.Save();
        }
    }
}
