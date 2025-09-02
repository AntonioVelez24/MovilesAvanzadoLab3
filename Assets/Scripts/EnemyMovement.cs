using Unity.Netcode;
using UnityEngine;

public class EnemyMovement : NetworkBehaviour
{
    public float speed = 3f;
    public float detectionRadius = 8f;
    public Transform playerTarget;

    public override void OnNetworkSpawn()
    {
        playerTarget = null;
    }

    public void SearchPlayer()
    {
        RaycastHit hit;

        for (int i = 0; i < 10f; i++)
        {
            Vector3 randomDirection = Random.onUnitSphere; 

            if (Physics.Raycast(transform.position, randomDirection, out hit, detectionRadius))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    playerTarget = hit.transform;
                    Debug.Log("Jugador detectado: " + playerTarget.name);
                    break; 
                }
            }
        }
    }

    private void Update()
    {
        if (playerTarget == null)
        {
            SearchPlayer();
        }

        if (playerTarget != null)
        {
            Vector3 direction = (playerTarget.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ulong playerID = collision.gameObject.GetComponent<NetworkObject>().OwnerClientId;
            CollisionWithPlayerRpc(playerID);
        }
    }

    [Rpc(SendTo.Server)]
    public void CollisionWithPlayerRpc(ulong playerID)
    {
        GetComponent<NetworkObject>().Despawn(true);
    }
}