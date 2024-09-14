using UnityEngine;

public class CharMoveState : CharacterState
{
    public CharMoveState(CharacterStateMachine character) : base(character) { }

    public override void Enter()
    {
        _character.isWalk = true;
        //_character.animManager.TriggerWalk();
        Debug.Log("<<<<<------------------Walk------------------->>>>>");
    }

    public override void Update()
    {
        if (Input.GetMouseButtonDown(0) && _character.isGround)
        {
            _character.SetState(_character.AttackState);
        }
        if (!_character.isDash && !_character.isAttack && !_character.onDMG)
        {
            _character.MoveInput();
            if (_character.isWalk) _character.SetAnimState(CharacterStateMachine.CharFxState.Walk);     
            else if(_character.isRunning) _character.SetAnimState(CharacterStateMachine.CharFxState.Run);
           
            if (!_character.isWalk && !_character.isRunning && _character.isGround && !_character.onDMG)
            {
                _character.SetState(_character.IdleState);
            }
           

            if (_character.isClimbing && !_character.isGround)
            {
                _character.SetState(_character.ClimbState);
            }

        }
           
    }

    public override void Exit()
    {
        _character.isWalk = false;
    }
}
