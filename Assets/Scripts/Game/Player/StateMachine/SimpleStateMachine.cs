namespace Game.Player.StateMachine
{
    public enum CharacterState
    {
        Idle,
        Run,
        Attack,
        Hit,
        Death
    }
    
    public class SimpleStateMachine
    {
        private CharacterState _currentState;

        public void ChangeState(CharacterState newState)
        {
            if (_currentState == newState)
                return;

            ExitState();
            _currentState = newState;
            EnterState();
        }

        private void EnterState()
        {
            switch (_currentState)
            {
                case CharacterState.Idle:
                    break;
                case CharacterState.Run:
                    break;
                case CharacterState.Attack:
                    break;
                case CharacterState.Hit:
                    break;
                case CharacterState.Death:
                    break;
            }
        }

        private void ExitState()
        {
            switch (_currentState)
            {
                case CharacterState.Idle:
                    break;
                case CharacterState.Run:
                    break;
                case CharacterState.Attack:
                    break;
                case CharacterState.Hit:
                    break;
                case CharacterState.Death:
                    break;
            }
        }
    }
}