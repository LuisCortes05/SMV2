using UnityEngine;
using UnityEngine.InputSystem;

public class Personaje : MonoBehaviour
{
    [Header("Hitbox del Combo")]
    public GameObject hitboxObjeto;
    public HitboxAtaque hitboxScript;
    public Animator ani;
    public int combo;
    public bool atacando;

    [Header("Configuración de Combo")]
    [SerializeField] private int totalGolpesCombo = 3; // Índices válidos: 0, 1, 2
    [SerializeField] private float maxTiempoPorGolpe = 1.2f; // Seguro anti-bloqueo
    private float temporizadorAtaque = 0f;

    [SerializeField] private PlayerHealth playerHealth;
    private GameManager gm;

    [Header("Hitboxes y Daño por Golpe (0 = Golpe 1, 1 = Golpe 2, 2 = Golpe 3)")]
    public GameObject[] hitboxesGolpes;
    public float[] danosPorGolpe = new float[] { 15f, 20f, 30f };

    private void Awake()
    {
        ani = GetComponent<Animator>();
    }

    private void Start()
    {
        ReiniciarEstadoCombate();
    }

    private void Update()
    {
        // Seguro anti-bloqueo: si una transición del Animator se salta Finish_Ani(),
        // libera el estado automáticamente al cumplirse el tiempo máximo del golpe.
        if (atacando)
        {
            temporizadorAtaque += Time.deltaTime;
            if (temporizadorAtaque >= maxTiempoPorGolpe)
            {
                Finish_Ani();
            }
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed || atacando) return;

        // Evita que combo exceda los golpes disponibles (0, 1, 2)
        if (combo >= totalGolpesCombo)
        {
            combo = 0;
        }

        atacando = true;
        temporizadorAtaque = 0f;

        LimpiarTriggersAnimator();
        ani.SetTrigger(combo.ToString());
    }

    public void ActivarHitbox(int indiceGolpe)
    {
        DesactivarTodasLasHitboxes();

        if (indiceGolpe >= 0 && indiceGolpe < hitboxesGolpes.Length && hitboxesGolpes[indiceGolpe] != null)
        {
            HitboxAtaque hitbox = hitboxesGolpes[indiceGolpe].GetComponent<HitboxAtaque>();
            if (hitbox != null && indiceGolpe < danosPorGolpe.Length)
            {
                hitbox.danoActual = danosPorGolpe[indiceGolpe];
            }

            hitboxesGolpes[indiceGolpe].SetActive(true);
        }
    }

    public void Start_Combo()
    {
        // Solo habilita el siguiente golpe si aún no llegamos al último del combo
        if (combo < totalGolpesCombo - 1)
        {
            atacando = false;
            temporizadorAtaque = 0f;
            combo++;
        }
    }

    public void Finish_Ani()
    {
        atacando = false;
        combo = 0;
        temporizadorAtaque = 0f;
        LimpiarTriggersAnimator();
        DesactivarTodasLasHitboxes();
    }

    private void LimpiarTriggersAnimator()
    {
        if (ani == null) return;

        for (int i = 0; i < totalGolpesCombo; i++)
        {
            ani.ResetTrigger(i.ToString());
        }
    }

    public void DesactivarTodasLasHitboxes()
    {
        if (hitboxesGolpes == null) return;

        for (int i = 0; i < hitboxesGolpes.Length; i++)
        {
            if (hitboxesGolpes[i] != null)
            {
                hitboxesGolpes[i].SetActive(false);
            }
        }
    }

    public void SetRivalHealth(PlayerHealth rival)
    {
        playerHealth = rival;
    }

    public void makeDamage(float damageAmount)
    {
        if (playerHealth == null) return;
        playerHealth.TakeDamage(damageAmount);
    }

    public void CancelarCombo()
    {
        Finish_Ani();
    }

    public void ReiniciarEstadoCombate()
    {
        Finish_Ani();
        if (ani != null)
        {
            ani.Rebind();
            ani.Update(0f);
        }
    }
}
