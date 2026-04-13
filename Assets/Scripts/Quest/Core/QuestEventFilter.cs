using System;

namespace Quest.Core
{
    [Serializable]
    public sealed class QuestEventFilter
    {
        public string EventId = string.Empty;
        public string SourceId = string.Empty;
        public string TargetId = string.Empty;

        public bool Matches(in QuestEventData eventData)
        {
            if (!MatchesToken(EventId, eventData.EventId))
            {
                return false;
            }

            if (!MatchesToken(SourceId, eventData.SourceId))
            {
                return false;
            }

            return MatchesToken(TargetId, eventData.TargetId);
        }

        public QuestEventFilter Clone()
        {
            return new QuestEventFilter
            {
                EventId = EventId,
                SourceId = SourceId,
                TargetId = TargetId,
            };
        }

        private static bool MatchesToken(string expected, string actual)
        {
            if (string.IsNullOrWhiteSpace(expected))
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(actual))
            {
                return false;
            }

            return string.Equals(expected.Trim(), actual.Trim(), StringComparison.OrdinalIgnoreCase);
        }
    }
}
