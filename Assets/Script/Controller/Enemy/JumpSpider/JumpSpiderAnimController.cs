using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpSpiderAnimController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private JumpSpider jumpSpider; // Gán trong Inspector

    private void Start()
    {
        animator = GetComponent<Animator>(); // Tìm Animator trong object JumpSpider
    }

    private void Update()
    {
        UpdateAnimations(jumpSpider.currentState);
    }

    public void UpdateAnimations(JumpSpider.JumpSpiderState state)
    {
        switch (state)
        {
            case JumpSpider.JumpSpiderState.Idle:
                animator.Play("Idle");
                break;
            case JumpSpider.JumpSpiderState.Jump:
                animator.SetTrigger("Jump");
                break;
            case JumpSpider.JumpSpiderState.Fall:
                animator.SetTrigger("Fall");
                break;
            case JumpSpider.JumpSpiderState.OnDMG:
                if (jumpSpider.isGround) animator.SetTrigger("OnDMGGround");
                else animator.SetTrigger("OnDMGSpace");
                break;
            case JumpSpider.JumpSpiderState.Dead:
                animator.SetTrigger("Dead");
                break;
            default:
                break;
        }
    }
}
