public abstract class StateMachine
{
    protected IState currentState;

    public void ChangeState(IState newState)
    {
        // ? 空条件运算符，如果currentState为null，则不会调用此处的方法
        currentState?.Exit();

        currentState = newState;
        
        currentState.Enter();
    }

    public void HandleInput()
    {
        currentState?.HandleInput();
    }

    public void Update()
    {
        currentState?.Update();
    }

    public void PhysicsUpdate()
    {
        currentState?.PhysicsUpdate();
    }
}