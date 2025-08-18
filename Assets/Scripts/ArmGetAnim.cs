using UnityEngine;

public class ArmGetAnim : MonoBehaviour
{
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetShoot(bool val)
    {
        anim.SetBool("IsShooting", val);
    }

    public void SetIdle(bool val)
    {
        anim.SetBool("IsMoving", val);
    }

    public void SetGround(bool val)
    {
        anim.SetBool("IsGrounded", val);
    }

    public void ChangeOrientation(bool val)
    {
        spriteRenderer.flipX = val;
    }
}
