using UnityEngine;
using System.Collections.Generic;

public class HitboxAtaque : MonoBehaviour
{
    [HideInInspector] public float danoActual;
    private PlayerHealth miSalud;
    private List<GameObject> objetivosGolpeados = new List<GameObject>();

    private void Awake()
    {
        // Busca el PlayerHealth en el objeto padre (tu personaje)
        miSalud = GetComponentInParent<PlayerHealth>();
    }

    private void OnEnable()
    {
        objetivosGolpeados.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Busca PlayerHealth en el collider golpeado o en su objeto padre
        PlayerHealth saludOponente = collision.GetComponentInParent<PlayerHealth>();

        // Verifica que exista, que no seas tú mismo y que no haya sido golpeado ya en este ataque
        if (saludOponente != null && saludOponente != miSalud && !objetivosGolpeados.Contains(saludOponente.gameObject))
        {
            objetivosGolpeados.Add(saludOponente.gameObject);
            saludOponente.TakeDamage(danoActual);
        }
    }
}
