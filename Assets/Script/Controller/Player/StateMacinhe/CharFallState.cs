using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharFallState : CharacterState
{
    public CharFallState(CharacterStateMachine character) : base(character) { }

    public override void Enter()
    {
        _character.isFall = true;
        Debug.Log("<<<<<------------------Fall------------------->>>>>");
    }

    public override void Update()
    {
        _character.MoveInput();
        if (!_character.isAttack) _character.SetAnimState(CharacterStateMachine.CharFxState.Fall);

        if (_character.isGround && !_character.isDash && !_character.onDMG)
        {
            _character.SetState(_character.IdleState);
        }
        

        if (_character.isClimbing && !_character.isGround)
        {
            _character.SetState(_character.ClimbState);
        }
        
    }

    public override void Exit()
    {
        _character.isFall = false;
    }
}
