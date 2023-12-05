using System.Collections.Generic;
using Kitchen;
using KitchenMods;
using KitchenWalkThisWay.Components;
using Unity.Collections;
using Unity.Entities;

namespace KitchenWalkThisWay.Systems
{
    public class FillInterceptRequests : DaySystem, IModSystem
    {
        private EntityQuery _customerRequests;
        private EntityQuery _pathingPoints;
        
        protected override void Initialise()
        {
            base.Initialise();
            _customerRequests = GetEntityQuery(new QueryHelper().All(typeof(CMoveToLocation), typeof(CRequestNewPathingLocation)).None(typeof(CPathIntercept)));
            _pathingPoints = GetEntityQuery(new QueryHelper().All(typeof(CPathingPoint)).None(typeof(CUnpathable)));
        }
        
        private readonly Dictionary<int, Entity> _pathingPointsByIndex = new Dictionary<int, Entity>();
        
        protected override void OnUpdate()
        {
            _pathingPointsByIndex.Clear();
            
            NativeArray<Entity> customerRequests = _customerRequests.ToEntityArray(Allocator.Temp);
            NativeArray<Entity> pathingPoints = _pathingPoints.ToEntityArray(Allocator.Temp);
            
            foreach (Entity pathingPoint in pathingPoints)
            {
                if (Require(pathingPoint, out CPathingPoint cPathingPoint))
                    _pathingPointsByIndex.Add(cPathingPoint.PathingIndex, pathingPoint);
            }
            
            foreach (Entity customer in customerRequests)
            {
                if (!Require(customer, out CRequestNewPathingLocation cRequestNewPathingLocation) || !Require(customer, out CMoveToLocation cMoveToLocation)) continue;
                Entity nextPathingPoint = GetNextValidPathingPoint(cRequestNewPathingLocation.PathingLocationIndex);
                if (nextPathingPoint != Entity.Null)
                {
                    if (!Require(nextPathingPoint, out CPosition cPosition)) continue;
                    Set(customer, new CPathIntercept
                    {
                        Target = cPosition,
                        CachedMoveToLocation = cMoveToLocation,
                        InterceptCount = cRequestNewPathingLocation.PathingLocationIndex
                    });
                    EntityManager.RemoveComponent<CRequestNewPathingLocation>(customer);
                }
                else
                {
                    Set(customer, new CCompletedIntercept());
                }
            }
            
            customerRequests.Dispose();
            pathingPoints.Dispose();
        }

        private Entity GetNextValidPathingPoint(int point, int offset = 0)
        {
            while (true)
            {
                if (offset > 10)
                {
                    return Entity.Null;
                }

                if (_pathingPointsByIndex.TryGetValue(point, out Entity pathingPoint)) return pathingPoint;
                point = point + 1;
                offset = offset + 1;
            }
        }
    }
}