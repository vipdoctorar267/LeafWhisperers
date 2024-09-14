using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunSpiderAnimController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private RunSpider runSpider; // Gán trong Inspector

    private void Start()
    {
        animator = GetComponent<Animator>(); // Tìm Animator trong object RunSpider
    }

    private void Update()
    {
        UpdateAnimations(runSpider._currentState);
    }

    public void UpdateAnimations(RunSpider.RunSpiderState state)
    {
        switch (state)
        {
            case RunSpider.RunSpiderState.Wander:
                animator.Play("Wander");
                break;
            case RunSpider.RunSpiderState.Chase:
                animator.Play("Chase");
                break;
            case RunSpider.RunSpiderState.Dash:
                animator.Play("Dash");
                break;
            case RunSpider.RunSpiderState.Idle:
                animator.Play("Idle");
                break;
            case RunSpider.RunSpiderState.OnDMG:
                animator.SetTrigger("OnDMG");
                break;
            case RunSpider.RunSpiderState.Dead:
                animator.SetTrigger("Dead");
                break;
            default:
                break;
        }
    }
}
