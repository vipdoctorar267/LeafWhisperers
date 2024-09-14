

// Lớp cơ sở cho các trạng thái
public abstract class RunSpiderState
{
    protected RunSpiderStateMachine _runSpider;

    public RunSpiderState(RunSpiderStateMachine runSpider)
    {
        _runSpider = runSpider;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}
