namespace FSM
{
    public enum StateType
    {
        Idle,
        Walk,
        Run,
        Jump,
        Aim, 
        Attack,
        Looting,

        #region Scope

        Movement,
        Attacking,
        Communication

        #endregion
    }
}