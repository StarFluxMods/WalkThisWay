using Kitchen;
using KitchenMods;
using KitchenWalkThisWay.Components;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KitchenWalkThisWay.Systems
{
    public class InterceptPathing : DaySystem, IModSystem
    {
        private EntityQuery _customersWithIntercept;
        
        protected override void Initialise()
        {
            base.Initialise();
            _customersWithIntercept = GetEntityQuery(new QueryHelper().All(typeof(CPathIntercept), typeof(CPosition), typeof(CMoveToLocation)));
        }

        protected override void OnUpdate()
        {
            NativeArray<Entity> customersWithIntercept = _customersWithIntercept.ToEntityArray(Allocator.Temp);

            foreach (Entity customerWithIntercept in customersWithIntercept)
            {
                if (!Require(customerWithIntercept, out CPosition cPosition) || !Require(customerWithIntercept, out CMoveToLocation cMoveToLocation) || !Require(customerWithIntercept, out CPathIntercept cPathIntercept)) continue;
                if (Vector3.SqrMagnitude(cPosition - cPathIntercept.Target) > 0.5f)
                {
                    Set(customerWithIntercept, new CMoveToLocation
                    {
                        Location = cPathIntercept.Target
                    });
                }
                else
                {
                    Set(customerWithIntercept, cPathIntercept.CachedMoveToLocation);
                    Set(customerWithIntercept, new CRequestNewPathingLocation
                    {
                        PathingLocationIndex = cPathIntercept.InterceptCount + 1
                    });
                    EntityManager.RemoveComponent<CPathIntercept>(customerWithIntercept);
                }
            }
            
            customersWithIntercept.Dispose();
        }
    }
}