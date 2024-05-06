/*
	This file is part of Attached, a component of KSP-Recall
		© 2020-2023 LisiasT : http://lisias.net <support@lisias.net>

	KSP-Recall is double licensed, as follows:
		* SKL 1.0 : https://ksp.lisias.net/SKL-1_0.txt
		* GPL 2.0 : https://www.gnu.org/licenses/gpl-2.0.txt

	And you are allowed to choose the License that better suit your needs.

	KSP-Recall is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

	You should have received a copy of the SKL Standard License 1.0
	along with KSP-Recall. If not, see <https://ksp.lisias.net/SKL-1_0.txt>.

	You should have received a copy of the GNU General Public License 2.0
	along with KSP-Recall. If not, see <https://www.gnu.org/licenses/>.

*/
using System;
using System.Reflection;

namespace KSP_Recall.Refunds 
{
	public abstract class Abstract : PartModule, IPartCostModifier
	{
		// ModuleCartoPart is used by Stock Stackable parts, and these parts cannot have a State (i.e., anything that would change after spawning).
		// And this include Resources.
		internal const string MODULECARGOPART = "ModuleCargoPart";
		internal const float THRESHOLD = 0.01f;

		protected readonly string ResouceName;
		protected readonly bool UseResource;

		internal Abstract(bool useResource, string resourceName, KSPe.Util.Log.Logger log)
		{
			this.ResouceName = resourceName;
			this.UseResource = useResource;
			this.Log = log;
		}

		protected abstract bool IsActive { get; }
		protected abstract float CostFix { get; }
		protected abstract PartResourceDefinition PRD { get; }
		internal PartCostHelper partCost { get; private set; }

		private Part _prefab = null;
		private Part prefab { get => _prefab ?? (_prefab = this.part.partInfo.partPrefab); set => _prefab = value; }
		private int delayTicks = 0;
		private PartResource pr;	// Buffer to store the synthetical Part Resource (avoiding overloading the GC, see Issue #21 on GitHub.

		#region KSP Life Cycle

		public override void OnAwake()
		{
			Log.dbg("OnAwake {0}", this.PartInstanceId);
			base.OnAwake();
			// Here we have a problem. Some parts have the this.part.partInfo set, others don't.
			// Don't know why, and this is worrying - TweakScale depends on this! Is this a race condition?

			// Keep this disabled until someone need the recalculated costs, to avoiding being incorrectly charged on Launch
			// (we are charged after the Craft it's initialised on launch)
			this.enabled = false;
		}

		public override void OnStart(StartState state)
		{
			Log.dbg("OnStart {0} {1} {2}", this.PartInstanceId, state, this.IsActive);
			base.OnStart(state);
			this.partCost = new PartCostHelper(this.part, this.Log);
		}

		public override void OnCopy(PartModule fromModule)
		{
			Log.dbg("OnCopy {0} from {1:X}", this.PartInstanceId, fromModule.part.GetInstanceID());
			base.OnCopy(fromModule);
		}

		public override void OnLoad(ConfigNode node)
		{
			Log.dbg("OnLoad {0} {1}", this.PartInstanceId, null != node);
			base.OnLoad(node);
			if (null == this.part.partInfo) this.prefab = this.part;

			// The code below should not be executed on game loading, as we don't have a real Part
			// on memory to work on.
			Log.dbg("HighLogic.LoadedScene {0}", HighLogic.LoadedScene);
			if (HighLogic.LoadedScene < GameScenes.MAINMENU) return;

			// Always clean up the Resource on loading, as we need to get rid of reminiscents
			// of the previous attempts. We don't want the Refunding resource on Edit Scene anyway.
			//
			this.ResetResource();

			if (!(this.IsActive && this.UseResource))
			{
				// Additionally, clean up Parts those Refunding was inactivated by the user.
				this.RemoveResourceIfAvailable();
				return;
			}

			// The cost of the craft is billed on launch, **after** loading the craft.
			//
			// So we need to postpone the Resource restoring until the last moment, or the
			// user will be overbilled.
			//
			// The OriginalCost is calcultated on Editing time, but the Resource will be updated
			// only at Flight time after launch.
 		}

		public override void OnSave(ConfigNode node)
		{
			Log.dbg("OnSave {0}", this.PartInstanceId);
			base.OnSave(node);
		}

		#endregion

		#region Unity Life Cycle

		private void FixedUpdate()
		{
			if (!this.IsActive) return;
			if (0 != --this.delayTicks) return;
			Log.dbg("FixedUpdate {0}", this.PartInstanceId);

			switch(HighLogic.LoadedScene)
			{
				case GameScenes.FLIGHT:
					this.SynchronousFullUpdate();
					break;
				case GameScenes.EDITOR:
					this.Calculate();
					break;
				default:
					break;
			}
			this.enabled = false;
		}

