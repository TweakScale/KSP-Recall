/*
	This file is part of Attached, a component of KSP-Recall
		© 2020-2024 LisiasT : http://lisias.net <support@lisias.net>

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
	public class FundsKeeper : PartModule
	{
		[KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
		public string OriginalCost = "0";
		internal string OriginalCostSalvaged = null;
		internal decimal OriginalCostValue => Convert.ToDecimal(this.OriginalCost);

		[KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
		public string OriginalResourcesCost = "0";
		internal decimal OriginalResourcesCostValue => Convert.ToDecimal(this.OriginalResourcesCost);

		[KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
		public string OriginalModulesCost = "0";
		internal decimal OriginalModulesCostValue => Convert.ToDecimal(this.OriginalModulesCost);

		internal decimal OriginalDryCost => this.OriginalCostValue - this.OriginalResourcesCostValue;

		internal decimal CurrentCost = 0;
		internal decimal CurrentResourcesCost = 0;
		internal decimal CurrentModulesCost = 0;
		internal decimal CurrentDryCost => this.CurrentCost - this.CurrentResourcesCost;

		private int delayTicks = 0;
		private PartResource pr;	// Buffer to store the synthetical Part Resource (avoiding overloading the GC, see Issue #21 on GitHub.
		private PartCostHelper partCost;

		#region KSP Life Cycle

		public override void OnAwake()
		{
			Log.dbg("OnAwake {0}", this.PartInstanceId);
			base.OnAwake();
			// Here we have a problem. Some parts have the this.part.partInfo set, others don't.
			// Don't know why, and this is worrying - TweakScale depends on this! Is this a race condition?

			// Keep this disabled until someone need the recalculated costs
			this.enabled = false;
		}

		public override void OnStart(StartState state)
		{
			Log.dbg("OnStart {0} {1}", this.PartInstanceId, state);
			base.OnStart(state);
			this.partCost = new PartCostHelper(this.part, Log);

			// Whoops... Ongoing savegame that never had FundsKeeper before. Let's try to salvage things!
			if ("0" == this.OriginalCost) this.OriginalCostSalvaged = this.partCost.CalculateCurrentCost().ToString();
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

			// The code below should not be executed on game loading, as we don't have a real Part
			// on memory to work on.
			if (HighLogic.LoadedScene < GameScenes.MAINMENU) return;
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
			if (0 != --this.delayTicks) return;
			Log.dbg("FixedUpdate {0}", this.PartInstanceId);

			switch(HighLogic.LoadedScene)
			{
				case GameScenes.FLIGHT:
					this.RecalculateCurrentCost();
					break;
				case GameScenes.EDITOR:
					this.CalculateOriginalCosts();
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

		// Should be called while flight or editing, where you don't need to get the module updated on the spot.
		// This spreads the load on time, avoiding overloading the CPU on hot code.
		internal void AsynchronousUpdate(int delayTicks = 1)
		{
			this.delayTicks = delayTicks;
			this.enabled = true;
		}

		// Should be called before iminent situations (as flight termination) where you *NEED* the thing updated before something "terminal" happens.
		// (screw the CPU, we need the data NOW).
		internal void SynchronousFullUpdate() =>
			this.RecalculateCurrentCost();

		private void CalculateOriginalCosts()
		{ 
			this.OriginalCost = this.partCost.CalculateCurrentCost().ToString();
			this.OriginalResourcesCost = this.partCost.CalculateResourcesCost().ToString();
			this.OriginalModulesCost = this.partCost.CalculateModulesCost().ToString();
			Log.dbg("CalculateOriginalCosts({0}) = {1}", this.PartInstanceId, this.OriginalCost);
		}

		private void RecalculateCurrentCost()
		{
			if (null != this.OriginalCostSalvaged) this.Salvage();
			this.CurrentCost = this.partCost.CalculateCurrentCost();
			this.CurrentResourcesCost = this.partCost.CalculateResourcesCost();
			this.CurrentModulesCost = this.partCost.CalculateModulesCost();
			Log.dbg("RecalculateCurrentCost({0}) = {1}", this.PartInstanceId, this.CurrentCost);
		}

		private void Salvage()
		{
			this.OriginalCost = this.OriginalCostSalvaged;
			this.OriginalCostSalvaged = null;

			// The following values **are wrong**, as we are fetching whatever the part have at the moment, and not at launch.
			// But it's better than nothing. There's no way to guess how the part was configured at launch, and we need some values here!
			this.OriginalResourcesCost = this.partCost.CalculateCurrentCost().ToString();
			this.OriginalModulesCost = this.partCost.CalculateModulesCost().ToString();
		}

		private string PartInstanceId => string.Format("{0}-{1}:{2:X}", this.VesselName, this.part.name, this.part.GetInstanceID());
		private string VesselName => null == this.part.vessel ? "<NO VESSEL>" : this.part.vessel.vesselName ;

		private static readonly KSPe.Util.Log.Logger Log = KSPe.Util.Log.Logger.CreateForType<Refunding>("KSP-Recall", "FundsKeeper", 0);
	}
}
