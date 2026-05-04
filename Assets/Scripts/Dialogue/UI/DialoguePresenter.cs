using System.Collections.Generic;
using Dialogue.Core;
using Dialogue.Runtime;

namespace Dialogue.UI
{
    public sealed class DialoguePresenter
    {
        private readonly DialogueSession _session;
        private readonly DialoguePanel _view;

        public DialoguePresenter(DialogueSession session, DialoguePanel view)
        {
            _session = session;
            _view = view;
        }

        public void Render(DialogueNodeData node)
        {
            if (node == null || _view == null || _session == null)
            {
                return;
            }

            var options = _session.GetAvailableOptions(node);
            var optionViews = new List<DialoguePanel.OptionView>(options.Count);
            for (var i = 0; i < options.Count; i++)
            {
                var option = options[i];
                optionViews.Add(new DialoguePanel.OptionView
                {
                    OptionId = option.OptionId,
                    Text = option.Text
                });
            }

            var model = new DialogueViewModel(
                node.SpeakerName,
                node.Text,
                node.NodeType,
                optionViews,
                true);

            _view.Render(model);
        }
    }
}
