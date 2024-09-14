using System.Collections;
using UnityEngine;

public class RunSpiderDashState : RunSpiderState
{
    private Transform _player;

    public RunSpiderDashState(RunSpiderStateMachine runSpider) : base(runSpider) { }

    public void SetPlayerTransform(Transform player)
    {
        _player = player;
    }

    public override void Enter()
    {

        if (_player == null)
        {
            Debug.LogError("Player transform is not set in RunSpiderDashState.");
            return;
        }

        _runSpider.currentStateType = RunSpiderStateMachine.SpiderState.DashAttack;
        Debug.Log("<<<<<------------------Dash------------------->>>>>");


        _runSpider.StartCoroutine(_runSpider.DashCoroutine());
        //_runSpider.RunSpiderIsDash = true;
        //_runSpider.RunSpiderDashTimer = _runSpider.RunSpiderDashDuration;
        //_runSpider.RunSpiderDashCDTimer = _runSpider.RunSpiderDashCD;
        //_runSpider.RunSpiderAllowDash = false; // Không cho phép dash cho đến khi hồi chiêu

        //// Xác định hướng Dash dựa trên vị trí của người chơi
        //_runSpider.RunSpiderDirection = _player.position.x < _runSpider.transform.position.x ? -1 : 1;
        //_runSpider.rb.gravityScale = 0; // Tạm thời vô hiệu hóa trọng lực
        //_runSpider.rb.velocity = new Vector2(_runSpider.RunSpiderDashStrength * _runSpider.RunSpiderDirection, _runSpider.rb.velocity.y); // Cập nhật vận tốc
    }

    public override void Update()
    {
        
    }

    public override void Exit()
    {
        // Logic khi thoát khỏi trạng thái này, nếu cần
        //_runSpider.RunSpiderIsDash = false;
        //_runSpider.rb.gravityScale = 1;
    }

    //public IEnumerator DashCoroutine()
    //{
    //    // Thiết lập thời gian và trạng thái dash
    //    _runSpider.RunSpiderIsDash = true;
    //    _runSpider.RunSpiderDashTimer = _runSpider.RunSpiderDashDuration;
    //    _runSpider.RunSpiderDashCDTimer = _runSpider.RunSpiderDashCD;

    //    // Vô hiệu hóa trọng lực và thiết lập vận tốc ban đầu
    //    _runSpider.rb.gravityScale = 0;
    //    _runSpider.rb.velocity = new Vector2(_runSpider.RunSpiderDashStrength * _runSpider.RunSpiderDirection, _runSpider.rb.velocity.y);
    //    Debug.Log("Dash started with velocity: " + _runSpider.rb.velocity);
    //    // Chờ trong khoảng thời gian của dash
    //    yield return new WaitForSeconds(_runSpider.RunSpiderDashDuration);

    //    // Kết thúc Dash
    //    _runSpider.RunSpiderIsDash = false;
    //    _runSpider.rb.velocity = new Vector2(0, _runSpider.rb.velocity.y);
    //    _runSpider.rb.gravityScale = 1;
    //    _runSpider.SetState(new RunSpiderChaseState(_runSpider));

    //    // Bắt đầu thời gian hồi chiêu Dash
    //    yield return new WaitForSeconds(_runSpider.RunSpiderDashCD);

    //    // Cho phép Dash trở lại sau khi hồi chiêu
    //    _runSpider.RunSpiderAllowDash = true;
    //}
}