		private void OnDestroy()
		{
			Log.dbg("OnDestroy {0}:{1:X}", this.name, this.part.GetInstanceID());
			/// Log.dbg("OnDestroy {0}", this.PartInstanceId); This doesn't work here.
			this.pr = null;
		}

		#endregion

		protected abstract void Calculate();
		protected abstract void Recalculate();

		#region Part Events Handlers

		float IPartCostModifier.GetModuleCost(float defaultCost, ModifierStagingSituation sit)
		{
			// This yet another messy hack aims to work around problems on refundings
			// https://github.com/net-lisias-ksp/KSP-Recall/issues/16
			// See the link for a rationale for doing things this freasking way.
			//
			// TL;DR : **STOCK** is the one borking on refundings by ignoring this Interface
			// Everybody else should be doing the right thing. So the Refunding stunt would end
			// up screweing the good boys.
			//
			// But, and again, Stock is the one ignoring this interface. So by implementing it, 3rd
			// parties that does The Tight Thing™ will call this, and so the Refunding resource will be
			// counter acted for them.
			//
			// The net result is that only Stock (and anyone else borking on this thing) will not be counter-acted
			// but this. :)
			return !this.IsActive ? 0f
					// If the refunding resource is not present, there's no need for the hack
					: null == this.part.Resources.Get(this.ResouceName)	? 0f

					: this.UseResource
					? -this.CostFix

					// If we are not using the Resource Hack, then we behave as normal citizens.
					: this.CostFix
				;

		}

		ModifierChangeWhen IPartCostModifier.GetModuleCostChangeWhen()
			=> ModifierChangeWhen.CONSTANTLY;

#if false
		// This apparently is not needed, and we need to delay this thing anyway to avoiding initialising before
		// KSP charges for Launch, and it's easier to handle this without reluing on GameEvents.OnFundsChanged.
		[KSPEvent(guiActive = false, active = true)]
		void OnPartScaleChanged(BaseEventDetails data)
		{
			Log.dbg("OnPartScaleChanged");
			this.AsynchronousUpdate();
		}
#endif

		#endregion

		// Should be called while flight or editing, where you don't need to get the module updated on the spot.
		// This spreads the load on time, avoiding overloading the CPU on hot code.
		internal void AsynchronousUpdate(int delayTicks = 1)
		{
			this.delayTicks = delayTicks;
			this.enabled = this.IsActive;
		}

		// Should be called before iminent situations (as flight termination) where you *NEED* the thing updated before something "terminal" happens.
		// (screw the CPU, we need the data NOW).
		internal void SynchronousFullUpdate()
		{
			if (!this.IsActive) return; // Just in case someone call it directly

			this.Recalculate();
			if (this.UseResource)
			{ 
				this.UpdateResource();
				this.NotifyResourcesChanged();
			}
		}

		private void UpdateResource()
		{
			Log.dbg("UpdateResource {0}:{1:X}", this.part.partInfo.name, this.part.GetInstanceID());

			// No need to create the meta-resource if no fix is needed!
			if (Math.Abs(this.CostFix) < THRESHOLD)
			{ 
				Log.dbg("CostFix below the threshold ({0}). Removing the meta-resource and aborting.", this.CostFix);
				this.RemoveResourceIfAvailable();
				return;
			}

			if (this.RemoveResourceWhenNeeded()) // Giving up on handling Stackables for now. The Rails stunt didn't worked as expected...
				return;

			// Rebuild the Refund Resource if it was destroyed by something like a Fuel Switch
			PartResource pr = this.RestoreResource();

			Log.dbg("Before {0} {1} {2} {3}", pr.ToString(), pr.amount, pr.maxAmount, pr.info.unitCost);

			// One of the ugliest hacks I even did on KSP. Pretty nasty!! :P
			// (and being the only solution for while that handles FMRS and similar Add'Ons don't make it prettier!)

			// This will prevent the cost of the Refunding from being subtracked from the cost of the Part, what would counter-attack
			// the fix below
			FieldInfo field = typeof(PartResource).GetField("maxAmount", BindingFlags.Instance | BindingFlags.Public);
			field.SetValue(pr, 0.0000001d); // This aims to keep MAS happy. See https://github.com/net-lisias-ksp/KSP-Recall/issues/18

			// This effectivelly "steals back" the Funds lost by the KSP's current stunt (using the prefab's cost on recovering costs)
			// See https://github.com/net-lisias-ksp/KSP-Recall/issues/12
			field = typeof(PartResource).GetField("amount", BindingFlags.Instance | BindingFlags.Public);
			field.SetValue(pr, this.CostFix);

			Log.dbg("After {0} {1} {2} {3}", pr.ToString(), pr.amount, pr.maxAmount, pr.info.unitCost);
		}

		private void ResetResource()
		{
			Log.dbg("Resetting {0} on part {1}", this.ResouceName, this.PartInstanceId);
			if (null == this.pr) return;

			FieldInfo field = typeof(PartResource).GetField("maxAmount", BindingFlags.Instance | BindingFlags.Public);
			field.SetValue(this.pr, 1d);
			this.pr.amount = 0;
		}

