using UnityEngine;
using TMPro;

public class RaycastInteraccionSelva : MonoBehaviour
{
    public Camera camaraJugador;
    public float distanciaRaycast = 3.5f;
    public TextMeshProUGUI textoInteractuar;

    private GameObject objetoDetectado;

    void Start()
    {
        if (textoInteractuar != null)
        {
            textoInteractuar.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        DetectarObjeto();

        if (objetoDetectado != null && Input.GetKeyDown(KeyCode.E))
        {
            RecogerObjeto(objetoDetectado);
        }
    }

    void DetectarObjeto()
    {
        Ray ray = new Ray(camaraJugador.transform.position, camaraJugador.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distanciaRaycast, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide))
        {
            if (hit.collider.CompareTag("Interactuable"))
            {
                objetoDetectado = hit.collider.gameObject;

                if (textoInteractuar != null)
                {
                    textoInteractuar.gameObject.SetActive(true);
                }

                return;
            }
        }

        objetoDetectado = null;

        if (textoInteractuar != null)
        {
            textoInteractuar.gameObject.SetActive(false);
        }
    }

    void RecogerObjeto(GameObject objeto)
    {
        AudioSource audio = objeto.GetComponent<AudioSource>();

        if (audio != null)
        {
            audio.Play();
        }

        Collider col = objeto.GetComponent<Collider>();

        if (col != null)
        {
            col.enabled = false;
        }

        foreach (Transform hijo in objeto.transform)
        {
            hijo.gameObject.SetActive(false);
        }

        if (textoInteractuar != null)
        {
            textoInteractuar.gameObject.SetActive(false);
        }

        objetoDetectado = null;

        Destroy(objeto, 1f);
    }
}