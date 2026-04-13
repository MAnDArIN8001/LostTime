using UnityEngine;

namespace Quest.Core
{
    public readonly struct QuestEventData
    {
        public QuestEventData(
            string eventId,
            string sourceId,
            string targetId,
            int countDelta,
            GameObject sourceObject,
            GameObject targetObject)
        {
            EventId = string.IsNullOrWhiteSpace(eventId) ? string.Empty : eventId.Trim();
            SourceId = string.IsNullOrWhiteSpace(sourceId) ? string.Empty : sourceId.Trim();
            TargetId = string.IsNullOrWhiteSpace(targetId) ? string.Empty : targetId.Trim();
            CountDelta = countDelta <= 0 ? 1 : countDelta;
            SourceObject = sourceObject;
            TargetObject = targetObject;
        }

        public string EventId { get; }
        public string SourceId { get; }
        public string TargetId { get; }
        public int CountDelta { get; }
        public GameObject SourceObject { get; }
        public GameObject TargetObject { get; }
    }
}
