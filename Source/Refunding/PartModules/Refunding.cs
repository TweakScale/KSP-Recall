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

namespace KSP_Recall.Refunds 
{
	public class Refunding : Abstract
	{
		internal const string RESOURCENAME = "RefundingForKSP111x";
		private decimal costFix = 0;
		protected override float CostFix => Convert.ToSingle(this.costFix);

		private FundsKeeper fk;

		// This crappy escuse of a code is neeed due https://github.com/net-lisias-ksp/KSP-Recall/issues/23
		private static PartResourceDefinition __PRD;
		protected override PartResourceDefinition PRD => __PRD??(__PRD = PartResourceLibrary.Instance.GetDefinition(this.ResouceName));

		public Refunding() : base(
							true
							, RESOURCENAME
							, KSPe.Util.Log.Logger.CreateForType<Refunding>("KSP-Recall", "Refunding"
							, 0)
		) { }

		#region KSP UI

		[KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "KSP-Recall::Refunding")]
		[UI_Toggle(disabledText = "Disabled", enabledText = "Enabled", scene = UI_Scene.Editor)]
		public bool active = false;
		protected override bool IsActive { get => this.active; }

		#endregion

		#region KSP Life Cycle

		public override void OnAwake()
		{
			base.OnAwake();
			this.active = Globals.Instance.Refunding;
		}

		public override void OnStart(StartState state)
		{
			base.OnStart(state);
			this.fk = this.part.Modules.GetModule<FundsKeeper>();
		}

		public override void OnLoad(ConfigNode node)
		{
			base.OnLoad(node);
			if (null == this.fk) 			this.fk = this.part.Modules.GetModule<FundsKeeper>();

			// Salvage previous savegames
			if (node.HasValue("OriginalCost"))
			{
				string value = node.GetValue("OriginalCost");
				decimal originalCost = decimal.TryParse(value, out originalCost) ? originalCost : 0;
				this.fk.OriginalCostSalvaged = originalCost.ToString();
				Log.dbg("Salvaged savegame with older Refunding. OriginalCost = {0}", value);
			}
		}

		#endregion

		protected override void Calculate() { }

		protected override void Recalculate()
		{
			Log.dbg("Recalculate {0}", this.PartInstanceId);

			if (this.part.Modules.Contains<KerbalEVA>() && this.part.Modules.Contains("ModuleInventoryPart"))
			{	// Now overcomes the **Stock** double refunding on Resources from Parts inside a ModuleInventoryPart
				// See https://github.com/net-lisias-ksp/KSP-Recall/issues/16#issuecomment-820346879
				IPartCostModifier pcm = this.part.Modules["ModuleInventoryPart"] as IPartCostModifier;
				decimal fix = Convert.ToDecimal(pcm.GetModuleCost(0, ModifierStagingSituation.CURRENT));
				Log.dbg("This part is a Kerbal with ModuleInventoryPart. Deducting {0}", fix);
				this.costFix = -fix;
				// TODO: Check if there's not a situation where Kerbals would be carrying resources themselves...
			}
			else
				this.costFix = this.partCost.CalculateModulesCost();

			Log.dbg("Recalculate Results originalCost: {0}; currentCost:{1:0.0}; fix:{2:0.0} ; ", this.fk.OriginalCost,  this.fk.CurrentCost, this.costFix);
		}
	}
}
