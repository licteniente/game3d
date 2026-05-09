using UnityEngine;
using UnityEngine.InputSystem;

public class MovePlayer : MonoBehaviour
{
    public float speedplayer = 5.0f;
    public float speedRotation = 200f;

    private Vector2 movementInput;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float x = movementInput.x;
        float y = movementInput.y;

        // Movimiento del personaje
        transform.Rotate(0, x * speedRotation * Time.deltaTime, 0);
        transform.Translate(0, 0, y * speedplayer * Time.deltaTime);

        // Animaciones con suavizado
        if (animator != null)
        {
            animator.SetFloat("VelX", x, 0.1f, Time.deltaTime);
            animator.SetFloat("VelY", y, 0.1f, Time.deltaTime);
            animator.SetFloat("Blend", movementInput.magnitude, 0.1f, Time.deltaTime);
        }
    }

    public void OnMove(InputValue value)
    {
        movementInput = value.Get<Vector2>();
    }
}