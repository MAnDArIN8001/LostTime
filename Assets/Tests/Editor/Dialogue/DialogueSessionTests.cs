using System.Collections.Generic;
using Dialogue.Core;
using Dialogue.Runtime;
using NUnit.Framework;

namespace Tests.Dialogue
{
    public sealed class DialogueSessionTests
    {
        [Test]
        public void ContinueNode_AdvancesAndPublishesStepCompleted()
        {
            var definition = BuildContinueDialogue();
            var history = new DialogueHistory();
            var session = new DialogueSession(definition, history, new FakeQuestQuery(false));
            var completedNodeIds = new List<string>();

            session.StepCompleted += completedNodeIds.Add;

            Assert.That(session.Start(), Is.True);
            Assert.That(session.CurrentNode.NodeId, Is.EqualTo("start"));

            Assert.That(session.Continue(), Is.True);
            Assert.That(session.CurrentNode.NodeId, Is.EqualTo("end"));
            Assert.That(completedNodeIds, Is.EquivalentTo(new[] { "start" }));
        }

        [Test]
        public void ChoiceNode_PersistsHistoryAndCompletesStep()
        {
            var definition = BuildChoiceDialogue();
            var history = new DialogueHistory();
            var session = new DialogueSession(definition, history, new FakeQuestQuery(false));
            string selectedNode = null;
            string selectedOption = null;
            string completedNode = null;

            session.OptionSelected += (nodeId, optionId) =>
            {
                selectedNode = nodeId;
                selectedOption = optionId;
            };
            session.StepCompleted += nodeId => completedNode = nodeId;

            Assert.That(session.Start(), Is.True);
            Assert.That(session.SelectOption("opt_b"), Is.True);

            Assert.That(selectedNode, Is.EqualTo("start"));
            Assert.That(selectedOption, Is.EqualTo("opt_b"));
            Assert.That(completedNode, Is.EqualTo("start"));
            Assert.That(history.Entries.Count, Is.EqualTo(1));
            Assert.That(history.Entries[0].OptionId, Is.EqualTo("opt_b"));
        }

        [Test]
        public void QuestCompletedCondition_FiltersUnavailableBranch()
        {
            var definition = BuildQuestConditionDialogue();
            var history = new DialogueHistory();
            var session = new DialogueSession(definition, history, new FakeQuestQuery(false));

            Assert.That(session.Start(), Is.True);
            var options = session.GetAvailableOptions(session.CurrentNode);
            Assert.That(options.Count, Is.EqualTo(1));
            Assert.That(options[0].OptionId, Is.EqualTo("always"));
        }

        [Test]
        public void Break_DoesNotMarkDialogueCompleted()
        {
            var definition = BuildContinueDialogue();
            var history = new DialogueHistory();
            var session = new DialogueSession(definition, history, new FakeQuestQuery(false));

            Assert.That(session.Start(), Is.True);
            session.Break();

            Assert.That(session.Status, Is.EqualTo(DialogueStatus.Broken));
            Assert.That(history.CompletedDialogues.Contains(definition.DialogueId), Is.False);
        }

        private static DialogueDefinitionData BuildContinueDialogue()
        {
            return new DialogueDefinitionData
            {
                DialogueId = "d1",
                StartNodeId = "start",
                Nodes = new List<DialogueNodeData>
                {
                    new()
                    {
                        NodeId = "start",
                        NodeType = DialogueNodeType.Continue,
                        Options = new List<DialogueOptionData>
                        {
                            new() { OptionId = "c", NextNodeId = "end" }
                        }
                    },
                    new() { NodeId = "end", NodeType = DialogueNodeType.End }
                }
            };
        }

        private static DialogueDefinitionData BuildChoiceDialogue()
        {
            return new DialogueDefinitionData
            {
                DialogueId = "d2",
                StartNodeId = "start",
                Nodes = new List<DialogueNodeData>
                {
                    new()
                    {
                        NodeId = "start",
                        NodeType = DialogueNodeType.Choice,
                        Options = new List<DialogueOptionData>
                        {
                            new() { OptionId = "opt_a", NextNodeId = "end" },
                            new() { OptionId = "opt_b", NextNodeId = "end" }
                        }
                    },
                    new() { NodeId = "end", NodeType = DialogueNodeType.End }
                }
            };
        }

        private static DialogueDefinitionData BuildQuestConditionDialogue()
        {
            return new DialogueDefinitionData
            {
                DialogueId = "d3",
                StartNodeId = "start",
                Nodes = new List<DialogueNodeData>
                {
                    new()
                    {
                        NodeId = "start",
                        NodeType = DialogueNodeType.Choice,
                        Options = new List<DialogueOptionData>
                        {
                            new() { OptionId = "always", NextNodeId = "end" },
                            new()
                            {
                                OptionId = "quest",
                                NextNodeId = "end",
                                Conditions = new List<DialogueConditionData>
                                {
                                    new() { ConditionType = DialogueConditionType.QuestCompleted, Key = "quest.main" }
                                }
                            }
                        }
                    },
                    new() { NodeId = "end", NodeType = DialogueNodeType.End }
                }
            };
        }

        private sealed class FakeQuestQuery : IQuestStateQuery
        {
            private readonly bool _completed;

            public FakeQuestQuery(bool completed)
            {
                _completed = completed;
            }

            public bool IsQuestCompleted(string questId)
            {
                return _completed;
            }
        }
    }
}
