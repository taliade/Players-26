using UnityEngine;
using UnityEngine.SceneManagement;

// Escena Menú: botones Jugar y Salir.
public class MenuController : MonoBehaviour
{
    [SerializeField] string gameScene = "Juego";

    public void Play()
    {
        SceneManager.LoadScene(gameScene);
    }

    public void Quit()
    {
        Application.Quit();   // en el editor no hace nada; en el .exe cierra el juego
    }
}
