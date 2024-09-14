using UnityEngine;

public class CharClimbState : CharacterState
{
    public CharClimbState(CharacterStateMachine character) : base(character) { }

    public override void Enter()
    {
        _character.isClimbing = true;
        _character.rb.velocity = new Vector2(0,0);
        _character.rb.gravityScale = 0;
        //_character.animManager.TriggerClimb();
        Debug.Log("<<<<<------------------Climb------------------->>>>>");
    }

    public override void Update()
    {
        
        _character.SetAnimState(CharacterStateMachine.CharFxState.Climb);
        
        if (!_character.isClimbing && !_character.isJump)
        {
            _character.rb.gravityScale = _character.customGravityScale;
            _character.SetState(_character.FallState);
        }

        if (_character.isGround && _character.isClimbing)
        {
            _character.SetState(_character.ClimbState);
        }
    }

    public override void Exit()
    {
        _character.isClimbing = false;
        _character.rb.gravityScale = _character.customGravityScale;
    }
}
