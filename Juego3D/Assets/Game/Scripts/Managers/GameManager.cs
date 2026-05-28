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
    public int vidaJugador;
    public string[] cristalesRecogidos;
    public string[] piezasReparadas;
}

public class GameManager : MonoBehaviour
{
    [Header("Datos del jugador")]
    public int energiaTotal = 0;
    public int escenaActual = 0;
    public int vidaJugador = 3;

    [Header("Estructuras de datos requeridas")]
    public List<string> cristalesRecogidos = new List<string>();
    public Dictionary<string, bool> piezasNave = new Dictionary<string, bool>();
    public Queue<string> tareasReparacion = new Queue<string>();
    public Stack<string> historialEventos = new Stack<string>();

    [Header("Prefabs de objetos recolectables")]
    public GameObject prefabNucleoEnergia;
    public GameObject prefabPiezaMotor;
    public GameObject prefabPiezaAntena;
    public GameObject prefabCombustible;

    [Header("SpawnPoints de núcleos de energía")]
    public Transform[] spawnNucleosEnergia;

    [Header("SpawnPoints de piezas de la nave")]
    public Transform spawnMotor;
    public Transform spawnAntena;
    public Transform spawnCombustible;

    [Header("Temporizador de Cuevas")]
    public bool usarTemporizador = false;
    public float tiempoLimite = 120f;
    public int cristalesNecesarios = 5;

    private float tiempoRestante;
    private bool tiempoActivo = false;

    [Header("UI")]
    public TMP_Text textoEnergia;
    public TMP_Text textoVida;
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

        CrearObjetosRecolectables();

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
        vidaJugador = 3;

        cristalesRecogidos.Clear();

        InicializarEstructuras();

        GuardarProgreso();

        SceneManager.LoadScene("Playa");
    }

    public void CrearObjetosRecolectables()
    {
        CrearNucleosEnergia();
        CrearPiezaMotor();
        CrearPiezaAntena();
        CrearCombustible();
    }

    void CrearNucleosEnergia()
    {
        if (prefabNucleoEnergia == null)
        {
            return;
        }

        if (spawnNucleosEnergia == null)
        {
            return;
        }

        for (int i = 0; i < spawnNucleosEnergia.Length; i++)
        {
            if (spawnNucleosEnergia[i] != null)
            {
                string id = "nucleo_" + SceneManager.GetActiveScene().name + "_" + i;

                if (!cristalesRecogidos.Contains(id))
                {
                    GameObject nuevoNucleo = Instantiate(
                        prefabNucleoEnergia,
                        spawnNucleosEnergia[i].position,
                        spawnNucleosEnergia[i].rotation
                    );

                    Interactable interactable = nuevoNucleo.GetComponent<Interactable>();

                    if (interactable != null)
                    {
                        interactable.tipo = Interactable.TipoInteractuable.Cristal;
                        interactable.idObjeto = id;
                        interactable.mensajeAlRecoger = "Has recogido un núcleo de energía.";
                        interactable.recogerAlTocar = true;
                        interactable.gameManager = this;
                    }
                }
            }
        }
    }

    void CrearPiezaMotor()
    {
        if (prefabPiezaMotor != null && spawnMotor != null)
        {
            if (piezasNave.ContainsKey("motor") && piezasNave["motor"] == false)
            {
                GameObject motor = Instantiate(
                    prefabPiezaMotor,
                    spawnMotor.position,
                    spawnMotor.rotation
                );

                Interactable interactable = motor.GetComponent<Interactable>();

                if (interactable != null)
                {
                    interactable.tipo = Interactable.TipoInteractuable.Pieza;
                    interactable.nombrePieza = "motor";
                    interactable.mensajeAlRecoger = "Has recogido una parte de la nave: motor.";
                    interactable.recogerAlTocar = true;
                    interactable.gameManager = this;
                }
            }
        }
    }

    void CrearPiezaAntena()
    {
        if (prefabPiezaAntena != null && spawnAntena != null)
        {
            if (piezasNave.ContainsKey("antena") && piezasNave["antena"] == false)
            {
                GameObject antena = Instantiate(
                    prefabPiezaAntena,
                    spawnAntena.position,
                    spawnAntena.rotation
                );

                Interactable interactable = antena.GetComponent<Interactable>();

                if (interactable != null)
                {
                    interactable.tipo = Interactable.TipoInteractuable.Pieza;
                    interactable.nombrePieza = "antena";
                    interactable.mensajeAlRecoger = "Has recogido una parte de la nave: antena.";
                    interactable.recogerAlTocar = true;
                    interactable.gameManager = this;
                }
            }
        }
    }

    void CrearCombustible()
    {
        if (prefabCombustible != null && spawnCombustible != null)
        {
            if (piezasNave.ContainsKey("combustible") && piezasNave["combustible"] == false)
            {
                GameObject combustible = Instantiate(
                    prefabCombustible,
                    spawnCombustible.position,
                    spawnCombustible.rotation
                );

                Interactable interactable = combustible.GetComponent<Interactable>();

                if (interactable != null)
                {
                    interactable.tipo = Interactable.TipoInteractuable.Pieza;
                    interactable.nombrePieza = "combustible";
                    interactable.mensajeAlRecoger = "Has recogido suministros de combustible.";
                    interactable.recogerAlTocar = true;
                    interactable.gameManager = this;
                }
            }
        }
    }

    public void AgregarCristal(string idCristal)
    {
        if (!cristalesRecogidos.Contains(idCristal))
        {
            cristalesRecogidos.Add(idCristal);
            energiaTotal += 10;

            historialEventos.Push("Objeto recogido: " + idCristal);

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
        else
        {
            Debug.LogWarning("La pieza no existe en el Dictionary: " + nombrePieza);
        }
    }

    public void QuitarVida(int cantidad)
    {
        vidaJugador -= cantidad;

        if (vidaJugador < 0)
        {
            vidaJugador = 0;
        }

        historialEventos.Push("El jugador perdió vida. Vida actual: " + vidaJugador);

        MostrarMensaje("Has recibido daño. Vida: " + vidaJugador);

        ActualizarUI();
        GuardarProgreso();

        if (vidaJugador <= 0)
        {
            MostrarMensaje("Te quedaste sin vida. Reiniciando escena...");
            ReiniciarEscenaActual();
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
            MostrarMensaje("Se acabó el tiempo.");
            ReiniciarEscenaActual();
        }
    }

    public void VerificarSalidaCuevas()
    {
        if (cristalesRecogidos.Count >= cristalesNecesarios)
        {
            tiempoActivo = false;
            MostrarMensaje("Lograste salir de la cueva.");
            CambiarEscena("Zona Nave");
        }
        else
        {
            MostrarMensaje("Necesitas " + cristalesNecesarios + " cristales para salir.");
        }
    }

    public void ReiniciarEscenaActual()
    {
        historialEventos.Push("Se reinició la escena actual");

        GuardarProgreso();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GuardarProgreso()
    {
        DatosGuardado datos = new DatosGuardado();

        datos.energiaTotal = energiaTotal;
        datos.escenaActual = escenaActual;
        datos.vidaJugador = vidaJugador;
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
        vidaJugador = datos.vidaJugador;

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

        if (textoVida != null)
        {
            textoVida.text = "Vida: " + vidaJugador;
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