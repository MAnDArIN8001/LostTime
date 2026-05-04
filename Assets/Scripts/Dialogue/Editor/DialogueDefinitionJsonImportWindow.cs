#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Dialogue.Authoring;
using Dialogue.Core;
using UnityEditor;
using UnityEngine;

namespace Dialogue.Editor
{
    public sealed class DialogueDefinitionJsonImportWindow : EditorWindow
    {
        private const string SampleJson = "{\n  \"dialogueId\": \"mentor_intro\",\n  \"displayName\": \"Mentor Intro\",\n  \"startNodeId\": \"n1\",\n  \"nodes\": [\n    {\n      \"nodeId\": \"n1\",\n      \"speakerId\": \"mentor\",\n      \"speakerName\": \"Mentor\",\n      \"text\": \"Welcome, traveler.\",\n      \"nodeType\": 0,\n      \"entryConditions\": [],\n      \"options\": [\n        {\n          \"optionId\": \"continue_1\",\n          \"text\": \"Continue\",\n          \"nextNodeId\": \"n2\",\n          \"conditions\": []\n        }\n      ]\n    },\n    {\n      \"nodeId\": \"n2\",\n      \"speakerId\": \"mentor\",\n      \"speakerName\": \"Mentor\",\n      \"text\": \"Choose your path.\",\n      \"nodeType\": 1,\n      \"entryConditions\": [],\n      \"options\": [\n        {\n          \"optionId\": \"opt_a\",\n          \"text\": \"I am ready\",\n          \"nextNodeId\": \"n3\",\n          \"conditions\": []\n        },\n        {\n          \"optionId\": \"opt_b\",\n          \"text\": \"Not yet\",\n          \"nextNodeId\": \"n4\",\n          \"conditions\": []\n        }\n      ]\n    }\n  ]\n}";

        private DialogueDefinition _targetDefinition;
        private string _jsonText = SampleJson;
        private Vector2 _scroll;

        [MenuItem("Tools/LostTime/Dialogue/Import JSON To Definition")]
        private static void Open()
        {
            var window = GetWindow<DialogueDefinitionJsonImportWindow>("Dialogue JSON Import");
            window.minSize = new Vector2(680f, 520f);
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Dialogue JSON Import", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            _targetDefinition = (DialogueDefinition)EditorGUILayout.ObjectField(
                "Target Definition",
                _targetDefinition,
                typeof(DialogueDefinition),
                false);

            EditorGUILayout.HelpBox(
                "JSON shape must match the dialogue schema. Enums are numeric: nodeType (0 Continue, 1 Choice, 2 End), conditionType (0 AlwaysTrue, 1 PreviousChoiceIs, 2 QuestCompleted).",
                MessageType.Info);

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            _jsonText = EditorGUILayout.TextArea(_jsonText, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Paste From Clipboard"))
                {
                    _jsonText = EditorGUIUtility.systemCopyBuffer;
                    Repaint();
                }

                if (GUILayout.Button("Copy Sample"))
                {
                    EditorGUIUtility.systemCopyBuffer = SampleJson;
                }
            }

            EditorGUILayout.Space();
            using (new EditorGUI.DisabledScope(_targetDefinition == null || string.IsNullOrWhiteSpace(_jsonText)))
            {
                if (GUILayout.Button("Apply JSON To Asset", GUILayout.Height(32f)))
                {
                    ApplyJson();
                }
            }
        }

