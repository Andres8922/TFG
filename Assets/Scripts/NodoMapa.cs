using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para viajar

public class NodoMapa : MonoBehaviour
{
    // Definimos los tipos de casillas que existen
    public enum TipoNodo { Enemigo, Jefe, Tienda, Tesoro }
    public TipoNodo tipoDeEsteNodo;

    private void OnMouseDown()
    {
        Debug.Log("Viajando al combate...");

        // Guardamos en el cerebro (GameManager) qué tipo de pelea toca
        // (Esto lo usaremos luego para saber si spawneamos un goblin o un dragón)
        // GameManager.Instance.tipoEncuentro = tipoDeEsteNodo; <--- Lo descomentaremos luego

        // Cargamos la escena de peleas
        Transicion.Instance.CargarEscena("Combate");
    }

    public void ConfigurarNodo(TipoNodo nuevoTipo)
    {
        tipoDeEsteNodo = nuevoTipo;

        // Cambiamos el color según el tipo para saber qué es
        SpriteRenderer dibujo = GetComponent<SpriteRenderer>();

        if (nuevoTipo == TipoNodo.Enemigo) dibujo.color = Color.red;       // Rojo = Enemigo
        if (nuevoTipo == TipoNodo.Jefe) dibujo.color = Color.black;     // Negro = Jefe Final
        if (nuevoTipo == TipoNodo.Tienda) dibujo.color = Color.yellow;    // Amarillo = Tienda
        if (nuevoTipo == TipoNodo.Tesoro) dibujo.color = Color.cyan;      // Azul = Tesoro
    }
}