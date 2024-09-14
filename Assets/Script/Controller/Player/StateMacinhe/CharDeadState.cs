using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharDeadState : CharacterState
{
    
    public CharDeadState(CharacterStateMachine character) : base(character) { }

    public override void Enter()
    {
        Debug.Log("<<<<<------------------Dead------------------->>>>>");
        _character.SetAnimState(CharacterStateMachine.CharFxState.Dead);
        
    }

    public override void Update()
    {
        if (!_character.isDead)
        {
            if (_character.isGround && !_character.isDash)
            {
                _character.SetState(_character.IdleState);
            }


            if (_character.isClimbing && !_character.isGround)
            {
                _character.SetState(_character.ClimbState);
            }
            if (_character.rb.velocity.y < 0 && !_character.isGround && !_character.isClimbing)
            {
                _character.SetState(_character.FallState);
            }
        }

    }

    public override void Exit()
    {
        
    }
   
}
