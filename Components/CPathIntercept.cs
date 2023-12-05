using Kitchen;
using KitchenMods;
using UnityEngine;

namespace KitchenWalkThisWay.Components
{
    public struct CPathIntercept : IModComponent
    {
        public Vector3 Target;

        public CMoveToLocation CachedMoveToLocation;
        
        public int InterceptCount;
    }
}