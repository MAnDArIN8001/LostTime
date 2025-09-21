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

        #region Scope

        Movement,
        Attacking,

        #endregion
    }
}