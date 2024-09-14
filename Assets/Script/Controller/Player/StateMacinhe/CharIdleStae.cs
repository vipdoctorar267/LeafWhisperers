using UnityEngine;

public class CharIdleState : CharacterState
{
    public CharIdleState(CharacterStateMachine character) : base(character) { }

    public override void Enter()
    {
        _character.rb.gravityScale = 0;
        _character.rb.velocity = Vector2.zero;
        _character.isIdle = true;
        //_character.animManager.TriggerIdle();
        Debug.Log("<<<<<------------------Idle------------------->>>>>");
    }

    public override void Update()
    {
        if (_character.onDMG) _character.SetState(_character.OnDMGState);
        if (!_character.isGround && !_character.isClimbing) _character.SetState(_character.FallState);
        if (!_character.isDash && !_character.isAttack && !_character.onDMG)
        {
            _character.SetAnimState(CharacterStateMachine.CharFxState.Idle);
            if (_character.isWalk || _character.isRunning)
            {
                _character.SetState(_character.MoveState);
            }
           
        }
       
    }

    public override void Exit()
    {
        _character.isIdle = false;
        _character.rb.gravityScale = _character.customGravityScale;
    }
}
