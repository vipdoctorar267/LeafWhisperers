// Lớp cơ sở cho các trạng thái
public abstract class CharacterState
{
    protected CharacterStateMachine _character;

    public CharacterState(CharacterStateMachine character)
    {
        _character = character;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}