using UnityEngine;

public class CharJumpState : CharacterState
{
    public CharJumpState(CharacterStateMachine character) : base(character) { }

    public override void Enter()
    {
        
        _character.isJump = true;
        Debug.Log("<<<<<------------------Jump------------------->>>>>");
    }

    public override void Update()
    {
        
        if (!_character.isDash && !_character.onDMG)
        {
            _character.MoveInput();
           


            if (_character.isClimbing && !_character.isGround)
            {
                _character.SetState(_character.ClimbState);
            }
            if (_character.rb.velocity.y < 0 && !_character.isGround)
            {
                _character.SetState(_character.FallState);
            }
        }
       
    }

    public override void Exit()
    {
        _character.isJump = false;
    }
}
