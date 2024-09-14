using System.Collections;
using UnityEngine;

public class CharDashState : CharacterState
{
    

    public CharDashState(CharacterStateMachine character) : base(character) { }

    public override void Enter()
    {
        Debug.Log("<<<<<------------------Dash------------------->>>>>");
        _character.isDash = true; // Đặt trạng thái đang Dash
        _character.dashTimer = _character.dashDuration; // Đặt thời gian Dash
        _character.dashCDTimer = _character.dashCD; // Đặt thời gian hồi chiêu Dash
        _character.allowDash = false; // Không cho phép Dash tiếp theo cho đến khi thời gian hồi chiêu kết thúc
        _character.rb.gravityScale = 0;
        _character.rb.velocity = new Vector2(_character.rb.velocity.x, 0);// Đặt lại vận tốc y trước khi Dash 
        _character.rb.velocity = new Vector2(_character.dashStrength * _character.direction, _character.rb.velocity.y); // Cập nhật vận tốc Dash
        Debug.Log("huong" + _character.direction);
       
    }

    public override void Update()
    {
        _character.SetAnimState(CharacterStateMachine.CharFxState.Dash);
        if (_character.isGround && !_character.isDash)
        {
            _character.SetState(_character.IdleState);
        }

        if (_character.isClimbing && !_character.isGround && !_character.isDash)
        {
            _character.SetState(_character.ClimbState);
        }
        if (!_character.isClimbing && !_character.isGround && !_character.isDash)
        {
            _character.SetState(_character.FallState);
        }
    }

    public override void Exit()
    {
       
    }

   
}
