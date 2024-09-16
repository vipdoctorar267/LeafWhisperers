using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharAttackState : CharacterState
{
    public CharAttackState(CharacterStateMachine character) : base(character) { }

    public override void Enter()
    {
        Debug.Log("<<<<<------------------Attack------------------->>>>>");
        _character.StartCoroutine(WaitForAnimationEnd());
    }

    public override void Update()
    {

    }

    public override void Exit()
    {
       
    }
    private IEnumerator WaitForAnimationEnd()
    {
        if (!_character.isAttack)
        {
            _character.Attack01();
            _character.isAttack = true;
            

            yield return new WaitForSeconds(11f / 60f); 
            yield return new WaitForSeconds(0.1f);
            _character.isAttack = false;
            if (!_character.isDash && !_character.onDMG)
            {

                if (!_character.isWalk && !_character.isRunning && _character.isGround && !_character.onDMG)
                {
                    _character.SetState(_character.IdleState);
                }
                else if (_character.isGround)
                {
                    _character.SetState(_character.MoveState);
                }
            }
            yield return new WaitForSeconds(0.2f);
            _character.allowAttack01 = true;

        }
       
    }
}
