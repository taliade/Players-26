using UnityEngine;

// Mueve el objeto de derecha a izquierda (como los zombies de Plants vs Zombies)
// y lo destruye cuando sale por el borde izquierdo de la cámara.
[RequireComponent(typeof(Rigidbody2D))]
public class Mover : MonoBehaviour
{
    [SerializeField] float speed = 4f;

    Rigidbody2D rb;
    Camera cam;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        cam = Camera.main;
    }

    void FixedUpdate()
    {
        // Condicional: si la partida terminó, todo queda quieto.
        if (GameManager.Instance != null && GameManager.Instance.IsOver) return;

        rb.MovePosition(rb.position + Vector2.left * speed * Time.fixedDeltaTime);

        float leftEdge = cam.transform.position.x - cam.orthographicSize * cam.aspect;
        if (rb.position.x < leftEdge - 1f)
        {
            Destroy(gameObject);
        }
    }
}
