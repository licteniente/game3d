using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void NuevaPartida()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.energiaTotal = 0;
            GameManager.Instance.escenaActual = 0;
            GameManager.Instance.cristalesRecogidos.Clear();

            var piezas = GameManager.Instance.piezasNave;
            foreach (var key in new System.Collections.Generic.List<string>(piezas.Keys))
                piezas[key] = false;

            GameManager.Instance.historialEventos.Push("Nueva partida iniciada");
        }

        SceneManager.LoadScene("Playa");
    }

    public void CargarPartida()
    {
        if (JSONManager.Instance == null)
        {
            Debug.LogWarning("No se encontró JSONManager.");
            return;
        }

        JSONManager.Instance.CargarProgreso();

        int escena = GameManager.Instance.escenaActual;
        string[] escenas = { "Playa", "Selva", "Cuevas", "ZonaNave" };

        if (escena >= 0 && escena < escenas.Length)
            SceneManager.LoadScene(escenas[escena]);
        else
            SceneManager.LoadScene("Playa");
    }

    public void Salir()
    {
        Debug.Log("Saliendo del juego");
        Application.Quit();
    }
}