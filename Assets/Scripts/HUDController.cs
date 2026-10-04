using TMPro;
using UnityEngine;

// Solo dibuja: escucha al GameManager y actualiza textos y panel final.
public class HUDController : MonoBehaviour
{
    [SerializeField] TMP_Text livesText;
    [SerializeField] TMP_Text keysText;
    [SerializeField] TMP_Text giftsText;     // opcional
    [SerializeField] GameObject tapToStart;  // cartel "Toca para empezar"
    [SerializeField] GameObject endPanel;
    [SerializeField] TMP_Text endTitle;
    [SerializeField] GameObject endButtons;  // Reintentar + Menú (se ocultan si pasas de nivel)
    [SerializeField] string winMessage = "¡Liberaste al conejo!";
    [SerializeField] string loseMessage = "El monstruo te atrapó";
    [SerializeField] HeartPulse heartPulse;  // ícono de corazón que late al recuperar vida

    int lastLives;

    // Start y no OnEnable: así el Awake del GameManager ya corrió y Instance no es null.
    void Start()
    {
        GameManager gm = GameManager.Instance;
        gm.Changed += Refresh;
        gm.Ended += ShowEnd;
        endPanel.SetActive(false);
        lastLives = gm.Lives;
        Refresh();
    }

    void OnDestroy()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null) return;
        gm.Changed -= Refresh;
        gm.Ended -= ShowEnd;
    }

    void Refresh()
    {
        GameManager gm = GameManager.Instance;
        livesText.text = $"Vidas: {gm.Lives}/{gm.MaxLives}";
        keysText.text = $"Llaves: {gm.Keys}/{gm.KeysToWin}";
        if (giftsText != null) giftsText.text = $"Regalos: {gm.Gifts}";
        if (tapToStart != null) tapToStart.SetActive(!gm.IsPlaying && !gm.IsOver);

        // Condicional: solo late si las vidas SUBIERON.
        if (heartPulse != null && gm.Lives > lastLives)
        {
            heartPulse.Pulse();
        }
        lastLives = gm.Lives;
    }

    void ShowEnd(bool won)
    {
        endPanel.SetActive(true);
        endTitle.text = won ? winMessage : loseMessage;

        bool goingToNextLevel = won && GameManager.Instance.HasNextLevel;
        if (endButtons != null) endButtons.SetActive(!goingToNextLevel);
    }
}
