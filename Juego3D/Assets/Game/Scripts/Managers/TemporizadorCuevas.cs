using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TemporizadorCuevas : MonoBehaviour
{
    public float tiempoLimite = 120f; // 2 minutos
    public int cristalesNecesarios = 5;
    public Text textoTiempo; // arrastra el Text de la UI aquí

    private float tiempoRestante;
    private bool activo = true;

    void Start()
    {
        tiempoRestante = tiempoLimite;
    }

    void Update()
    {
        if (!activo) return;

        tiempoRestante -= Time.deltaTime;

        if (textoTiempo != null)
            textoTiempo.text = "Tiempo: " + Mathf.CeilToInt(tiempoRestante) + "s";

        if (tiempoRestante <= 0)
            ReiniciarEscena();
    }

    public void VerificarEscape()
    {
        int recogidos = GameManager.Instance.cristalesRecogidos.Count;
        if (recogidos >= cristalesNecesarios)
        {
            activo = false;
            GameManager.Instance.CargarEscena("ZonaNave");
        }
        else
        {
            Debug.Log("Necesitas " + cristalesNecesarios + " cristales. Tienes: " + recogidos);
        }
    }

    void ReiniciarEscena()
    {
        Debug.Log("¡Se acabó el tiempo! Reiniciando cuevas...");
        SceneManager.LoadScene("Cuevas");
    }
}