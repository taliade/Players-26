using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Dueño del estado de la partida: vidas, llaves, regalos, victoria y derrota.
// No toca la UI: avisa con eventos y el HUDController dibuja.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] int maxLives = 3;
    [SerializeField] int keysToWin = 5;

    [Header("Nivel")]
    [SerializeField] string nextScene = "";       // vacío = último nivel. Ej: "Nivel2_Calle"
    [SerializeField] float nextSceneDelay = 2f;   // segundos mostrando el mensaje antes de cambiar

    public int Lives { get; private set; }
    public int Keys { get; private set; }
    public int Gifts { get; private set; }
    public int MaxLives => maxLives;
    public int KeysToWin => keysToWin;
    public bool IsPlaying { get; private set; }
    public bool IsOver { get; private set; }
    public bool HasNextLevel => !string.IsNullOrEmpty(nextScene);

    public event Action Changed;       // algo cambió: el HUD se redibuja
    public event Action<bool> Ended;   // true = liberaste al conejo

    void Awake()
    {
        Instance = this;
        Lives = maxLives;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void StartGame()
    {
        IsPlaying = true;
        Changed?.Invoke();
    }

    public void TakeHit()
    {
        Lives = Mathf.Max(0, Lives - 1);
        Changed?.Invoke();
        if (Lives == 0) End(false);
    }

    public void Heal()
    {
        Lives = Mathf.Min(maxLives, Lives + 1);
        Changed?.Invoke();
    }

    public void AddKey()
    {
        Keys++;
        Changed?.Invoke();
        if (Keys >= keysToWin) End(true);
    }

    public void AddGift()
    {
        Gifts++;
        Changed?.Invoke();
    }

    void End(bool won)
    {
        IsOver = true;
        IsPlaying = false;
        Ended?.Invoke(won);

        // Condicional: si ganaste y hay otro nivel, pasa solo después de unos segundos.
        if (won && HasNextLevel)
        {
            Invoke(nameof(LoadNextLevel), nextSceneDelay);
        }
    }

    void LoadNextLevel()
    {
        SceneManager.LoadScene(nextScene);
    }

    // Va conectado al OnClick del botón "Reintentar".
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Va conectado al OnClick del botón "Menú".
    public void GoToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
