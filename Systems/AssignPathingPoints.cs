using System.Collections.Generic;
using Kitchen;
using KitchenMods;
using KitchenWalkThisWay.Components;
using Unity.Collections;
using Unity.Entities;

namespace KitchenWalkThisWay.Systems
{
    public class AssignPathingPoints : GameSystemBase, IModSystem
    {
        private EntityQuery _requestPathingPoints;
        private EntityQuery _existingPathingPoints;
        
        protected override void Initialise()
        {
            base.Initialise();
            _requestPathingPoints = GetEntityQuery(new QueryHelper().All(typeof(CRequestPathingPoint)).None(typeof(CPathingPoint)));
            _existingPathingPoints = GetEntityQuery(new QueryHelper().All(typeof(CPathingPoint)));
        }

        private readonly Dictionary<int, Entity> _pathingPointsByIndex = new Dictionary<int, Entity>();
        
        protected override void OnUpdate()
        {
            _pathingPointsByIndex.Clear();
            NativeArray<Entity> requestPathingPoints = _requestPathingPoints.ToEntityArray(Allocator.Temp);
            NativeArray<Entity> existingPathingPoints = _existingPathingPoints.ToEntityArray(Allocator.Temp);
            
            foreach (Entity pathingPointEntity in existingPathingPoints)
            {
                if (Require(pathingPointEntity, out CPathingPoint cPathingPoint))
                    _pathingPointsByIndex.Add(cPathingPoint.PathingIndex, pathingPointEntity);
            }
            
            foreach (Entity requestPathingPointEntity in requestPathingPoints)
            {
                if (!EntityManager.HasComponent<CRequestPathingPoint>(requestPathingPointEntity)) continue;
                int pathingIndex = 1;
                while (_pathingPointsByIndex.ContainsKey(pathingIndex))
                    pathingIndex++;
                    
                EntityManager.AddComponentData(requestPathingPointEntity, new CPathingPoint { PathingIndex = pathingIndex });
                _pathingPointsByIndex.Add(pathingIndex, requestPathingPointEntity);
                EntityManager.RemoveComponent<CRequestPathingPoint>(requestPathingPointEntity);
            }

            requestPathingPoints.Dispose();
            existingPathingPoints.Dispose();
        }
    }
}