		private void RemoveResourceIfAvailable()
		{
			Log.dbg("Removing {0} from part {1}", this.ResouceName, this.PartInstanceId);

			if (null == this.part.Resources) return;	// Oukey, this is a bug on KSP ou just an anti-feature? :-(
			PartResource pr = this.part.Resources.Get(this.ResouceName);
			if (null != pr) this.part.Resources.Remove(pr);
		}

		// Remove the Refunding Resource from parts with PartModuleCargo that can be stackable
		internal bool RemoveResourceWhenNeeded()
		{
			bool r;
			if (r = this.IsStackable())
			{
				Log.dbg("Part {0} is Stackable. Removed {1} support!", this.PartInstanceId, this.GetType().Name);
				this.RemoveResourceIfAvailable();
			}
			return r;
		}

		private bool IsStackable()
		{
			bool r = this.part.Modules.Contains(MODULECARGOPART);

			if (r) { // Parts with resources are not stackable.
				int count = 0;
				foreach (PartResource pr in this.part.Resources) if (!this.ResouceName.Equals(pr.resourceName))
					++count;
				r &= (0 == count);
			}

			// Yeah, I'm using reflection - so this PartModule is still useable on previous KSP versions if the user choose to fool around.
			// Hacky and messy, uh? ]:->
			if (r) { // If the part has resources, there's no point on check for the stackable
				System.Type type = System.Type.GetType(MODULECARGOPART, false, true);
				r &= (null != type);
				if (r) 
					foreach (PartModule pm in this.part.Modules) if (type.Equals(pm.GetType()))
					{
						FieldInfo field = type.GetField("stackableQuantity", BindingFlags.Instance | BindingFlags.Public);
						r &= (null != field) && ((int)field.GetValue(pm) > 0);
						break;
					}
			}

			Log.dbg("{0} is {1}", this.PartInstanceId, r ? "stackable" : "not stackable");
			return r;
		}

#if true
		protected PartResource RestoreResource()
		{
			// THIS SHOULD NOT BE CALLED when this.active is false, or it may inject a Refunding resource when none is desired.

			if (null == this.pr)
			{
				this.pr = new PartResource(this.part);
				{
					this.pr.SetInfo(PRD);
					this.pr.maxAmount = 1;
					this.pr.amount = 0;
					this.pr.flowState = true;
					this.pr.isTweakable = PRD.isTweakable;
					this.pr.isVisible = PRD.isVisible;
					this.pr.hideFlow = true;
					this.pr.flowMode = PartResource.FlowMode.None;
				}
			}

			PartResource pr = this.part.Resources.Get(this.ResouceName);
			if (null == pr)
			{
				// This gets rid of that pesky log entries like this one:
				//	[LOG 00:02:31.256] [PartSet]: Failed to add Resource 1566956177 to Simulation PartSet:60079 as corresponding Part Mk0 Liquid Fuel Fuselage-4274492751 SimulationResource was not found.
				this.part.Resources.dict.Add(PRD.name.GetHashCode(), this.pr);
				pr = this.part.Resources.Get(this.ResouceName);
			}

			this.ResetResource();

			return pr;
		}
#else
		protected void RestoreResource()
		{
			Log.dbg("RestoreResource {0}", this.PartInstanceId);
			PartResource pr = this.part.Resources.Get(RESOURCENAME);
			Log.dbg("Before {0} {1} {2} {3}", pr.ToString(), pr.amount, pr.maxAmount, pr.info.unitCost);
			pr.SetInfo(PartResourceLibrary.Instance.GetDefinition(RESOURCENAME));
			Log.dbg("After {0} {1} {2} {3}", pr.ToString(), pr.amount, pr.maxAmount, pr.info.unitCost);
		}

		private PartResourceDefinition CreateCustomResourceDef(float unitCost)
		{
			PartResourceDefinition prd = PartResourceLibrary.Instance.GetDefinition(RESOURCENAME);
			Log.dbg("ConfigNode Source: {0}", prd.Config.ToString());

			ConfigNode cn = new ConfigNode("RESOURCE_DEFINITION");
			cn.SetValue("unitCost", unitCost, true);
			prd.Save(cn);
			PartResourceDefinition r = new PartResourceDefinition();
			r.Load(cn);
			return r;
		}
#endif
		protected void NotifyResourcesChanged()
		{
			// Place holder. Find a way to induce KSP to save the part again.
		}

		protected string PartInstanceId => string.Format("{0}-{1}:{2:X}", this.VesselName, this.part.name, this.part.GetInstanceID());
		protected string VesselName => null == this.part.vessel ? "<NO VESSEL>" : this.part.vessel.vesselName ;

		protected readonly KSPe.Util.Log.Logger Log;
	}
}
