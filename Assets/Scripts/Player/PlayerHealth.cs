using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int playerID = 1;
    public float maxHealth = 100f;
    private float currentHealth;

    public GameManager gm;

    private void Start()
    {
        currentHealth = maxHealth;

        gm.UpdateHealtBar(playerID, currentHealth, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        gm.UpdateHealtBar(playerID, currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Debug.Log($"Jugador {playerID} perdio");
            gm.PlayerDefeated(playerID);
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        gm.UpdateHealtBar(playerID, currentHealth, maxHealth);
    }
}
