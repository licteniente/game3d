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

    public void Interactuar(GameManager gameManager)
    {
        if (gameManager == null)
        {
            Debug.LogWarning("No se asignó el GameManager para interactuar.");
            return;
        }

        if (tipo == TipoInteractuable.Cristal)
        {
            gameManager.AgregarCristal(idObjeto);
            Destroy(gameObject);
        }
        else if (tipo == TipoInteractuable.Pieza)
        {
            gameManager.RepararPieza(nombrePieza);
            Destroy(gameObject);
        }
        else if (tipo == TipoInteractuable.Portal)
        {
            gameManager.CambiarEscena(escenaDestino);
        }
        else if (tipo == TipoInteractuable.SalidaCuevas)
        {
            gameManager.VerificarSalidaCuevas();
        }
        else if (tipo == TipoInteractuable.Trampa)
        {
            gameManager.MostrarMensaje("Has caído en una trampa.");
            gameManager.ReiniciarEscenaActual();
        }
    }
}