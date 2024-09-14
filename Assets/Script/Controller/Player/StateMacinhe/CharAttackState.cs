using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharAttackState : CharacterState
{
    public CharAttackState(CharacterStateMachine character) : base(character) { }

    public override void Enter()
    {
        _character.isAttack = true;
        _character.SetAnimState(CharacterStateMachine.CharFxState.Attack01);
        _character.Attack01();
        Debug.Log("<<<<<------------------Attack------------------->>>>>");
        _character.StartCoroutine(WaitForAnimationEnd());
    }

    public override void Update()
    {

    }

    public override void Exit()
    {
        _character.isAttack = false;
    }
    private IEnumerator WaitForAnimationEnd()
    {
        yield return new WaitForSeconds(11f / 60f); // Giả sử hoạt ảnh là 79 frame với tốc độ 60 FPS
        if (!_character.isDash && !_character.onDMG)
        {
            if (!_character.isWalk && !_character.isRunning && _character.isGround && !_character.onDMG)
            {
                _character.SetState(_character.IdleState);
            }
            else if ( _character.isGround)
            {
                _character.SetState(_character.MoveState);
            }
        }
    }
}
