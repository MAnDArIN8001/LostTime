namespace Gameplay.Interaction.Character
{
    public interface ICharacterControlSessionPolicy
    {
        CharacterControlSessionPolicyDecision Evaluate(in CharacterControlSessionPolicyContext context);
    }
}
