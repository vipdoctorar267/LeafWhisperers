using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationManager : MonoBehaviour
{
    [SerializeField] private Animator animator; // Sử dụng SerializeField với biến private

    private void Start()
    {
        animator = GetComponent<Animator>(); // Tìm Animator trong object PlayerAvatar
    }

    public void UpdateAnimations(bool isGround, bool isWalk, bool isRunning, bool isClimbing,bool isFall,bool isDash)
    {
        // Cập nhật các biến trạng thái cho Animator
        animator.SetBool("isGround", isGround);
        animator.SetBool("isWalk", isWalk);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isClimbing", isClimbing);
        animator.SetBool("isFall", isFall);

        // Chuyển đổi giữa các trạng thái hoạt ảnh dựa trên giá trị moveInput và isRunning
        if (isDash) return;

        if (isClimbing)
        {
            animator.Play("Climb");
        }else if (isFall)
        {
            animator.Play("Fall");
        }
        else if (isRunning && isGround)
        {
            animator.Play("Run");
        }
        else if (isWalk && isGround)
        {
            animator.Play("Walk");
        }
        else if (isGround)
        {
            animator.Play("Idle");
        }
    }

    public void TriggerJump()
    {
        animator.SetTrigger("Jump");
    }

    public void TriggerClimbJump()
    {
        animator.SetTrigger("ClimbJump");
    }

    public void TriggerSpaceJump()
    {
        animator.SetTrigger("SpaceJump");
    }

    public void TriggerJumpToFall()
    {
        animator.SetTrigger("JumpToFall");
    }

    public void TriggerDash()
    {
        animator.SetTrigger("Dash");
    }

    public void TriggerFallToIdle()
    {
        animator.SetTrigger("FallToIdle");
    }


    public bool IsPlaying(string animationName)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(animationName);
    }

    public void SetIsMeleeAttack(bool isMeleeAttack)
    {
        animator.SetBool("IsMeleeAttack", isMeleeAttack);
        if (isMeleeAttack)
        {
            animator.SetTrigger("MeleeAttack");
        }
    }

    public void SetIsRangeAttack(bool isRangeAttack)
    {
        animator.SetBool("IsRangeAttack", isRangeAttack);
        if (isRangeAttack)
        {
            animator.SetTrigger("RangeAttack");
        }
    }

    public void SetIsAttack(bool isAttack)
    {
        animator.SetBool("IsAttack", isAttack);
    }
}
