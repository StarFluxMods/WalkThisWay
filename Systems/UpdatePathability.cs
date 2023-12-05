using System.Collections.Generic;
using Kitchen;
using KitchenMods;
using KitchenWalkThisWay.Components;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.AI;

namespace KitchenWalkThisWay.Systems
{
    public class UpdatePathability : TableUpdateSystem, IModSystem
    {
        private EntityQuery _pathingPoints;
        
        protected override void Initialise()
        {
            base.Initialise();
            _pathingPoints = GetEntityQuery(new QueryHelper().All(typeof(CPathingPoint)));
        }

        private readonly Dictionary<int, Entity> _pathingPointsByIndex = new Dictionary<int, Entity>();
        private readonly NavMeshPath _path = new NavMeshPath();
        protected override void OnUpdate()
        {
            _pathingPointsByIndex.Clear();
            NativeArray<Entity> pathingPoints = _pathingPoints.ToEntityArray(Allocator.Temp);
            
            foreach (Entity pathingPoint in pathingPoints)
            {
                if (Require(pathingPoint, out CPathingPoint cPathingPoint))
                    _pathingPointsByIndex.Add(cPathingPoint.PathingIndex, pathingPoint);
            }

            SPerformTableUpdate tableUpdate = GetSingleton<SPerformTableUpdate>();
            
            foreach (int pathingIndex in _pathingPointsByIndex.Keys)
            {
                if (!_pathingPointsByIndex.TryGetValue(pathingIndex, out Entity currentPathingPoint)) continue;
                if (!Require(currentPathingPoint, out CPosition cPosition)) continue;
                NavMesh.CalculatePath(tableUpdate.PathingSource, cPosition, NavMesh.AllAreas, _path);
                        
                if (_path.status != NavMeshPathStatus.PathComplete)
                {
                    if (!EntityManager.HasComponent<CUnpathable>(currentPathingPoint))
                        EntityManager.AddComponent<CUnpathable>(currentPathingPoint);
                }
                else
                {
                    if (EntityManager.HasComponent<CUnpathable>(currentPathingPoint))
                        EntityManager.RemoveComponent<CUnpathable>(currentPathingPoint);
                }

            }
            
            pathingPoints.Dispose();
        }
    }
}