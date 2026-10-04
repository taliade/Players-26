using System.Collections;
using UnityEngine;

// El corazón del HUD late dos veces (pum-pum) cuando recuperás una vida.
public class HeartPulse : MonoBehaviour
{
    [SerializeField] float bigScale = 1.4f;
    [SerializeField] float beatTime = 0.12f;

    Vector3 baseScale;
    Coroutine running;

    void Awake()
    {
        baseScale = transform.localScale;
    }

    public void Pulse()
    {
        if (running != null) StopCoroutine(running);
        transform.localScale = baseScale;
        running = StartCoroutine(Beat());
    }

    IEnumerator Beat()
    {
        yield return ScaleTo(bigScale);          // pum
        yield return ScaleTo(1f);
        yield return ScaleTo(bigScale - 0.15f);  // pum (un poco más chico, como un latido real)
        yield return ScaleTo(1f);
        running = null;
    }

    IEnumerator ScaleTo(float factor)
    {
        Vector3 from = transform.localScale;
        Vector3 to = baseScale * factor;
        float t = 0f;
        while (t < beatTime)
        {
            t += Time.unscaledDeltaTime;
            transform.localScale = Vector3.Lerp(from, to, t / beatTime);
            yield return null;
        }
        transform.localScale = to;
    }
}
