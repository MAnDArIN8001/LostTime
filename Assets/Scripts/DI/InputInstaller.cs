using System;
using Zenject;

namespace DI
{
    public class InputInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            var input = new MainInput();
            
            input.Enable();
            
            Container.Bind<MainInput>().FromInstance(input).AsSingle();
        }

        public void OnDestroy()
        {
            var input = Container.Resolve<MainInput>();
            
            input.Disable();
            input.Dispose();
        }
    }
}