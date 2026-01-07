public interface IState
{
    public enum StateID { 
        SomeState, SomeOtherState, SomeThirdState
    }

    public enum Event { 
        Enter, Update, Exit
    }

    public abstract StateID Name { get; }
    protected abstract Event Stage { get; }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
    public abstract IState Handle();
}
