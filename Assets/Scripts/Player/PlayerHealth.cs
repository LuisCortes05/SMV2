using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int playerID = 1;
    public float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    public GameManager gm;
    public float CurrentHealth => currentHealth;

    private void Start()
    {
        ResetHealth();
    }

    public void TakeDamage(float amount)
    {
        if(currentHealth <= 0) return;
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        gm.UpdateHealthBar(playerID, currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Debug.Log($"Jugador {playerID} perdio");
            gm.PlayerDefeated(playerID);
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        if(gm != null)
        {
            gm.UpdateHealthBar(playerID, currentHealth, maxHealth);
        }
    }
}
