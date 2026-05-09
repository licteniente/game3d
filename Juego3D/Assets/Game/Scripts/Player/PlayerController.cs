using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    public float velocidad = 5f;
    public float fuerzaSalto = 5f;
    public float gravedad = -9.81f;
    public Transform camara;

    private CharacterController cc;
    private Vector3 velocidadVertical;
    private bool enSuelo;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        enSuelo = cc.isGrounded;
        if (enSuelo && velocidadVertical.y < 0)
            velocidadVertical.y = -2f;
        
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Movimiento relativo a la cámara
        Vector3 dir = camara.right * x + camara.forward * z;
        dir.y = 0f;
        cc.Move(dir.normalized * velocidad * Time.deltaTime);

        // Rotar el personaje hacia donde se mueve
        if (dir.magnitude > 0.1f)
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(dir),
                0.15f
            );

        // Salto
        if (Input.GetButtonDown("Jump") && enSuelo)
            velocidadVertical.y = Mathf.Sqrt(fuerzaSalto * -2f * gravedad);

        velocidadVertical.y += gravedad * Time.deltaTime;
        cc.Move(velocidadVertical * Time.deltaTime);
    }
}   