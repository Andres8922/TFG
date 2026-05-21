using UnityEngine;

public class SelectorDificultad : MonoBehaviour
{
    // 0 = Fácil, 1 = Medio, 2 = Difícil
    public void ElegirDificultad(int indice)
    {
        GameManager.Instance.dificultad = indice;
        Transicion.Instance.CargarEscena("SeleccionHeroe");
    }
}
