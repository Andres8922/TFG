using UnityEngine;
using TMPro;
using System.Collections;

public class TextoFlotante : MonoBehaviour
{
    public float duracion = 1.2f;
    public float fuerzaDisparo = 3f;
    public float gravedad = 6f;

    public void Inicializar(int cantidad, Color color)
    {
        TMP_Text texto = GetComponent<TMP_Text>();
        texto.text = "-" + cantidad;
        texto.color = color;

        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr != null)
        {
            mr.sortingLayerName = "UI";
            mr.sortingOrder = 100;
        }

        float anguloAleatorio = Random.Range(50f, 130f); // arco superior: entre 50° y 130°
        float rad = anguloAleatorio * Mathf.Deg2Rad;
        Vector2 direccion = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

        StartCoroutine(Animar(direccion * fuerzaDisparo));
    }

    IEnumerator Animar(Vector2 velocidad)
    {
        TMP_Text texto = GetComponent<TMP_Text>();
        float tiempo = 0f;
        Color colorInicial = texto.color;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float progreso = tiempo / duracion;

            velocidad.y -= gravedad * Time.deltaTime;
            transform.position += new Vector3(velocidad.x, velocidad.y, 0f) * Time.deltaTime;
            texto.color = new Color(colorInicial.r, colorInicial.g, colorInicial.b, 1f - progreso);

            yield return null;
        }

        Destroy(gameObject);
    }
}
