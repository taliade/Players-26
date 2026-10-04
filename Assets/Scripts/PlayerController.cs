using UnityEngine;
using UnityEngine.InputSystem;

// Requisitos: Inputs, Variables, Condicionales, Funciones y animación Idle/Run.
// Dos controles: tocar/hacer clic (va al carril más cercano al dedo) o flechas ↑ ↓ / W S (un carril por vez).
// <Pointer> lee mouse y touch con el mismo código: sirve para PC y para Android.
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float[] laneY = { 2f, 0f, -2f };   // altura de cada carril, de arriba a abajo
    [SerializeField] float switchSpeed = 20f;           // qué tan rápido cambia de carril
    [SerializeField] AudioSource footsteps;             // sonido de pasos en loop mientras corre

    InputAction pointerPos;
    InputAction pointerPress;
    InputAction laneUp;
    InputAction laneDown;
    Rigidbody2D rb;
    Animator anim;
    Camera cam;
    int lane;

    // El Spawner usa los mismos carriles: una sola fuente de verdad.
    public float[] LaneY => laneY;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;   // que nunca "se duerma" y deje de detectar choques
        anim = GetComponent<Animator>();
        cam = Camera.main;

        lane = laneY.Length / 2;   // arranca en el carril del medio
        rb.position = new Vector2(rb.position.x, laneY[lane]);

        pointerPos = new InputAction("PointerPos", InputActionType.Value, "<Pointer>/position");
        pointerPress = new InputAction("PointerPress", InputActionType.Button, "<Pointer>/press");

        laneUp = new InputAction("LaneUp", InputActionType.Button);
        laneUp.AddBinding("<Keyboard>/upArrow");
        laneUp.AddBinding("<Keyboard>/w");

        laneDown = new InputAction("LaneDown", InputActionType.Button);
        laneDown.AddBinding("<Keyboard>/downArrow");
        laneDown.AddBinding("<Keyboard>/s");
    }

    void OnEnable()
    {
        pointerPos.Enable();
        pointerPress.Enable();
        laneUp.Enable();
        laneDown.Enable();
    }

    void OnDisable()
    {
        pointerPos.Disable();
        pointerPress.Disable();
        laneUp.Disable();
        laneDown.Disable();
    }

    void OnDestroy()
    {
        pointerPos.Dispose();
        pointerPress.Dispose();
        laneUp.Dispose();
        laneDown.Dispose();
    }

    void Update()
    {
        GameManager gm = GameManager.Instance;

        // Animación: corre solo mientras se está jugando. Antes del primer toque y al terminar, Idle.
        if (anim != null)
        {
            anim.SetBool("IsRunning", gm.IsPlaying);
        }
        UpdateFootsteps(gm.IsPlaying);

        if (gm.IsOver) return;

        bool tapped = pointerPress.WasPressedThisFrame();
        bool up = laneUp.WasPressedThisFrame();
        bool down = laneDown.WasPressedThisFrame();
        if (!tapped && !up && !down) return;

        // El primer toque o flecha arranca la partida.
        if (!gm.IsPlaying)
        {
            gm.StartGame();
        }

        if (tapped)
        {
            float tapY = cam.ScreenToWorldPoint(pointerPos.ReadValue<Vector2>()).y;
            lane = NearestLane(tapY);
        }
        else if (up)
        {
            lane = Mathf.Max(0, lane - 1);          // el carril 0 es el de arriba
        }
        else
        {
            lane = Mathf.Min(laneY.Length - 1, lane + 1);
        }
    }

    void UpdateFootsteps(bool running)
    {
        if (footsteps == null) return;

        if (running && !footsteps.isPlaying)
        {
            footsteps.Play();
        }
        else if (!running && footsteps.isPlaying)
        {
            footsteps.Stop();
        }
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
