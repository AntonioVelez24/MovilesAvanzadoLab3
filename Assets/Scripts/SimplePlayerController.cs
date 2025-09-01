using UnityEngine;
using Unity.Netcode;

public class SimplePlayerController : NetworkBehaviour
{
    public NetworkVariable<ulong> PlayerID;

    public float jumpForce;
    public float speed;

    private Animator animator;
    private Rigidbody rb;
    
    public LayerMask groundLayer;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if(Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            float velX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
            float velY = Input.GetAxis("Vertical") * speed * Time.deltaTime;

            UpdatePositionRpc(velX, velY);
        }
        CheckGroundRpc();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            JumpTriggerRpc("Jump");
        }
    }
    [Rpc(SendTo.Server)]
    public void UpdatePositionRpc(float x, float y)
    {
        transform.position += new Vector3(x, 0, y);
    }
    [Rpc(SendTo.Server)]
    public void JumpTriggerRpc(string animationName)
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        animator.SetTrigger(animationName);
    }
    [Rpc(SendTo.Server)]
    public void CheckGroundRpc()
    {
        if (Physics.Raycast(transform.position, Vector3.down, 1.5f, groundLayer))
        {
            animator.SetBool("Grounded", true);
            animator.SetBool("FreeFall", false);
        }
        else
        {
            animator.SetBool("Grounded", false);
            animator.SetBool("FreeFall", true);
        }
    }
}
