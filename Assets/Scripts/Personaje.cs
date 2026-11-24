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
    [SerializeField] private PlayerHealth playerHealth;
    private GameManager gm;
    [Header("Hitboxes y Daño por Golpe (0 = Golpe 1, 1 = Golpe 2, 2 = Golpe 3)")]
    public GameObject[] hitboxesGolpes;
    public float[] danosPorGolpe = new float[] { 15f, 20f, 30f };


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ani = GetComponent<Animator>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        // context.performed equivale exactamente a Input.GetKeyDown
        if (context.performed && !atacando)
        {
            atacando = true;
            ani.SetTrigger(combo.ToString());
        }
    }

    public void ActivarHitbox(int indiceGolpe)
    {
        DesactivarTodasLasHitboxes();

        if (indiceGolpe >= 0 && indiceGolpe < hitboxesGolpes.Length && hitboxesGolpes[indiceGolpe] != null)
        {
            // Obtenemos el script HitboxAtaque del objeto y le pasamos el daño correspondiente
            HitboxAtaque hitbox = hitboxesGolpes[indiceGolpe].GetComponent<HitboxAtaque>();
            if (hitbox != null && indiceGolpe < danosPorGolpe.Length)
            {
                hitbox.danoActual = danosPorGolpe[indiceGolpe];
            }

            // Encendemos el objeto después de asignar el daño
            hitboxesGolpes[indiceGolpe].SetActive(true);
        }
    }

    public void Start_Combo()
    {
        atacando = false;
        if (combo < 3)
        {
            combo++;
        }
    }

    public void Finish_Ani()
    {
        atacando = false;
        combo = 0;
        DesactivarTodasLasHitboxes();
    }

    public void Combos_()
    {
        if (Input.GetKeyDown(KeyCode.A) && !atacando)
        {
            atacando = true;
            ani.SetTrigger(""+combo);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Combos_();
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

    public void makeDamage(float damageAmount)
    {
        if (playerHealth == null) return;

        playerHealth.TakeDamage(damageAmount);
    }

    public void CancelarCombo()
    {
        DesactivarTodasLasHitboxes();
        Finish_Ani();
    }

}
