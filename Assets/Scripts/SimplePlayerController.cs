using UnityEngine;
using Unity.Netcode;

public class SimplePlayerController : NetworkBehaviour
{
    public NetworkVariable<ulong> PlayerID;

    public NetworkVariable<int> Life;

    public float JumpForce = 5;
    public float Speed = 10;

    private Animator animator;
    private Rigidbody rb;
    public LayerMask groundLayer;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Update()
    {
        if (!IsOwner) return;

        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            float VelX = Input.GetAxisRaw("Horizontal") * Speed * Time.deltaTime;
            float VelY = Input.GetAxisRaw("Vertical") * Speed * Time.deltaTime;
            transform.position += new Vector3(VelX, 0, VelY);
        }

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb = GetComponent<Rigidbody>();
            rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            JumpTriggerRpc("Jump");
        }

        CheckGroundRpc();
    }

    [Rpc(SendTo.Server)]
    public void JumpTriggerRpc(string animationName)
    {
        animator.SetTrigger(animationName);
    }

    [Rpc(SendTo.Server)]
    public void CheckGroundRpc()
    {
        bool grounded = Physics.Raycast(transform.position, Vector3.down, 1f, groundLayer);
        animator.SetBool("Grounded", grounded);
        animator.SetBool("FreeFall", !grounded);
    }
    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1f, groundLayer);
    }
}