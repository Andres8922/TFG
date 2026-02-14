using UnityEngine;
using TMPro;

public class GuardarNombreUsuario : MonoBehaviour
{
    private TMP_InputField campo;

    void Start()
    {
        GameObject[] todos = GameObject.FindObjectsOfType<GameObject>();

        Debug.Log("Objetos en escena: " + todos.Length);

        foreach (GameObject obj in todos)
        {
            if (obj.GetComponent<TMP_InputField>() != null)
            {
                campo = obj.GetComponent<TMP_InputField>();
                Debug.Log("InputField encontrado: " + obj.name);
                break;
            }
        }

        if (campo == null)
        {
            Debug.LogError("CREA UN INPUTFIELD: UI -> Input Field - TextMeshPro");
        }
        else
        {

            if (PlayerPrefs.HasKey("NombreUsuario"))
            {
                campo.text = PlayerPrefs.GetString("NombreUsuario");
            }
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