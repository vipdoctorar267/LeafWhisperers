using UnityEngine;

public class RunSpiderWanderState : RunSpiderState
{
    public RunSpiderWanderState(RunSpiderStateMachine runSpider) : base(runSpider) { }

    public override void Enter()
    {
        _runSpider.currentStateType = RunSpiderStateMachine.SpiderState.Wander;
        Debug.Log("<<<<<------------------Wander------------------->>>>>");
    }

    public override void Update()
    {
        Wander();
    }

    private void Wander()
    {
        float wanderSpeedThisFrame = _runSpider.WanderSpeed * Time.deltaTime;

        if (_runSpider.IsMovingRight)
        {
            _runSpider.transform.Translate(Vector2.right * wanderSpeedThisFrame);
            if (_runSpider.transform.position.x >= _runSpider.PatrolEndPos)
            {
                _runSpider.IsMovingRight = false;
            }
        }
        else
        {
            _runSpider.transform.Translate(Vector2.left * wanderSpeedThisFrame);
            if (_runSpider.transform.position.x <= _runSpider.PatrolStartPos)
            {
                _runSpider.IsMovingRight = true;
            }
        }
    }

    public override void Exit()
    {
        // Logic khi thoát kh?i tr?ng thái này, n?u c?n
    }
}
