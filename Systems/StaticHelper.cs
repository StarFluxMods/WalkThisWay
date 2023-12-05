using Kitchen;
using Unity.Entities;

namespace KitchenWalkThisWay.Systems
{
    public class StaticHelper : GameSystemBase
    {
        private static StaticHelper _instance;

        protected override void Initialise()
        {
            _instance = this;
        }

        protected override void OnUpdate()
        {
            
        }
        
        public static bool HasComponent<T>(Entity entity) where T : struct, IComponentData
        {
            return _instance.Has<T>(entity);
        }
    }
}