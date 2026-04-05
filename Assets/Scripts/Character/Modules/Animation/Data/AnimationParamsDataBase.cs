using UnityEngine;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using CodeGeneration;
#endif

namespace Character.Modules.Animation.Data
{
    [CreateAssetMenu(fileName = "NewAnimationParamsDatabase", menuName = "Gameplay/Animation/Animation Param Data Base", order = 0)]
    public class AnimationParamsDataBase : ScriptableObject
    {
        [SerializeField] private List<AnimationParamSetup> _animationParamsList;

        private readonly Dictionary<string, AnimationParamSetup> _animationParamsDictionary = new();

        public IReadOnlyDictionary<string, AnimationParamSetup> AnimationParamSetups => _animationParamsDictionary;

        private void OnEnable()
        {
            RebuildDictionary();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            RebuildDictionary();
        }

        [ContextMenu("Generate Key Class")]
        private void GenerateKeyClass()
        {
            var settings = CodeGenerationSettingsProvider.GetOrDefault();
            var generationTarget = settings.ResolveForSourceTypeName(
                nameof(AnimationParamsDataBase),
                "CharacterAnimationKeys");
            var constPairs = (_animationParamsList ?? new List<AnimationParamSetup>())
                .Where(param => param != null)
                .Select(animationParam => new ConstPair { Id = animationParam.Id, Name = animationParam.Name })
                .ToList();

            ConstKeysGenerator.GenerateKeysClass(
                generationTarget.ClassName,
                constPairs,
                generationTarget.OutputFolderPath,
                generationTarget.NamespaceName);
        }
#endif

        private void RebuildDictionary()
        {
            _animationParamsDictionary.Clear();

            if (_animationParamsList == null)
            {
                return;
            }

            foreach (var animationParam in _animationParamsList)
            {
                if (animationParam == null || string.IsNullOrWhiteSpace(animationParam.Id))
                {
                    continue;
                }

                if (_animationParamsDictionary.ContainsKey(animationParam.Id))
                {
                    Debug.LogWarning($"Animation param id duplication detected and skipped: {animationParam.Id}", this);
                    continue;
                }

                _animationParamsDictionary.Add(animationParam.Id, animationParam);
            }
        }
    }
}