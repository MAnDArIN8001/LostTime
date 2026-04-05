using System;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using Loot.Systems;
using UnityEngine;
using UnityEngine.Serialization;

namespace Quest
{
    public class VerticalSliceQuestProgression : MonoBehaviour
    {
        [Serializable]
        private sealed class SealObjective
        {
            [SerializeField] private string _name = "Seal";
            [SerializeField] private InteractionTarget _sealTarget;
            [SerializeField] private MonoBehaviour _encounter;
            [SerializeField] private GameObject _encounterRoot;

            public SealObjective()
            {
            }

            public SealObjective(string name, InteractionTarget sealTarget, MonoBehaviour encounter, GameObject encounterRoot)
            {
                _name = name;
                _sealTarget = sealTarget;
                _encounter = encounter;
                _encounterRoot = encounterRoot;
            }

            public string Name => _name;
            public InteractionTarget SealTarget => _sealTarget;
            public MonoBehaviour EncounterBehaviour => _encounter;
            public IEncounterEnemy Encounter => _encounter as IEncounterEnemy;
            public GameObject EncounterRoot => _encounterRoot;
        }

        public enum QuestStep
        {
            TalkToMentor = 0,
            RestoreSeals = 1,
            UnlockArena = 2,
            DefeatGuardian = 3,
            ReturnToMentor = 4,
            Completed = 5,
        }

        [Header("Quest Targets")]
        [SerializeField] private InteractionTarget _mentorTarget;
        [FormerlySerializedAs("_sealTarget")]
        [SerializeField] private SealObjective[] _seals = Array.Empty<SealObjective>();
        
        [Header("Legacy Quest Target (Auto-Migrated)")]
        [SerializeField] private InteractionTarget _sealTarget;
        [SerializeField] private MonoBehaviour _beastEncounter;
        [SerializeField] private GameObject _beastRoot;

        [Header("Arena Unlock")]
        [SerializeField] private GameObject[] _arenaUnlockTargets = Array.Empty<GameObject>();
        [SerializeField, Min(0f)] private float _arenaUnlockDelay = 1.5f;
        [Header("Guardian")]
        [SerializeField] private MonoBehaviour _guardianEncounter;

        private QuestStep _currentStep;
        private bool _isArenaUnlocked;
        private bool _bossCleared;
        private int _restoredSealCount;
        private Coroutine _unlockArenaRoutine;
        private readonly HashSet<int> _restoredSealIndices = new();
        private SealObjective[] _runtimeSeals = Array.Empty<SealObjective>();

        public string CurrentObjectiveText => GetObjectiveText(_currentStep);
        public QuestStep CurrentStep => _currentStep;

        public event Action<string> ObjectiveChanged;
        public event Action Completed;
        public event Action<int, int> SealRestored;
        public event Action<int, int> EncounterCleared;
        public event Action ArenaUnlocked;
        public event Action<QuestStep> QuestStepAdvanced;
        public event Action QuestCompleted;

        private void OnEnable()
        {
            _runtimeSeals = ResolveSealObjectives();

            for (var i = 0; i < _runtimeSeals.Length; i++)
            {
                var seal = _runtimeSeals[i];
                if (seal == null)
                {
                    continue;
                }

                if (seal.EncounterRoot == null && seal.EncounterBehaviour != null)
                {
                    seal.EncounterBehaviour.gameObject.SetActive(false);
                }
                else if (seal.EncounterRoot != null)
                {
                    seal.EncounterRoot.SetActive(false);
                }

                if (seal.SealTarget != null)
                {
                    seal.SealTarget.Interacted += OnSealInteracted;
                }

                if (seal.Encounter != null)
                {
                    seal.Encounter.Died += OnEncounterDied;
                }
            }

            foreach (var unlockTarget in _arenaUnlockTargets)
            {
                if (unlockTarget != null)
                {
                    unlockTarget.SetActive(false);
                }
            }

            if (_mentorTarget != null)
            {
                _mentorTarget.Interacted += OnMentorInteracted;
            }

            var guardian = GuardianEncounter;
            if (guardian != null)
            {
                guardian.Died += OnGuardianDied;
            }

            _currentStep = QuestStep.TalkToMentor;
            _isArenaUnlocked = false;
            _bossCleared = false;
            _restoredSealCount = 0;
            _restoredSealIndices.Clear();
            NotifyObjectiveChanged();
        }

