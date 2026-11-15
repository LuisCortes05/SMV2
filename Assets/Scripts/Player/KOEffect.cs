using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class KOEffect : MonoBehaviour
{
    [Header("KO Effect Settings")]
    [SerializeField] private VisualEffect KOEffectVFX;
    [SerializeField] private float effectDuration = 2.5f;
    [SerializeField] private bool moveToDefeatedPlayer = false; //Si se necesita en la posicion del jugador derrotado sea activa
                                                                //Si no se deja desactivado

    private static readonly int ActiveEventID = Shader.PropertyToID("Active");
    private static readonly int DisableEventID = Shader.PropertyToID("Disable");

    private WaitForSeconds waitDuration;
    private Coroutine disableCoroutine;

    private void Awake()
    {
        waitDuration = new WaitForSeconds(effectDuration);

        if (KOEffectVFX != null)
        {
            KOEffectVFX.Stop();
        }
    }

    public void PlayKOEffect(Vector3? targetPosition = null)
    {
        if (KOEffectVFX == null) return;

        if(moveToDefeatedPlayer && targetPosition.HasValue)
        {
            KOEffectVFX.transform.position = targetPosition.Value;
        }

        if(disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
        }

        KOEffectVFX.SendEvent(ActiveEventID);
        Debug.Log("Event Avtive");
        disableCoroutine = StartCoroutine(DisableAfterDuration());
    }

    public void StopKOEffect()
    {
        if(disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
            disableCoroutine = null;
        }

        if(KOEffectVFX != null)
        {
            KOEffectVFX.SendEvent(DisableEventID);
        }
    }

    private IEnumerator DisableAfterDuration()
    {
        yield return waitDuration;

        if(KOEffectVFX != null)
        {
            KOEffectVFX.SendEvent(DisableEventID);
            Debug.Log("Event Disable");
        }

        disableCoroutine = null;
    }
}
