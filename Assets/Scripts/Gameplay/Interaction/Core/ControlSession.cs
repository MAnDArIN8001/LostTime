using UnityEngine;

namespace Gameplay.Interaction.Core
{
    public sealed class ControlSession
    {
        private IControlable _target;
        private GameObject _interactor;
        private ControlMode _mode;
        private PointerTargetContext _pointerContext;
        private int _version;

        public bool IsActive => _target != null && _interactor != null && IsSingleMode(_mode);

        public ControlSessionSnapshot Snapshot => IsActive
            ? new ControlSessionSnapshot(_version, _target, _interactor, _mode, _pointerContext)
            : ControlSessionSnapshot.Inactive(_version);

        public bool CanBegin(
            IControlable target,
            ControlMode mode,
            GameObject interactor,
            in PointerTargetContext pointerContext)
        {
            if (IsActive || target == null || interactor == null || !IsSingleMode(mode))
            {
                return false;
            }

            if (!SupportsMode(target, mode))
            {
                return false;
            }

            return target.CanControl(mode, interactor, pointerContext);
        }

        public bool TryBegin(
            IControlable target,
            ControlMode mode,
            GameObject interactor,
            in PointerTargetContext pointerContext)
        {
            if (!CanBegin(target, mode, interactor, pointerContext))
            {
                InteractionDebugLog.Log(pointerContext.HitCollider, $"ControlSession begin rejected. target={target}, mode={mode}, interactor='{(interactor != null ? interactor.name : "null")}'.");
                return false;
            }

            _target = target;
            _interactor = interactor;
            _mode = mode;
            _pointerContext = pointerContext;
            _version++;

            InteractionDebugLog.Log(pointerContext.HitCollider, $"ControlSession begin accepted. mode={_mode}, interactor='{_interactor.name}'.");
            _target.BeginControl(_mode, _interactor, _pointerContext);
            return true;
        }

        public bool CanMaintain(in PointerTargetContext pointerContext)
        {
            return IsActive && _target.CanControl(_mode, _interactor, pointerContext);
        }

        public bool TryMaintain(in PointerTargetContext pointerContext)
        {
            if (!CanMaintain(pointerContext))
            {
                InteractionDebugLog.LogVerbose(pointerContext.HitCollider, "ControlSession maintain rejected.");
                return false;
            }

            _pointerContext = pointerContext;
            _target.UpdateControl(_mode, _interactor, _pointerContext);
            InteractionDebugLog.LogVerbose(pointerContext.HitCollider, $"ControlSession maintain executed. mode={_mode}.");
            return true;
        }

        public bool TryEnd()
        {
            return TryEnd(_pointerContext);
        }

        public bool TryEnd(in PointerTargetContext pointerContext)
        {
            if (!IsActive)
            {
                return false;
            }

            var target = _target;
            var interactor = _interactor;
            var mode = _mode;

            try
            {
                InteractionDebugLog.Log(pointerContext.HitCollider, $"ControlSession ending. mode={mode}, interactor='{(interactor != null ? interactor.name : "null")}'.");
                target.EndControl(mode, interactor, pointerContext);
            }
            finally
            {
                ClearInternal();
                _version++;
            }

            return true;
        }

        public bool Cancel()
        {
            if (!IsActive)
            {
                return false;
            }

            InteractionDebugLog.Log(null, $"ControlSession cancelled. mode={_mode}, interactor='{_interactor.name}'.");
            ClearInternal();
            _version++;
            return true;
        }

        private void ClearInternal()
        {
            _target = null;
            _interactor = null;
            _mode = ControlMode.None;
            _pointerContext = default;
        }

        private static bool SupportsMode(IControlable target, ControlMode mode)
        {
            return (target.SupportedModes & mode) == mode;
        }

        private static bool IsSingleMode(ControlMode mode)
        {
            return mode == ControlMode.Push || mode == ControlMode.Pull;
        }
    }
}