        private void OnDisable()
        {
            if (_mentorTarget != null)
            {
                _mentorTarget.Interacted -= OnMentorInteracted;
            }

            var guardian = GuardianEncounter;
            if (guardian != null)
            {
                guardian.Died -= OnGuardianDied;
            }

            for (var i = 0; i < _runtimeSeals.Length; i++)
            {
                var seal = _runtimeSeals[i];
                if (seal == null)
                {
                    continue;
                }

                if (seal.SealTarget != null)
                {
                    seal.SealTarget.Interacted -= OnSealInteracted;
                }

                if (seal.Encounter != null)
                {
                    seal.Encounter.Died -= OnEncounterDied;
                }
            }

            if (_unlockArenaRoutine != null)
            {
                StopCoroutine(_unlockArenaRoutine);
                _unlockArenaRoutine = null;
            }
        }

        private void OnMentorInteracted(IInteractable interactable, GameObject interactor)
        {
            if (_currentStep == QuestStep.TalkToMentor)
            {
                ActivateSealEncounters();
                SetStep(QuestStep.RestoreSeals);
                return;
            }

            if (_currentStep == QuestStep.ReturnToMentor)
            {
                SetStep(QuestStep.Completed);
                Completed?.Invoke();
                QuestCompleted?.Invoke();
            }
        }

        private void OnEncounterDied(IEncounterEnemy encounter)
        {
            if (_currentStep != QuestStep.RestoreSeals)
            {
                return;
            }

            if (!TryGetSealIndexByEncounter(encounter, out var sealIndex))
            {
                return;
            }

            EncounterCleared?.Invoke(sealIndex, GetConfiguredSealCount());
        }

        private IEncounterEnemy GuardianEncounter => _guardianEncounter as IEncounterEnemy;

        private void OnGuardianDied(IEncounterEnemy encounter)
        {
            if (_currentStep != QuestStep.DefeatGuardian || _bossCleared)
            {
                return;
            }

            if (_guardianEncounter == null || !ReferenceEquals(encounter, GuardianEncounter))
            {
                return;
            }

            _bossCleared = true;
            SetStep(QuestStep.ReturnToMentor);
        }

        private void OnSealInteracted(IInteractable interactable, GameObject interactor)
        {
            if (_currentStep != QuestStep.RestoreSeals)
            {
                return;
            }

            if (!TryGetSealIndexByTarget(interactable, out var sealIndex))
            {
                return;
            }

            if (!IsSealReadyForRestore(sealIndex))
            {
                return;
            }

            if (!_restoredSealIndices.Add(sealIndex))
            {
                return;
            }

            var requiredSealCount = GetConfiguredSealCount();
            _restoredSealCount = _restoredSealIndices.Count;
            SealRestored?.Invoke(sealIndex, requiredSealCount);
            NotifyObjectiveChanged();

            if (requiredSealCount > 0
                && _restoredSealCount >= requiredSealCount
                && !_isArenaUnlocked
                && _unlockArenaRoutine == null)
            {
                SetStep(QuestStep.UnlockArena);
                _unlockArenaRoutine = StartCoroutine(UnlockArenaAfterDelay());
            }
        }

        private void NotifyObjectiveChanged()
        {
            ObjectiveChanged?.Invoke(CurrentObjectiveText);
        }

        private void SetStep(QuestStep nextStep)
        {
            _currentStep = nextStep;
            if (_currentStep == QuestStep.ReturnToMentor)
            {
                EnsureMentorCanCompleteQuest();
            }

            NotifyObjectiveChanged();
            QuestStepAdvanced?.Invoke(_currentStep);

            if (_currentStep == QuestStep.DefeatGuardian)
            {
                TryAdvanceIfGuardianAlreadyDead();
            }
        }

