using System.Collections;
using UnityEngine;

// Requisito: Colisiones + Condicionales + SFX (+ Instantiate del confeti).
// Va en el Player. Detecta qué tocó, suena el efecto, muestra feedback y le avisa al GameManager.
public class TriggerHandler : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] AudioSource sfx;          // AudioSource del Player con Play On Awake apagado
    [SerializeField] AudioClip hitSfx;
    [SerializeField] AudioClip keySfx;
    [SerializeField] AudioClip heartSfx;       // latido de corazón
    [SerializeField] AudioClip giftSfx;        // sorpresa / confeti

    [Header("Feedback visual")]
    [SerializeField] SpriteRenderer body;      // se pone roja un instante al recibir un golpe
    [SerializeField] GameObject confettiPrefab;

    Color normalColor = Color.white;

    void Awake()
    {
        if (sfx == null) sfx = GetComponent<AudioSource>();
        if (body == null) body = GetComponent<SpriteRenderer>();
        if (body != null) normalColor = body.color;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        GameManager gm = GameManager.Instance;
        if (gm.IsOver) return;
        if (!other.TryGetComponent(out Item item)) return;

        switch (item.Kind)
        {
            case ItemKind.Monster:
                Play(hitSfx);
                StartCoroutine(Flash());
                gm.TakeHit();
                break;
            case ItemKind.Key:
                Play(keySfx);
                gm.AddKey();
                break;
            case ItemKind.Heart:
                Play(heartSfx);
                gm.Heal();
                break;
            case ItemKind.Gift:
                Play(giftSfx);
                if (confettiPrefab != null)
                {
                    Instantiate(confettiPrefab, other.transform.position, Quaternion.identity);
                }
                gm.AddGift();
                break;
        }

        // Se destruye al tocarlo: así un monstruo no te saca dos vidas.
        Destroy(other.gameObject);
    }

    IEnumerator Flash()
    {
        if (body == null) yield break;
        body.color = Color.red;
        yield return new WaitForSeconds(0.12f);
        body.color = normalColor;
    }

    void Play(AudioClip clip)
    {
        if (clip != null && sfx != null)
        {
            sfx.PlayOneShot(clip);
        }
    }
}
