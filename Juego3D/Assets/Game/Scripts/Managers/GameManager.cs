using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[System.Serializable]
public class DatosGuardado
{
    public int energiaTotal;
    public int escenaActual;
    public string[] cristalesRecogidos;
    public string[] piezasReparadas;
}

public class GameManager : MonoBehaviour
{
    [Header("Datos del jugador")]
    public int energiaTotal = 0;
    public int escenaActual = 0;

    [Header("Estructuras de datos requeridas")]
    public List<string> cristalesRecogidos = new List<string>();
    public Dictionary<string, bool> piezasNave = new Dictionary<string, bool>();
    public Queue<string> tareasReparacion = new Queue<string>();
    public Stack<string> historialEventos = new Stack<string>();

    [Header("Temporizador de Cuevas")]
    public bool usarTemporizador = false;
    public float tiempoLimite = 120f;
    public int cristalesNecesarios = 5;
    private float tiempoRestante;
    private bool tiempoActivo = false;

    [Header("UI")]
    public TMP_Text textoEnergia;
    public TMP_Text textoTiempo;
    public TMP_Text textoMensaje;

    private string rutaGuardado;

    void Start()
    {
        rutaGuardado = Application.persistentDataPath + "/guardado.json";

        InicializarEstructuras();

        if (File.Exists(rutaGuardado))
        {
            CargarProgreso();
        }

        if (usarTemporizador)
        {
            IniciarTemporizador();
        }

        ActualizarUI();
    }

    void Update()
    {
        if (usarTemporizador && tiempoActivo)
        {
            ActualizarTemporizador();
        }
    }

    void InicializarEstructuras()
    {
        piezasNave.Clear();
        piezasNave.Add("motor", false);
        piezasNave.Add("antena", false);
        piezasNave.Add("combustible", false);

        tareasReparacion.Clear();
        tareasReparacion.Enqueue("Encontrar motor");
        tareasReparacion.Enqueue("Encontrar antena");
        tareasReparacion.Enqueue("Llenar combustible");

        historialEventos.Clear();
        historialEventos.Push("Juego iniciado");
    }

    public void NuevaPartida()
    {
        energiaTotal = 0;
        escenaActual = 1;

        cristalesRecogidos.Clear();

        InicializarEstructuras();

        GuardarProgreso();

        SceneManager.LoadScene("Playa");
    }

    public void AgregarCristal(string idCristal)
    {
        if (!cristalesRecogidos.Contains(idCristal))
        {
            cristalesRecogidos.Add(idCristal);
            energiaTotal += 10;

            historialEventos.Push("Cristal recogido: " + idCristal);

            GuardarProgreso();
            ActualizarUI();
        }
    }

    public void RepararPieza(string nombrePieza)
    {
        if (piezasNave.ContainsKey(nombrePieza))
        {
            piezasNave[nombrePieza] = true;

            if (tareasReparacion.Count > 0)
            {
                tareasReparacion.Dequeue();
            }

            historialEventos.Push("Pieza reparada: " + nombrePieza);

            GuardarProgreso();
            ActualizarUI();
        }
    }

    public void CambiarEscena(string nombreEscena)
    {
        if (nombreEscena == "Playa")
        {
            escenaActual = 1;
        }
        else if (nombreEscena == "Selva")
        {
            escenaActual = 2;
        }
        else if (nombreEscena == "Cuevas Cristalinas")
        {
            escenaActual = 3;
        }
        else if (nombreEscena == "Zona Nave")
        {
            escenaActual = 4;
        }

        historialEventos.Push("Cambio de escena: " + nombreEscena);

        GuardarProgreso();

        SceneManager.LoadScene(nombreEscena);
    }

    public void ContinuarPartida()
    {
        CargarProgreso();

        if (escenaActual == 1)
        {
            SceneManager.LoadScene("Playa");
        }
        else if (escenaActual == 2)
        {
            SceneManager.LoadScene("Selva");
        }
        else if (escenaActual == 3)
        {
            SceneManager.LoadScene("Cuevas Cristalinas");
        }
        else if (escenaActual == 4)
        {
            SceneManager.LoadScene("Zona Nave");
        }
        else
        {
            SceneManager.LoadScene("Playa");
        }
    }

    void IniciarTemporizador()
    {
        tiempoRestante = tiempoLimite;
        tiempoActivo = true;
        ActualizarUI();
    }

    void ActualizarTemporizador()
    {
        tiempoRestante -= Time.deltaTime;

        if (textoTiempo != null)
        {
            textoTiempo.text = "Tiempo: " + Mathf.CeilToInt(tiempoRestante) + "s";
        }

        if (tiempoRestante <= 0)
        {
            ReiniciarEscenaActual();
        }
    }

    public void VerificarSalidaCuevas()
    {
        if (cristalesRecogidos.Count >= cristalesNecesarios)
        {
            tiempoActivo = false;
            CambiarEscena("Zona Nave");
        }
        else
        {
            MostrarMensaje("Necesitas " + cristalesNecesarios + " cristales para salir.");
        }
    }

    public void ReiniciarEscenaActual()
    {
        historialEventos.Push("Se reinició la escena por tiempo terminado");

        GuardarProgreso();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GuardarProgreso()
    {
        DatosGuardado datos = new DatosGuardado();

        datos.energiaTotal = energiaTotal;
        datos.escenaActual = escenaActual;
        datos.cristalesRecogidos = cristalesRecogidos.ToArray();

        List<string> piezasListas = new List<string>();

        foreach (KeyValuePair<string, bool> pieza in piezasNave)
        {
            if (pieza.Value == true)
            {
                piezasListas.Add(pieza.Key);
            }
        }

        datos.piezasReparadas = piezasListas.ToArray();

        string json = JsonUtility.ToJson(datos, true);

        File.WriteAllText(rutaGuardado, json);

        Debug.Log("Progreso guardado en: " + rutaGuardado);
    }

    public void CargarProgreso()
    {
        if (!File.Exists(rutaGuardado))
        {
            Debug.Log("No existe archivo de guardado todavía.");
            return;
        }

        string json = File.ReadAllText(rutaGuardado);

        DatosGuardado datos = JsonUtility.FromJson<DatosGuardado>(json);

        energiaTotal = datos.energiaTotal;
        escenaActual = datos.escenaActual;

        cristalesRecogidos.Clear();

        if (datos.cristalesRecogidos != null)
        {
            cristalesRecogidos.AddRange(datos.cristalesRecogidos);
        }

        if (datos.piezasReparadas != null)
        {
            foreach (string pieza in datos.piezasReparadas)
            {
                if (piezasNave.ContainsKey(pieza))
                {
                    piezasNave[pieza] = true;
                }
            }
        }

        historialEventos.Push("Progreso cargado");

        ActualizarUI();
    }

    void ActualizarUI()
    {
        if (textoEnergia != null)
        {
            textoEnergia.text = "Energía: " + energiaTotal;
        }

        if (textoTiempo != null && usarTemporizador)
        {
            textoTiempo.text = "Tiempo: " + Mathf.CeilToInt(tiempoRestante) + "s";
        }
    }

    public void MostrarMensaje(string mensaje)
    {
        if (textoMensaje != null)
        {
            textoMensaje.text = mensaje;
        }

        Debug.Log(mensaje);
    }
}