using JetBrains.Annotations;

namespace Character.Modules.Animation.Facade
{
    public interface IAnimationFacade
    {
        public void Set(string name, [CanBeNull] object value);
    }
}