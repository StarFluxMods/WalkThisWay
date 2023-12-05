using Kitchen;
using KitchenMods;
using KitchenWalkThisWay.Components;
using MessagePack;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Font = Kitchen.Font;

namespace KitchenWalkThisWay.Systems
{
    public class FlagView : UpdatableObjectView<FlagView.ViewData>
	{
		#region ECS View System (Runs on host and updates views to be broadcasted to clients)
		public class UpdateView : IncrementalViewSystemBase<ViewData>, IModSystem
		{
			private EntityQuery _views;

			protected override void Initialise()
			{
				base.Initialise();

				_views = GetEntityQuery(new QueryHelper().All(typeof(CPathingPoint), typeof(CLinkedView)));
			}

			protected override void OnUpdate()
			{
				using NativeArray<Entity> entities = _views.ToEntityArray(Allocator.Temp);
				using NativeArray<CLinkedView> views = _views.ToComponentDataArray<CLinkedView>(Allocator.Temp);

				for (int i = 0; i < views.Length; i++)
				{
					CLinkedView view = views[i];

					if (!Require(entities[i], out CPathingPoint cPathingPoint)) continue;
					bool isPathable = !Has<CUnpathable>(entities[i]) && !Has<CDestroyApplianceAtDay>(entities[i]);
					
					ViewData data = new ViewData
					{
						IndexID = cPathingPoint.PathingIndex,
						IsPathable = isPathable
					};

					SendUpdate(view, data);
				}
			}
		}
		#endregion

		#region Message Packet
		[MessagePackObject()]
		public struct ViewData : ISpecificViewData, IViewData.ICheckForChanges<ViewData>
		{
			[Key(1)] public int IndexID;
			[Key(2)] public bool IsPathable;

			public IUpdatableObject GetRelevantSubview(IObjectView view) => view.GetSubView<FlagView>();

			public bool IsChangedFrom(ViewData cached)
			{
				return IndexID != cached.IndexID || IsPathable != cached.IsPathable;
			}
		}
		#endregion

		public TextMeshPro text1;
		public TextMeshPro text2;
		protected override void UpdateData(ViewData viewData)
		{
			text1.gameObject.SetActive(true);
			text2.gameObject.SetActive(true);
			text1.font = ModuleDirectory.Main.Get(Font.Title);
			text2.font = ModuleDirectory.Main.Get(Font.Title);
			text1.color = viewData.IsPathable ? Color.white : Color.red;
			text2.color = viewData.IsPathable ? Color.white : Color.red;
			text1.text = viewData.IndexID.ToString();
			text2.text = viewData.IndexID.ToString();
		}
	}
}