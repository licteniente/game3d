using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Referencia al GameManager")]
    public GameManager gameManager;

    [Header("Paneles del menú")]
    public GameObject panelMenuPrincipal;
    public GameObject panelConfiguracion;

    void Start()
    {
        MostrarMenuPrincipal();
    }

    public void NuevaPartida()
    {
        if (gameManager != null)
        {
            gameManager.NuevaPartida();
        }
        else
        {
            Debug.LogWarning("No se asignó el GameManager en el MenuManager.");
            SceneManager.LoadScene("Selva");
        }
    }

    public void ContinuarPartida()
    {
        if (gameManager != null)
        {
            gameManager.ContinuarPartida();
        }
        else
        {
            Debug.LogWarning("No se asignó el GameManager en el MenuManager.");
            SceneManager.LoadScene("Selva");
        }
    }

    public void MostrarConfiguracion()
    {
        if (panelMenuPrincipal != null)
        {
            panelMenuPrincipal.SetActive(false);
        }

        if (panelConfiguracion != null)
        {
            panelConfiguracion.SetActive(true);
        }
    }

    public void MostrarMenuPrincipal()
    {
        if (panelMenuPrincipal != null)
        {
            panelMenuPrincipal.SetActive(true);
        }

        if (panelConfiguracion != null)
        {
            panelConfiguracion.SetActive(false);
        }
    }

    public void SalirJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}