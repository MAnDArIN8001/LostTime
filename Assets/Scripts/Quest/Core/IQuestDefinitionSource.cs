namespace Quest.Core
{
    public interface IQuestDefinitionSource
    {
        QuestDefinitionData[] CreateDefinitions();
        QuestDefinitionData CreateDefinition();
    }
}
