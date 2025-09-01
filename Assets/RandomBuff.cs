using Unity.Netcode;
using UnityEngine;

public class RandomBuff : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<NetworkObject>();
            AddBuffToPlayerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }
    [Rpc(SendTo.Server)]
    private void AddBuffToPlayerRpc(ulong playerID)
    {
        print("Aplicar buff a :" + playerID);
        GetComponent<NetworkObject>().Despawn(true);
    }
}
