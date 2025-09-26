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
        [SerializeField] private string _generatedClassName;
        
        [Space, SerializeField] private List<AnimationParamSetup> _animationParamsList;

        private readonly Dictionary<string, AnimationParamSetup> _animationParamsDictionary = new();

        public IReadOnlyDictionary<string, AnimationParamSetup> AnimationParamSetups => _animationParamsDictionary;
        
#if UNITY_EDITOR
        
        private void OnValidate()
        {
            var constPairs = new List<ConstPair>();
            
            _animationParamsDictionary.Clear();
            
            foreach (var animationParam in _animationParamsList)
            {
                _animationParamsDictionary.Add(animationParam.Id, animationParam);
                
                constPairs.Add(new ConstPair() { Id = animationParam.Id, Name = animationParam.Name});
            }
        }

        [ContextMenu("Generate Key Class")]
        private void GenerateKeyClass()
        {
            var constPairs = _animationParamsList.Select(animationParam => new ConstPair() { Id = animationParam.Id, Name = animationParam.Name }).ToList();
            
            ConstKeysGenerator.GenerateItemKeysClass(_generatedClassName, constPairs);
        }

#endif
    }
}