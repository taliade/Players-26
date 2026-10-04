using UnityEngine;

// Requisito: Colisiones + Condicionales + SFX.
// Va en el Player. Detecta qué tocó, suena el efecto y le avisa al GameManager.
[RequireComponent(typeof(AudioSource))]
public class TriggerHandler : MonoBehaviour
{
    [SerializeField] AudioClip hitSfx;
    [SerializeField] AudioClip keySfx;
    [SerializeField] AudioClip heartSfx;
    [SerializeField] AudioClip giftSfx;

    AudioSource sfx;

    void Awake()
    {
        sfx = GetComponent<AudioSource>();
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
                gm.AddGift();
                break;
        }

        // Se destruye al tocarlo: así un monstruo no te saca dos vidas.
        Destroy(other.gameObject);
    }

    void Play(AudioClip clip)
    {
        if (clip != null)
        {
            sfx.PlayOneShot(clip);
        }
    }
}
