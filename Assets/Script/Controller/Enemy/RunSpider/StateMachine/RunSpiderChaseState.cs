// RunSpiderChaseState.cs
using UnityEngine;
public class RunSpiderChaseState : RunSpiderState
{
    private Transform _player;

    public RunSpiderChaseState(RunSpiderStateMachine runSpider) : base(runSpider) { }

    public void SetPlayerTransform(Transform player)
    {
        _player = player;
    }

    public override void Enter()
    {
        
        _runSpider.currentStateType = RunSpiderStateMachine.SpiderState.Chase;
        Debug.Log("<<<<<------------------Chase------------------->>>>>");
    }

    public override void Update()
    {
        Chase();
    }

    private void Chase()
    {
        float chaseSpeedThisFrame = _runSpider.ChaseSpeed * Time.deltaTime;

        if (_player.position.x <= _runSpider.transform.position.x)
        {
            _runSpider.transform.Translate(Vector2.left * chaseSpeedThisFrame);
        }
        else if (_player.position.x >= _runSpider.transform.position.x)
        {
            _runSpider.transform.Translate(Vector2.right * chaseSpeedThisFrame);
        }
    }

    public override void Exit()
    {
        // Logic khi thoát khỏi trạng thái này, nếu cần
    }
}
