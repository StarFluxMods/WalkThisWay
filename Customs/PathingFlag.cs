using System.Collections.Generic;
using KitchenData;
using KitchenLib.Customs;
using KitchenLib.Utils;
using KitchenWalkThisWay.Components;
using KitchenWalkThisWay.Systems;
using TMPro;
using UnityEngine;

namespace KitchenWalkThisWay.Customs
{
    public class PathingFlag : CustomAppliance
    {
        public override string UniqueNameID => "PathingFlag";
        public override GameObject Prefab => MaterialUtils.AssignMaterialsByNames(Mod.Bundle.LoadAsset<GameObject>("FlagPole"));
        public override List<(Locale, ApplianceInfo)> InfoList => new List<(Locale, ApplianceInfo)>
        {
            (Locale.English, LocalisationUtils.CreateApplianceInfo("Pathing Flag", "Walk This Way Please.", new List<Appliance.Section>
            {
                new Appliance.Section
                {
                    Title = "Customer Pathing",
                    Description = "Customers will detour from their path to walk to this flag."
                }
            }, new List<string>()))
        };
        public override List<IApplianceProperty> Properties => new List<IApplianceProperty>
        {
            new CRequestPathingPoint()
        };

        public override bool IsPurchasable => true;
        public override ShoppingTags ShoppingTags => ShoppingTags.Automation;
        public override RarityTier RarityTier => RarityTier.Rare;
        public override PriceTier PriceTier => PriceTier.Expensive;

        public override void OnRegister(Appliance gameDataObject)
        {
            FlagView flagView = gameDataObject.Prefab.AddComponent<FlagView>();
            flagView.text1 = GameObjectUtils.GetChildObject(gameDataObject.Prefab, "Text_1").GetComponent<TextMeshPro>();
            flagView.text2 = GameObjectUtils.GetChildObject(gameDataObject.Prefab, "Text_2").GetComponent<TextMeshPro>();
        }
    }
}