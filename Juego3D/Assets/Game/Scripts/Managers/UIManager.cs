using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Canvas")]
    public GameObject canvasMenu;
    public GameObject canvasConfiguracion;

    void Start()
    {
        MostrarMenu();
    }

    public void MostrarMenu()
    {
        canvasMenu.SetActive(true);
        canvasConfiguracion.SetActive(false);
    }

    public void MostrarConfiguracion()
    {
        canvasMenu.SetActive(false);
        canvasConfiguracion.SetActive(true);
    }
}