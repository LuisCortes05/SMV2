using UnityEngine;
using UnityEngine.InputSystem;

public class GolpeCargado : MonoBehaviour
{
    private Animator animator;
    private PlayerMovement playerMovement;
    private bool manteniendoBoton = false;
    private bool esperandoSuelta = false;

    [Header("Configuración de Carga y Daño")]
    public float tiempoMaximoCarga = 2.0f;
    public float danoBase = 5f;
    public float danoMaximo = 30f;
    private float temporizadorCarga = 0f;

    [Header("Hitboxes de los 3 Golpes")]
    public HitboxAtaque[] hitboxes; // Arrastra aquí Hitbox_Golpe1, 2 y 3 en el Inspector

    void Awake()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (esperandoSuelta)
        {
            temporizadorCarga += Time.deltaTime;

            // Si llega al tiempo máximo de carga, libera automáticamente
            if (temporizadorCarga >= tiempoMaximoCarga)
            {
                LiberarGolpe();
            }
        }
    }

    // --- MÉTODOS PARA CONECTAR EN PLAYER INPUT (Invoke Unity Events) ---

    public void OnGolpe1(InputAction.CallbackContext context)
    {
        ProcesarInputGolpe(context, "Golpe1");
    }

    public void OnGolpe2(InputAction.CallbackContext context)
    {
        ProcesarInputGolpe(context, "Golpe2");
    }

    public void OnGolpe3(InputAction.CallbackContext context)
    {
        ProcesarInputGolpe(context, "Golpe3");
    }

    private void ProcesarInputGolpe(InputAction.CallbackContext context, string triggerGolpe)
    {
        if (context.started)
        {
            Debug.Log($"[INPUT] Botón presionado: {context.action.name} -> Intentando lanzar: {triggerGolpe}");
            if (manteniendoBoton) return;
            IniciarGolpe(triggerGolpe);
        }
        // Equivale a Input.GetKeyUp (instante en que se suelta el botón)
        else if (context.canceled)
        {
            if (esperandoSuelta)
            {
                LiberarGolpe();
            }
            else
            {
                manteniendoBoton = false;
            }
        }
    }

    // --- LÓGICA DE COMBATE Y CARGA ---

    private void IniciarGolpe(string triggerGolpe)
    {
        manteniendoBoton = true;
        esperandoSuelta = false;
        temporizadorCarga = 0f;
        animator.speed = 1f;
        ApagarTodosLosHitboxes();

        // Bloquea el movimiento y el salto al iniciar el ataque
        if (playerMovement != null)
            playerMovement.canMove = false;

        animator.SetTrigger(triggerGolpe);
    }

    public void CancelarAtaque()
    {
        manteniendoBoton = false;
        esperandoSuelta = false;
        temporizadorCarga = 0f;
        animator.speed = 1f;
        ApagarTodosLosHitboxes();

        // Restaura el movimiento si el ataque es cancelado (ej. al recibir daño)
        if (playerMovement != null)
            playerMovement.canMove = true;
    }

    public void PausarEnCarga()
    {
        if (manteniendoBoton)
        {
            animator.speed = 0f;
            esperandoSuelta = true;
        }
    }

    private void LiberarGolpe()
    {
        manteniendoBoton = false;
        esperandoSuelta = false;

        // Calcula qué porcentaje del tiempo máximo cargaste (valor entre 0.0 y 1.0)
        float porcentajeCarga = Mathf.Clamp01(temporizadorCarga / tiempoMaximoCarga);

        // Interpola linealmente entre el daño base (0% carga) y el daño máximo (100% carga)
        float danoCalculado = Mathf.Lerp(danoBase, danoMaximo, porcentajeCarga);

        // Le pasa el daño calculado a todos los hitboxes antes de que se activen
        foreach (HitboxAtaque hb in hitboxes)
        {
            hb.danoActual = Mathf.Round(danoCalculado);
        }

        animator.speed = 1f;
    }

    // --- FUNCIONES PARA LOS ANIMATION EVENTS ---

    // Llámalo en el Frame 2 (Impacto). Usa 0 para Golpe1, 1 para Golpe2, 2 para Golpe3
    public void ActivarHitbox(int indiceHitbox)
    {
        if (indiceHitbox >= 0 && indiceHitbox < hitboxes.Length)
        {
            hitboxes[indiceHitbox].gameObject.SetActive(true);
        }
    }

    // Llámalo en el Frame 3 (Recuperación) para cerrar la ventana de daño
    public void ApagarTodosLosHitboxes()
    {
        foreach (HitboxAtaque hb in hitboxes)
        {
            hb.gameObject.SetActive(false);
        }

        if (playerMovement != null)
            playerMovement.canMove = true;
    }


}