using Utils.Events;
using Character.Modules.Animation.Events;

namespace Character.Modules.Animation.Facade
{
    public class AnimationFacade : IAnimationFacade
    {
        private readonly EventBus _animationEventBus;

        public AnimationFacade(EventBus animationEventBus)
        {
            _animationEventBus = animationEventBus;
        }

        public void Set(string id, object value)
        {
            _animationEventBus.Publish<AnimationParamEvent>(new AnimationParamEvent() { AnimationParamId = id, Value = value});
        }
    }
}