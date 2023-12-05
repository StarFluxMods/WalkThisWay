using Kitchen;
using KitchenMods;
using KitchenWalkThisWay.Components;
using Unity.Collections;
using Unity.Entities;

namespace KitchenWalkThisWay.Systems
{
    public class InterceptNewCustomers : DaySystem, IModSystem
    {
        private EntityQuery _customers;
        
        protected override void Initialise()
        {
            base.Initialise();
            _customers = GetEntityQuery(new QueryHelper()
                .All(typeof(CCustomer), typeof(CMoveToLocation), typeof(CBelongsToGroup))
                .None(typeof(CCompletedIntercept), typeof(CRequestNewPathingLocation), typeof(CPathIntercept)));
        }

        protected override void OnUpdate()
        {
            NativeArray<Entity> customers = _customers.ToEntityArray(Allocator.Temp);
            
            foreach (Entity customer in customers)
            {
                if (!Require(customer, out CBelongsToGroup cBelongsToGroup)) continue;
                if (cBelongsToGroup.Group == Entity.Null) continue;
                if (Has<CAssignedTable>(cBelongsToGroup.Group) || Has<CAssignedStand>(cBelongsToGroup.Group))
                {
                    Set(customer, new CRequestNewPathingLocation
                    {
                        PathingLocationIndex = 1
                    });
                }

            }
            
            customers.Dispose();
        }
    }
}