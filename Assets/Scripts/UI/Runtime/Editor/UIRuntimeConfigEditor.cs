#if UNITY_EDITOR
using System;
using UI.Runtime;
using UnityEditor;
using UnityEngine;

namespace UI.Runtime.Editor
{
    [CustomEditor(typeof(UIRuntimeConfig))]
    public sealed class UIRuntimeConfigEditor : UnityEditor.Editor
    {
        private string _lastValidationMessage = string.Empty;
        private MessageType _lastMessageType = MessageType.Info;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            if (GUILayout.Button("Validate Config"))
            {
                ValidateConfig((UIRuntimeConfig)target);
            }

            if (!string.IsNullOrWhiteSpace(_lastValidationMessage))
            {
                EditorGUILayout.HelpBox(_lastValidationMessage, _lastMessageType);
            }
        }

        private void ValidateConfig(UIRuntimeConfig config)
        {
            if (config == null)
            {
                _lastValidationMessage = "Config is null.";
                _lastMessageType = MessageType.Error;
                return;
            }

            var definitions = config.PanelDefinitions;
            if (definitions == null || definitions.Count == 0)
            {
                _lastValidationMessage = "No panel definitions assigned.";
                _lastMessageType = MessageType.Warning;
                return;
            }

            var errors = 0;
            for (var i = 0; i < definitions.Count; i++)
            {
                var definition = definitions[i];
                if (definition == null)
                {
                    Debug.LogError($"[UIRuntimeConfigEditor] Null panel definition at index {i}.", config);
                    errors++;
                    continue;
                }

                if (string.IsNullOrWhiteSpace(definition.PanelTypeName))
                {
                    Debug.LogError($"[UIRuntimeConfigEditor] Empty PanelTypeName in definition '{definition.name}'.", definition);
                    errors++;
                }
                else
                {
                    var panelType = ResolveType(definition.PanelTypeName);
                    if (panelType == null)
                    {
                        Debug.LogError($"[UIRuntimeConfigEditor] Unknown panel type '{definition.PanelTypeName}' in '{definition.name}'.", definition);
                        errors++;
                    }
                    else if (!typeof(IUIPanel).IsAssignableFrom(panelType))
                    {
                        Debug.LogError($"[UIRuntimeConfigEditor] Type '{definition.PanelTypeName}' does not implement IUIPanel.", definition);
                        errors++;
                    }
                }

                if (definition.PanelPrefab == null && string.IsNullOrWhiteSpace(definition.AssetPathOrKey))
                {
                    Debug.LogError(
                        $"[UIRuntimeConfigEditor] Definition '{definition.name}' requires either PanelPrefab or AssetPathOrKey.",
                        definition);
                    errors++;
                }
            }

            if (errors == 0)
            {
                _lastValidationMessage = $"Validation passed. Definitions: {definitions.Count}.";
                _lastMessageType = MessageType.Info;
            }
            else
            {
                _lastValidationMessage = $"Validation failed. Errors: {errors}. Check Console for details.";
                _lastMessageType = MessageType.Error;
            }
        }

        private static Type ResolveType(string typeName)
        {
            var type = Type.GetType(typeName, false);
            if (type != null)
            {
                return type;
            }

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (var i = 0; i < assemblies.Length; i++)
            {
                type = assemblies[i].GetType(typeName, false);
                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }
    }
}
#endif
