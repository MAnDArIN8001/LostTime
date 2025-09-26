using Character.Modules.Animation;
using Character.Modules.Animation.Facade;
using Character.Modules.Movement;
using Character.Modules.Rotation;
using FSM;
using Loot.Data;
using UnityEngine;

namespace Character.States
{
    public class CharacterRunState : CharacterMovementState
    {
        public CharacterRunState(StateType stateType, float movementSpeed, MainInput mainInput, 
            MovementModule movementModule, IAnimationFacade animationModule, RotationModule rotationModule, Transform camera) 
            : base(stateType, movementSpeed, mainInput, movementModule, animationModule, rotationModule, camera)
        {
            
        }

        public override void Enter()
        {
            base.Enter();

            _animationModule.Set(CharacterAnimationKeys.Runing, true);
        }

        public override void Exit()
        {
            base.Exit();
            
            _animationModule.Set(CharacterAnimationKeys.Runing, false);
        }
    }
}