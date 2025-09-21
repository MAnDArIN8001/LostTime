using Character.Modules.Animation;
using Character.Modules.Movement;
using Character.Modules.Rotation;
using FSM;
using UnityEngine;

namespace Character.States
{
    public class CharacterRunState : CharacterMovementState
    {
        public CharacterRunState(StateType stateType, float movementSpeed, MainInput mainInput, 
            MovementModule movementModule, AnimationModule animationModule, RotationModule rotationModule, Transform camera) 
            : base(stateType, movementSpeed, mainInput, movementModule, animationModule, rotationModule, camera)
        {
            
        }

        public override void Enter()
        {
            base.Enter();

            _animationModule.SetRunning(true);
        }

        public override void Exit()
        {
            base.Exit();
            
            _animationModule.SetRunning(false);
        }
    }
}