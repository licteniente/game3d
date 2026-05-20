using UnityEngine;

public class Interactable : MonoBehaviour
{
    public enum TipoInteractuable
    {
        Cristal,
        Pieza,
        Portal,
        SalidaCuevas,
        Trampa
    }

    [Header("Tipo de objeto")]
    public TipoInteractuable tipo;

    [Header("Datos del objeto")]
    public string idObjeto;
    public string nombrePieza;
    public string escenaDestino;

    [TextArea]
    public string mensajeAlRecoger;

    [Header("Forma de interacción")]
    public bool recogerAlTocar = true;

    [Header("Referencia")]
    public GameManager gameManager;

    private bool usado = false;

    void OnTriggerEnter(Collider other)
    {
        if (!recogerAlTocar)
        {
            return;
        }

        if (usado)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            EjecutarInteraccion();
        }
    }

    public void Interactuar(GameManager gm)
    {
        if (usado)
        {
            return;
        }

        gameManager = gm;
        EjecutarInteraccion();
    }

    void EjecutarInteraccion()
    {
        if (gameManager == null)
        {
            Debug.LogWarning("No se asignó el GameManager en " + gameObject.name);
            return;
        }

        usado = true;

        if (tipo == TipoInteractuable.Cristal)
        {
            gameManager.AgregarCristal(idObjeto);

            if (mensajeAlRecoger != "")
            {
                gameManager.MostrarMensaje(mensajeAlRecoger);
            }

            Destroy(gameObject);
        }
        else if (tipo == TipoInteractuable.Pieza)
        {
            gameManager.RepararPieza(nombrePieza);

            if (mensajeAlRecoger != "")
            {
                gameManager.MostrarMensaje(mensajeAlRecoger);
            }

            Destroy(gameObject);
        }
        else if (tipo == TipoInteractuable.Portal)
        {
            if (mensajeAlRecoger != "")
            {
                gameManager.MostrarMensaje(mensajeAlRecoger);
            }

            gameManager.CambiarEscena(escenaDestino);
        }
        else if (tipo == TipoInteractuable.SalidaCuevas)
        {
            if (mensajeAlRecoger != "")
            {
                gameManager.MostrarMensaje(mensajeAlRecoger);
            }

            gameManager.VerificarSalidaCuevas();
        }
        else if (tipo == TipoInteractuable.Trampa)
        {
            if (mensajeAlRecoger != "")
            {
                gameManager.MostrarMensaje(mensajeAlRecoger);
            }
            else
            {
                gameManager.MostrarMensaje("Has caído en una trampa.");
            }

            gameManager.ReiniciarEscenaActual();
        }
    }
}