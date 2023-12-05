using KitchenLib;
using KitchenLib.Logging;
using KitchenLib.Logging.Exceptions;
using KitchenMods;
using System.Linq;
using System.Reflection;
using KitchenWalkThisWay.Customs;
using UnityEngine;

namespace KitchenWalkThisWay
{
    public class Mod : BaseMod, IModSystem
    {
        private const string MOD_GUID = "com.starfluxgames.walkthisway";
        private const string MOD_NAME = "Walk This Way";
        private const string MOD_VERSION = "0.1.1";
        private const string MOD_AUTHOR = "StarFluxGames";
        private const string MOD_GAMEVERSION = ">=1.1.4";

        public static AssetBundle Bundle;
        public static KitchenLogger _logger;

        public Mod() : base(MOD_GUID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, MOD_GAMEVERSION, Assembly.GetExecutingAssembly()) { }

        protected override void OnInitialise()
        {
            _logger.LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnPostActivate(KitchenMods.Mod mod)
        {
            Bundle = mod.GetPacks<AssetBundleModPack>().SelectMany(e => e.AssetBundles).FirstOrDefault() ?? throw new MissingAssetBundleException(MOD_GUID);
            _logger = InitLogger();

            AddGameDataObject<PathingFlag>();
        }
    }
}

