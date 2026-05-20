using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Referencias")]
    public Camera camaraJugador;
    public GameManager gameManager;

    [Header("Raycast")]
    public float distanciaInteraccion = 3f;
    public KeyCode teclaInteraccion = KeyCode.E;

    void Update()
    {
        DetectarInteraccion();
    }

    void DetectarInteraccion()
    {
        if (Input.GetKeyDown(teclaInteraccion))
        {
            Ray rayo = new Ray(camaraJugador.transform.position, camaraJugador.transform.forward);
            RaycastHit golpe;

            if (Physics.Raycast(rayo, out golpe, distanciaInteraccion))
            {
                Interactable objeto = golpe.collider.GetComponent<Interactable>();

                if (objeto != null)
                {
                    objeto.Interactuar(gameManager);
                }
                else
                {
                    Debug.Log("El objeto detectado no es interactuable.");
                }
            }
        }
    }
}