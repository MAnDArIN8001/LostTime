using System;
using UnityEngine;

namespace Gameplay.Guide.Core
{
    [Serializable]
    public struct GuideStoryMetadata
    {
        [SerializeField] private string _title;
        [SerializeField, TextArea(2, 8)] private string _text;

        public GuideStoryMetadata(string title, string text)
        {
            _title = title;
            _text = text;
        }

        public string Title => string.IsNullOrWhiteSpace(_title) ? string.Empty : _title.Trim();
        public string Text => string.IsNullOrWhiteSpace(_text) ? string.Empty : _text.Trim();
    }

    public readonly struct GuideStoryRequestedEvent
    {
        public GuideStoryRequestedEvent(GuideStoryMetadata metadata)
        {
            Metadata = metadata;
        }

        public GuideStoryMetadata Metadata { get; }
    }
}