        private void ActivateSealEncounters()
        {
            for (var i = 0; i < _runtimeSeals.Length; i++)
            {
                var seal = _runtimeSeals[i];
                if (seal == null)
                {
                    continue;
                }

                if (seal.EncounterRoot != null)
                {
                    seal.EncounterRoot.SetActive(true);
                    continue;
                }

                if (seal.EncounterBehaviour != null)
                {
                    seal.EncounterBehaviour.gameObject.SetActive(true);
                }
            }
        }

        private bool TryGetSealIndexByTarget(IInteractable interactable, out int index)
        {
            for (var i = 0; i < _runtimeSeals.Length; i++)
            {
                var seal = _runtimeSeals[i];
                if (seal != null && ReferenceEquals(seal.SealTarget, interactable))
                {
                    index = i;
                    return true;
                }
            }

            index = -1;
            return false;
        }

        private bool TryGetSealIndexByEncounter(IEncounterEnemy encounter, out int index)
        {
            for (var i = 0; i < _runtimeSeals.Length; i++)
            {
                var seal = _runtimeSeals[i];
                if (seal != null && ReferenceEquals(seal.Encounter, encounter))
                {
                    index = i;
                    return true;
                }
            }

            index = -1;
            return false;
        }

        private bool IsSealReadyForRestore(int sealIndex)
        {
            if (sealIndex < 0 || sealIndex >= _runtimeSeals.Length)
            {
                return false;
            }

            var seal = _runtimeSeals[sealIndex];
            if (seal == null || seal.Encounter == null)
            {
                return true;
            }

            return seal.Encounter.IsDead;
        }

        private int GetConfiguredSealCount()
        {
            var configured = 0;
            for (var i = 0; i < _runtimeSeals.Length; i++)
            {
                if (_runtimeSeals[i] != null)
                {
                    configured++;
                }
            }

            return configured;
        }

        private void EnsureMentorCanCompleteQuest()
        {
            if (_mentorTarget != null && !_mentorTarget.CanInteract)
            {
                _mentorTarget.ResetInteractionState();
            }
        }

        private IEnumerator UnlockArenaAfterDelay()
        {
            if (_arenaUnlockDelay > 0f)
            {
                yield return new WaitForSeconds(_arenaUnlockDelay);
            }

            for (var i = 0; i < _arenaUnlockTargets.Length; i++)
            {
                if (_arenaUnlockTargets[i] != null)
                {
                    _arenaUnlockTargets[i].SetActive(true);
                }
            }

            _isArenaUnlocked = true;
            ArenaUnlocked?.Invoke();
            _unlockArenaRoutine = null;
            SetStep(QuestStep.DefeatGuardian);
        }

        private void TryAdvanceIfGuardianAlreadyDead()
        {
            var encounter = GuardianEncounter;
            if (encounter == null || !encounter.IsDead || _bossCleared)
            {
                return;
            }

            _bossCleared = true;
            SetStep(QuestStep.ReturnToMentor);
        }

        private SealObjective[] ResolveSealObjectives()
        {
            var configuredSeals = new List<SealObjective>();
            for (var i = 0; i < _seals.Length; i++)
            {
                if (_seals[i] != null)
                {
                    configuredSeals.Add(_seals[i]);
                }
            }

            if (configuredSeals.Count > 0)
            {
                return configuredSeals.ToArray();
            }

            if (_sealTarget == null && _beastEncounter == null && _beastRoot == null)
            {
                return Array.Empty<SealObjective>();
            }

            return new[]
            {
                new SealObjective("Legacy Seal", _sealTarget, _beastEncounter, _beastRoot),
            };
        }

        private string GetObjectiveText(QuestStep step)
        {
            return step switch
            {
                QuestStep.TalkToMentor => "Talk to the mentor to begin the trial",
                QuestStep.RestoreSeals => $"Restore all anomaly seals ({_restoredSealCount}/{GetConfiguredSealCount()})",
                QuestStep.UnlockArena => "The arena is unlocking...",
                QuestStep.DefeatGuardian => "Defeat the trial guardian in the arena",
                QuestStep.ReturnToMentor => "Return to the mentor to finish the trial",
                QuestStep.Completed => "Trial complete",
                _ => "Trial complete",
            };
        }
    }
}
