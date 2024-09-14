using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbSpiderAnimController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private ClimbSpider climbSpider; // Gán trong Inspector

    private void Start()
    {
        animator = GetComponent<Animator>(); // Tìm Animator trong object ClimbSpider
    }

    private void Update()
    {
        UpdateAnimations(climbSpider.currentState);
    }

    public void UpdateAnimations(ClimbSpider.ClimbSpiderState state)
    {
        switch (state)
        {
            case ClimbSpider.ClimbSpiderState.Hang:
                animator.Play("Hang");
                break;
            case ClimbSpider.ClimbSpiderState.Descend:
                animator.Play("Hang");
                break;
            case ClimbSpider.ClimbSpiderState.Attack:
                animator.SetTrigger("Attack");
                break;
            case ClimbSpider.ClimbSpiderState.OnDMG:
                animator.SetTrigger("OnDMG");
                break;
            case ClimbSpider.ClimbSpiderState.Dead:
                if(climbSpider.isGround) animator.SetTrigger("DeadOnSpace");
                else animator.SetTrigger("DeadOnGround");
                break;
            default:
                break;
        }
    }
}
