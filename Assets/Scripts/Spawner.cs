using System.Collections;
using UnityEngine;

// Requisito: Instantiate con intervalos de tiempo (Corrutina).
// Cada cierto tiempo crea un monstruo o un objeto en un carril al azar, fuera de cámara a la derecha.
// Se usa corrutina y no InvokeRepeating porque el intervalo se acorta con el tiempo (más difícil).
public class Spawner : MonoBehaviour
{
    [SerializeField] PlayerController player;

    [Header("Prefabs")]
    [SerializeField] GameObject monsterPrefab;
    [SerializeField] GameObject keyPrefab;
    [SerializeField] GameObject heartPrefab;
    [SerializeField] GameObject giftPrefab;   // opcional: si queda vacío, no salen regalos

    [Header("Probabilidades (el resto son monstruos)")]
    [SerializeField, Range(0f, 1f)] float keyChance = 0.2f;
    [SerializeField, Range(0f, 1f)] float heartChance = 0.08f;
    [SerializeField, Range(0f, 1f)] float giftChance = 0.15f;

    [Header("Ritmo")]
    [SerializeField] float startInterval = 1.4f;
    [SerializeField] float minInterval = 0.6f;
    [SerializeField] float intervalStep = 0.03f;

    IEnumerator Start()
    {
        // Espera al primer toque del jugador.
        while (!GameManager.Instance.IsPlaying)
        {
            yield return null;
        }

        float interval = startInterval;
        while (!GameManager.Instance.IsOver)
        {
            SpawnOne();
            yield return new WaitForSeconds(interval);
            interval = Mathf.Max(minInterval, interval - intervalStep);
        }
    }

    void SpawnOne()
    {
        Camera cam = Camera.main;
        float x = cam.transform.position.x + cam.orthographicSize * cam.aspect + 1f;
        float[] lanes = player.LaneY;
        float y = lanes[Random.Range(0, lanes.Length)];

        Instantiate(PickPrefab(), new Vector3(x, y, 0f), Quaternion.identity);
    }

    GameObject PickPrefab()
    {
        float r = Random.value;

        if (r < keyChance) return keyPrefab;
        r -= keyChance;

        if (r < heartChance) return heartPrefab;
        r -= heartChance;

        if (giftPrefab != null && r < giftChance) return giftPrefab;

        return monsterPrefab;
    }
}
