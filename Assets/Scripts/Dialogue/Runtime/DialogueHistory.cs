using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue.Runtime
{
    [Serializable]
    public sealed class DialogueHistory
    {
        public List<DialogueHistoryEntry> Entries = new();
        public HashSet<string> CompletedDialogues = new(StringComparer.Ordinal);
    }

    [Serializable]
    public readonly struct DialogueHistoryEntry
    {
        public readonly string DialogueId;
        public readonly string NodeId;
        public readonly string OptionId;

        public DialogueHistoryEntry(string dialogueId, string nodeId, string optionId)
        {
            DialogueId = dialogueId ?? string.Empty;
            NodeId = nodeId ?? string.Empty;
            OptionId = optionId ?? string.Empty;
        }
    }

    public interface IDialogueHistoryRepository
    {
        DialogueHistory Load();
        void Save(DialogueHistory history);
    }

    public sealed class InMemoryDialogueHistoryRepository : IDialogueHistoryRepository
    {
        private readonly DialogueHistory _history = new();

        public DialogueHistory Load()
        {
            return _history;
        }

        public void Save(DialogueHistory history)
        {
            if (history == null)
            {
                return;
            }

            _history.Entries = history.Entries ?? new List<DialogueHistoryEntry>();
            _history.CompletedDialogues = history.CompletedDialogues ?? new HashSet<string>(StringComparer.Ordinal);
        }
    }

    [Serializable]
    internal sealed class DialogueHistoryContainer
    {
        public List<DialogueHistoryEntryDto> Entries = new();
        public List<string> CompletedDialogues = new();
    }

    [Serializable]
    internal sealed class DialogueHistoryEntryDto
    {
        public string DialogueId;
        public string NodeId;
        public string OptionId;
    }

    public sealed class PlayerPrefsDialogueHistoryRepository : IDialogueHistoryRepository
    {
        private readonly string _key;

        public PlayerPrefsDialogueHistoryRepository(string key = "dialogue.history.v1")
        {
            _key = string.IsNullOrWhiteSpace(key) ? "dialogue.history.v1" : key;
        }

        public DialogueHistory Load()
        {
            var history = new DialogueHistory();
            var raw = PlayerPrefs.GetString(_key, string.Empty);
            if (string.IsNullOrWhiteSpace(raw))
            {
                return history;
            }

            var container = JsonUtility.FromJson<DialogueHistoryContainer>(raw);
            if (container == null)
            {
                return history;
            }

            if (container.Entries != null)
            {
                for (var i = 0; i < container.Entries.Count; i++)
                {
                    var entry = container.Entries[i];
                    if (entry == null)
                    {
                        continue;
                    }

                    history.Entries.Add(new DialogueHistoryEntry(entry.DialogueId, entry.NodeId, entry.OptionId));
                }
            }

            if (container.CompletedDialogues != null)
            {
                for (var i = 0; i < container.CompletedDialogues.Count; i++)
                {
                    var dialogueId = container.CompletedDialogues[i];
                    if (!string.IsNullOrWhiteSpace(dialogueId))
                    {
                        history.CompletedDialogues.Add(dialogueId);
                    }
                }
            }

            return history;
        }

        public void Save(DialogueHistory history)
        {
            if (history == null)
            {
                return;
            }

            var container = new DialogueHistoryContainer
            {
                Entries = new List<DialogueHistoryEntryDto>(history.Entries.Count),
                CompletedDialogues = new List<string>(history.CompletedDialogues.Count)
            };

            for (var i = 0; i < history.Entries.Count; i++)
            {
                var entry = history.Entries[i];
                container.Entries.Add(new DialogueHistoryEntryDto
                {
                    DialogueId = entry.DialogueId,
                    NodeId = entry.NodeId,
                    OptionId = entry.OptionId
                });
            }

            foreach (var completedDialogueId in history.CompletedDialogues)
            {
                container.CompletedDialogues.Add(completedDialogueId);
            }

            var raw = JsonUtility.ToJson(container);
            PlayerPrefs.SetString(_key, raw);
            PlayerPrefs.Save();
        }
    }
}
