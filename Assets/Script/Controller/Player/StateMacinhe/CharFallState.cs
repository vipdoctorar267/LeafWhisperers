using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharFallState : CharacterState
{
    public CharFallState(CharacterStateMachine character) : base(character) { }

    public override void Enter()
    {
        
        Debug.Log("<<<<<------------------Fall------------------->>>>>");
    }

    public override void Update()
    {
        
        _character.SetAnimState(CharacterStateMachine.CharFxState.Fall);
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
        
    }
}
