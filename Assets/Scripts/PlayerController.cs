using UnityEngine;
using UnityEngine.InputSystem;

// Requisitos: Inputs, Variables, Condicionales, Funciones y animación Idle/Run.
// Tocás (o hacés clic) en la pantalla y la chica va al carril más cercano a tu dedo.
// <Pointer> lee mouse y touch con el mismo código: sirve para PC y para Android.
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float[] laneY = { 2f, 0f, -2f };   // altura de cada carril, de arriba a abajo
    [SerializeField] float switchSpeed = 20f;           // qué tan rápido cambia de carril

    InputAction pointerPos;
    InputAction pointerPress;
    Rigidbody2D rb;
    Animator anim;
    Camera cam;
    int lane;

    // El Spawner usa los mismos carriles: una sola fuente de verdad.
    public float[] LaneY => laneY;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        cam = Camera.main;

        lane = laneY.Length / 2;   // arranca en el carril del medio
        rb.position = new Vector2(rb.position.x, laneY[lane]);

        pointerPos = new InputAction("PointerPos", InputActionType.Value, "<Pointer>/position");
        pointerPress = new InputAction("PointerPress", InputActionType.Button, "<Pointer>/press");
    }

    void OnEnable()
    {
        pointerPos.Enable();
        pointerPress.Enable();
    }

    void OnDisable()
    {
        pointerPos.Disable();
        pointerPress.Disable();
    }

    void OnDestroy()
    {
        pointerPos.Dispose();
        pointerPress.Dispose();
    }

    void Update()
    {
        GameManager gm = GameManager.Instance;

        // Animación: corre solo mientras se está jugando. Antes del primer toque y al terminar, Idle.
        if (anim != null)
        {
            anim.SetBool("IsRunning", gm.IsPlaying);
        }

        if (gm.IsOver) return;
        if (!pointerPress.WasPressedThisFrame()) return;

        // El primer toque arranca la partida.
        if (!gm.IsPlaying)
        {
            gm.StartGame();
        }

        float tapY = cam.ScreenToWorldPoint(pointerPos.ReadValue<Vector2>()).y;
        lane = NearestLane(tapY);
    }

    void FixedUpdate()
    {
        float y = Mathf.MoveTowards(rb.position.y, laneY[lane], switchSpeed * Time.fixedDeltaTime);
        rb.MovePosition(new Vector2(rb.position.x, y));
    }

    int NearestLane(float y)
    {
        int best = 0;
        for (int i = 1; i < laneY.Length; i++)
        {
            if (Mathf.Abs(laneY[i] - y) < Mathf.Abs(laneY[best] - y))
            {
                best = i;
            }
        }
        return best;
    }
}
