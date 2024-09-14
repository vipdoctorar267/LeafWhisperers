using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharOnDMGState : CharacterState
{
     public CharOnDMGState(CharacterStateMachine character) : base(character) { }

    public override void Enter()
    {
       
        Debug.Log("<<<<<------------------OnDMG------------------->>>>>");
        _character.SetAnimState(CharacterStateMachine.CharFxState.OnDMG);
        _character.StartCoroutine(WaitOnDMGAnimEnd());
    }

    public override void Update()
    {

    }

    public override void Exit()
    {

    }
    private IEnumerator WaitOnDMGAnimEnd()
    {
        yield return new WaitForSeconds(13f / 60f); // Thời gian hoạt ảnh knockback
        _character.onDMG = false;
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
