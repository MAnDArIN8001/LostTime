using System;
using UnityEngine;

namespace Quest.Core
{
    [Serializable]
    public sealed class QuestEventFilter
    {
        public string EventId = string.Empty;
        public string SourceId = string.Empty;
        public string TargetId = string.Empty;

        public QuestEventFilter Clone()
        {
            return new QuestEventFilter
            {
                EventId = EventId,
                SourceId = SourceId,
                TargetId = TargetId,
            };
        }

        public QuestExpectedSignalDto ToExpectedSignal(int requiredCount = 1)
        {
            return new QuestExpectedSignalDto
            {
                EventId = EventId,
                SourceId = SourceId,
                TargetId = TargetId,
                RequiredCount = Mathf.Max(1, requiredCount),
            };
        }
    }
}
