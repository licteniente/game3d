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

    [Header("Daño")]
    public int daño = 1;
    public bool destruirAlTocar = true;

    [Header("Sonidos")]
    public AudioClip sonidoRecoger;
    public AudioClip sonidoDaño;

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

        if (tipo == TipoInteractuable.Cristal)
        {
            usado = true;

            gameManager.AgregarCristal(idObjeto);

            if (mensajeAlRecoger != "")
            {
                gameManager.MostrarMensaje(mensajeAlRecoger);
            }

            if (sonidoRecoger != null)
            {
                AudioSource.PlayClipAtPoint(sonidoRecoger, transform.position);
            }

            Destroy(gameObject);
        }
        else if (tipo == TipoInteractuable.Pieza)
        {
            usado = true;

            gameManager.RepararPieza(nombrePieza);

            if (mensajeAlRecoger != "")
            {
                gameManager.MostrarMensaje(mensajeAlRecoger);
            }

            if (sonidoRecoger != null)
            {
                AudioSource.PlayClipAtPoint(sonidoRecoger, transform.position);
            }

            Destroy(gameObject);
        }
        else if (tipo == TipoInteractuable.Portal)
        {
            usado = true;

            if (mensajeAlRecoger != "")
            {
                gameManager.MostrarMensaje(mensajeAlRecoger);
            }

            gameManager.CambiarEscena(escenaDestino);
        }
        else if (tipo == TipoInteractuable.SalidaCuevas)
        {
            usado = true;

            if (mensajeAlRecoger != "")
            {
                gameManager.MostrarMensaje(mensajeAlRecoger);
            }

            gameManager.VerificarSalidaCuevas();
        }
        else if (tipo == TipoInteractuable.Trampa)
        {
            gameManager.QuitarVida(daño);

            if (mensajeAlRecoger != "")
            {
                gameManager.MostrarMensaje(mensajeAlRecoger);
            }
            else
            {
                gameManager.MostrarMensaje("Cuidado, el hongo te hizo daño.");
            }

            if (sonidoDaño != null)
            {
                AudioSource.PlayClipAtPoint(sonidoDaño, transform.position);
            }

            if (destruirAlTocar)
            {
                usado = true;
                Destroy(gameObject);
            }
        }
    }
}