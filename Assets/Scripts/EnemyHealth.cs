using Unity.Netcode;
using UnityEngine;

public class EnemyHealth : NetworkBehaviour
{
    private float maxHealth = 5;
    public NetworkVariable<float> currentHealth = new NetworkVariable<float>();

    private void Start()
    {
        currentHealth.Value = maxHealth;
    }
    [Rpc(SendTo.Server)]
    public void TakeDamageRpc(float damage)
    {
        currentHealth.Value -= damage;
        if(currentHealth.Value <= 0)
        {
            DestroyEnemy();
        }
    }
    public void DestroyEnemy()
    {
        GetComponent<NetworkObject>().Despawn();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            TakeDamageRpc(1);
            NetworkObject bullet = other.GetComponent<NetworkObject>();
            bullet.Despawn();
            Debug.Log("Vida del enemigo: " + currentHealth.Value);
        }
    }
}