        private void ApplyJson()
        {
            if (_targetDefinition == null)
            {
                EditorUtility.DisplayDialog("Dialogue JSON Import", "Assign target DialogueDefinition asset.", "OK");
                return;
            }

            DialogueImportModel model;
            try
            {
                model = JsonUtility.FromJson<DialogueImportModel>(_jsonText);
            }
            catch (Exception exception)
            {
                EditorUtility.DisplayDialog("Dialogue JSON Import", $"JSON parse failed: {exception.Message}", "OK");
                return;
            }

            if (model == null)
            {
                EditorUtility.DisplayDialog("Dialogue JSON Import", "JSON parse returned null model.", "OK");
                return;
            }

            var serializedObject = new SerializedObject(_targetDefinition);
            serializedObject.Update();

            serializedObject.FindProperty("<DialogueId>k__BackingField").stringValue = model.dialogueId ?? string.Empty;
            serializedObject.FindProperty("<DisplayName>k__BackingField").stringValue = model.displayName ?? string.Empty;
            serializedObject.FindProperty("<StartNodeId>k__BackingField").stringValue = model.startNodeId ?? string.Empty;

            var nodesProperty = serializedObject.FindProperty("_nodes");
            var sourceNodes = model.nodes ?? new List<DialogueImportNode>();
            nodesProperty.arraySize = sourceNodes.Count;

            for (var nodeIndex = 0; nodeIndex < sourceNodes.Count; nodeIndex++)
            {
                var sourceNode = sourceNodes[nodeIndex] ?? new DialogueImportNode();
                var nodeProperty = nodesProperty.GetArrayElementAtIndex(nodeIndex);

                nodeProperty.FindPropertyRelative("<NodeId>k__BackingField").stringValue = sourceNode.nodeId ?? string.Empty;
                nodeProperty.FindPropertyRelative("<SpeakerId>k__BackingField").stringValue = sourceNode.speakerId ?? string.Empty;
                nodeProperty.FindPropertyRelative("<SpeakerName>k__BackingField").stringValue = sourceNode.speakerName ?? string.Empty;
                nodeProperty.FindPropertyRelative("<Text>k__BackingField").stringValue = sourceNode.text ?? string.Empty;
                nodeProperty.FindPropertyRelative("<NodeType>k__BackingField").enumValueIndex = ClampEnumIndex(sourceNode.nodeType, 0, 2);

                FillConditions(nodeProperty.FindPropertyRelative("_entryConditions"), sourceNode.entryConditions);
                FillOptions(nodeProperty.FindPropertyRelative("_options"), sourceNode.options);
            }

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(_targetDefinition);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            var data = _targetDefinition.ToData();
            var issues = DialogueDefinitionValidation.Validate(data);
            var errorCount = 0;
            var warningCount = 0;
            for (var i = 0; i < issues.Count; i++)
            {
                if (issues[i].Severity == DialogueValidationSeverity.Error)
                {
                    errorCount++;
                }
                else
                {
                    warningCount++;
                }
            }

            EditorUtility.DisplayDialog(
                "Dialogue JSON Import",
                $"Import complete.\nErrors: {errorCount}\nWarnings: {warningCount}\nCheck Console for details.",
                "OK");
        }

        private static void FillOptions(SerializedProperty optionsProperty, List<DialogueImportOption> sourceOptions)
        {
            var options = sourceOptions ?? new List<DialogueImportOption>();
            optionsProperty.arraySize = options.Count;
            for (var optionIndex = 0; optionIndex < options.Count; optionIndex++)
            {
                var sourceOption = options[optionIndex] ?? new DialogueImportOption();
                var optionProperty = optionsProperty.GetArrayElementAtIndex(optionIndex);

                optionProperty.FindPropertyRelative("<OptionId>k__BackingField").stringValue = sourceOption.optionId ?? string.Empty;
                optionProperty.FindPropertyRelative("<Text>k__BackingField").stringValue = sourceOption.text ?? string.Empty;
                optionProperty.FindPropertyRelative("<NextNodeId>k__BackingField").stringValue = sourceOption.nextNodeId ?? string.Empty;
                FillConditions(optionProperty.FindPropertyRelative("_conditions"), sourceOption.conditions);
            }
        }

        private static void FillConditions(SerializedProperty conditionsProperty, List<DialogueImportCondition> sourceConditions)
        {
            var conditions = sourceConditions ?? new List<DialogueImportCondition>();
            conditionsProperty.arraySize = conditions.Count;
            for (var conditionIndex = 0; conditionIndex < conditions.Count; conditionIndex++)
            {
                var sourceCondition = conditions[conditionIndex] ?? new DialogueImportCondition();
                var conditionProperty = conditionsProperty.GetArrayElementAtIndex(conditionIndex);
                conditionProperty.FindPropertyRelative("<ConditionType>k__BackingField").enumValueIndex = ClampEnumIndex(sourceCondition.conditionType, 0, 2);
                conditionProperty.FindPropertyRelative("<Key>k__BackingField").stringValue = sourceCondition.key ?? string.Empty;
                conditionProperty.FindPropertyRelative("<Value>k__BackingField").stringValue = sourceCondition.value ?? string.Empty;
            }
        }

        private static int ClampEnumIndex(int rawValue, int minValue, int maxValue)
        {
            return Mathf.Clamp(rawValue, minValue, maxValue);
        }

        [Serializable]
        private sealed class DialogueImportModel
        {
            public string dialogueId = string.Empty;
            public string displayName = string.Empty;
            public string startNodeId = string.Empty;
            public List<DialogueImportNode> nodes = new();
        }

        [Serializable]
        private sealed class DialogueImportNode
        {
            public string nodeId = string.Empty;
            public string speakerId = string.Empty;
            public string speakerName = string.Empty;
            public string text = string.Empty;
            public int nodeType = (int)DialogueNodeType.Continue;
            public List<DialogueImportCondition> entryConditions = new();
            public List<DialogueImportOption> options = new();
        }

        [Serializable]
        private sealed class DialogueImportOption
        {
            public string optionId = string.Empty;
            public string text = string.Empty;
            public string nextNodeId = string.Empty;
            public List<DialogueImportCondition> conditions = new();
        }

        [Serializable]
        private sealed class DialogueImportCondition
        {
            public int conditionType = (int)DialogueConditionType.AlwaysTrue;
            public string key = string.Empty;
            public string value = string.Empty;
        }
    }
}
#endif
