using System;
using UnityEngine;

namespace Quest.Core
{
    [Serializable]
    public sealed class QuestExpectedSignalDto
    {
        public string EventId = string.Empty;
        public string SourceId = string.Empty;
        public string TargetId = string.Empty;
        [Min(1)] public int RequiredCount = 1;

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

        public QuestExpectedSignalDto Clone()
        {
            return new QuestExpectedSignalDto
            {
                EventId = EventId,
                SourceId = SourceId,
                TargetId = TargetId,
                RequiredCount = Mathf.Max(1, RequiredCount),
            };
        }

        public void Normalize()
        {
            RequiredCount = Mathf.Max(1, RequiredCount);
            EventId = NormalizeToken(EventId);
            SourceId = NormalizeToken(SourceId);
            TargetId = NormalizeToken(TargetId);
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

        private static string NormalizeToken(string token)
        {
            return string.IsNullOrWhiteSpace(token) ? string.Empty : token.Trim();
        }
    }
}
