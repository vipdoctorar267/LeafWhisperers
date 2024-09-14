using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharAnimManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterStateMachine characterStateMachine; // Gán trong Inspector


    private void Start()
    {
        animator = GetComponent<Animator>(); // Tìm Animator trong object PlayerAvatar
       
    }

    private void Update()
    {
        UpdateAnimations(characterStateMachine.CurrentFxState);
    }

    public void UpdateAnimations(CharacterStateMachine.CharFxState animState)
    {
        switch (animState)
        {
            case CharacterStateMachine.CharFxState.Idle:
                animator.Play("Idle");
                break;
            case CharacterStateMachine.CharFxState.Walk:
                animator.Play("Walk");
                break;
            case CharacterStateMachine.CharFxState.Run:
                animator.Play("Run");
                break;
            case CharacterStateMachine.CharFxState.Jump:
                animator.SetTrigger("Jump");
                break;
            case CharacterStateMachine.CharFxState.Attack01:
                animator.SetTrigger("Attack01");
                break;
            case CharacterStateMachine.CharFxState.Climb:
                animator.Play("Climb");
                break;
            case CharacterStateMachine.CharFxState.Dash:
                animator.SetTrigger("Dash");
                break;
            case CharacterStateMachine.CharFxState.Fall:
                animator.Play("Fall");
                break;
            case CharacterStateMachine.CharFxState.OnDMG:
                animator.SetTrigger("OnDMG");
                break;
            case CharacterStateMachine.CharFxState.Dead:
                animator.SetTrigger("Dead");
                break;
            default:
                break;
        }
    }

   

}
