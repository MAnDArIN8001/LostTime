using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeGeneration
{
    [Serializable]
    public struct CodeGenerationTargetProfile
    {
        [SerializeField] private string _sourceTypeName;
        [SerializeField] private string _className;
        [SerializeField] private string _outputFolderPathOverride;
        [SerializeField] private string _namespaceOverride;

        public string SourceTypeName => _sourceTypeName;
        public string ClassName => _className;
        public string OutputFolderPathOverride => _outputFolderPathOverride;
        public string NamespaceOverride => _namespaceOverride;
    }

    public readonly struct CodeGenerationTarget
    {
        public CodeGenerationTarget(string className, string outputFolderPath, string namespaceName)
        {
            ClassName = className;
            OutputFolderPath = outputFolderPath;
            NamespaceName = namespaceName;
        }

        public string ClassName { get; }
        public string OutputFolderPath { get; }
        public string NamespaceName { get; }
    }

    [CreateAssetMenu(
        fileName = "CodeGenerationSettings",
        menuName = "Tools/Code Generation/Settings",
        order = 0)]
    public sealed class CodeGenerationSettings : ScriptableObject
    {
        [Header("Defaults")]
        [SerializeField] private string _outputFolderPath = "Assets/Generated";
        [SerializeField] private string _namespaceName = "Loot.Data";

        [Header("Per Source Type Profiles")]
        [SerializeField] private List<CodeGenerationTargetProfile> _targets = new();

        public CodeGenerationTarget ResolveForSourceTypeName(string sourceTypeName, string fallbackClassName)
        {
            CodeGenerationTargetProfile? matchedTarget = null;

            foreach (var target in _targets)
            {
                if (string.IsNullOrWhiteSpace(target.SourceTypeName) ||
                    !string.Equals(target.SourceTypeName.Trim(), sourceTypeName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                matchedTarget = target;
                break;
            }

            var className = fallbackClassName;
            var outputFolderPath = _outputFolderPath;
            var namespaceName = _namespaceName;

            if (matchedTarget.HasValue)
            {
                var target = matchedTarget.Value;

                if (!string.IsNullOrWhiteSpace(target.ClassName))
                {
                    className = target.ClassName.Trim();
                }

                if (!string.IsNullOrWhiteSpace(target.OutputFolderPathOverride))
                {
                    outputFolderPath = target.OutputFolderPathOverride.Trim();
                }

                if (!string.IsNullOrWhiteSpace(target.NamespaceOverride))
                {
                    namespaceName = target.NamespaceOverride.Trim();
                }
            }

            return new CodeGenerationTarget(className, outputFolderPath, namespaceName);
        }
    }

#if UNITY_EDITOR
    public static class CodeGenerationSettingsProvider
    {
        private static CodeGenerationSettings _cachedSettings;

        public static CodeGenerationSettings GetOrDefault()
        {
            if (_cachedSettings != null)
            {
                return _cachedSettings;
            }

            var guids = UnityEditor.AssetDatabase.FindAssets("t:CodeGenerationSettings");
            if (guids.Length == 0)
            {
                Debug.LogWarning("CodeGenerationSettings asset not found. Using in-memory defaults.");
                _cachedSettings = ScriptableObject.CreateInstance<CodeGenerationSettings>();
                return _cachedSettings;
            }

            if (guids.Length > 1)
            {
                Debug.LogWarning("Multiple CodeGenerationSettings assets found. Using the first one.");
            }

            var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
            _cachedSettings = UnityEditor.AssetDatabase.LoadAssetAtPath<CodeGenerationSettings>(path);

            if (_cachedSettings == null)
            {
                Debug.LogWarning("Failed to load CodeGenerationSettings asset. Using in-memory defaults.");
                _cachedSettings = ScriptableObject.CreateInstance<CodeGenerationSettings>();
            }

            return _cachedSettings;
        }
    }
#endif
}
