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
            // Attack01
            _character.Attack01();
            _character.isAttack = true;
            ChangeState();  
            yield return new WaitForSeconds(11f / 60f); 
            yield return new WaitForSeconds(0.1f);  
            _character.isAttack = false;
            _character.allowAttack02 = true;
            float comboTime = 0.5f;
            float elapsedTime = 0f;
            while (elapsedTime < comboTime)
            {
                if (Input.GetMouseButtonDown(0)) 
                {
                    // Attack02
                    _character.Attack02();
                    _character.isAttack = true;
                    ChangeState();  
                    yield return new WaitForSeconds(11f / 60f); 
                    yield return new WaitForSeconds(0.1f);
                    _character.isAttack = false;
                    _character.allowAttack03 = true;
                    _character.allowAttack02 = false; 
                    elapsedTime = 0f;
                    while (elapsedTime < comboTime)
                    {
                        if (Input.GetMouseButtonDown(0)) 
                        {
                            // Attack03
                            _character.Attack03();
                            _character.isAttack = true;
                            ChangeState();  
                            yield return new WaitForSeconds(11f / 60f); 
                            yield return new WaitForSeconds(0.1f);
                            _character.isAttack = false;
                            _character.allowAttack03 = false; 
                            _character.allowAttack01 = true;  
                            yield break;
                        }
                        elapsedTime += Time.deltaTime;
                        yield return null;
                    }

                    _character.allowAttack01 = true;
                    yield break;
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Nếu không có lần tấn công thứ hai, cho phép bắt đầu lại combo
            _character.allowAttack01 = true;
        }
    }

    //---------------------------------------------------------
    private void ChangeState()
    {
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
    }
}
