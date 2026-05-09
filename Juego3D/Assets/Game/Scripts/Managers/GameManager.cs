using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Estructuras de datos requeridas
    public Dictionary<string, bool> piezasNave = new Dictionary<string, bool>();
    public List<string> cristalesRecogidos = new List<string>();
    public Queue<string> tareasReparacion = new Queue<string>();
    public Stack<string> historialEventos = new Stack<string>();

    public int energiaTotal = 0;
    public int escenaActual = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InicializarJuego();
        }
        else Destroy(gameObject);
    }

    void InicializarJuego()
    {
        piezasNave["motor"] = false;
        piezasNave["antena"] = false;
        piezasNave["combustible"] = false;

        tareasReparacion.Enqueue("Encontrar motor");
        tareasReparacion.Enqueue("Encontrar antena");
        tareasReparacion.Enqueue("Llenar combustible");

        historialEventos.Push("Juego iniciado");
    }

    public void CargarEscena(string nombre)
    {
        escenaActual++;
        historialEventos.Push("Entrando a: " + nombre);
        SceneManager.LoadScene(nombre);
    }

    public void AgregarCristal(string id)
    {
        if (!cristalesRecogidos.Contains(id))
        {
            cristalesRecogidos.Add(id);
            energiaTotal += 10;
            historialEventos.Push("Cristal recogido: " + id);
        }
    }

    public void CompletarPieza(string pieza)
    {
        if (piezasNave.ContainsKey(pieza))
        {
            piezasNave[pieza] = true;
            if (tareasReparacion.Count > 0)
                tareasReparacion.Dequeue();
        }
    }
}