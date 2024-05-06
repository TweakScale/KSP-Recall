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

namespace KSP_Recall.Refunds 
{
	public class StealBackMyFunds : Abstract
	{
		private const bool visibleOnFlight = false;
		private const bool visibleOnEditor = true;

		internal const string RESOURCENAME = "StealBackMyFunds";
		internal new readonly decimal THRESHOLD = Convert.ToDecimal(Abstract.THRESHOLD);

		private float costFix = 0;
		protected override float CostFix { get => this.costFix; }
		private FundsKeeper fk;

		// This crappy escuse of a code is neeed due https://github.com/net-lisias-ksp/KSP-Recall/issues/23
		private static PartResourceDefinition __PRD;
		protected override PartResourceDefinition PRD => __PRD??(__PRD = PartResourceLibrary.Instance.GetDefinition(this.ResouceName));

		public StealBackMyFunds() : base(
							KSPe.Util.KSP.Version.Current >= KSPe.Util.KSP.Version.FindByVersion(1, 11, 0)
							, RESOURCENAME, KSPe.Util.Log.Logger.CreateForType<Refunding>("KSP-Recall", "StealBackMyFunds"
							, 0)
		) { }

		#region KSP UI

		[KSPField(isPersistant = true, guiActive = visibleOnFlight, guiActiveEditor = visibleOnEditor, guiName = "KSP-Recall::StealBackMyFunds")]
		[UI_Toggle(disabledText = "Disabled", enabledText = "Enabled", scene = UI_Scene.All)]
		public bool active = false;
		protected override bool IsActive { get => this.active; }

		#endregion

		#region KSP Life Cycle

		public override void OnAwake()
		{
			base.OnAwake();
			this.active = Globals.Instance.StealBackMyFunds;
		}

		public override void OnStart(StartState state)
		{
			base.OnStart(state);
			this.fk = this.part.Modules.GetModule<FundsKeeper>();
			{
				BaseField bf = this.Fields["active"];
				bf.guiActive = Globals.Instance.DebugMode || (visibleOnFlight && Globals.Instance.PawEntries);
				bf.guiActiveEditor = Globals.Instance.DebugMode || (visibleOnEditor && Globals.Instance.PawEntries);
			}
		}

		public override void OnLoad(ConfigNode node)
		{
			base.OnLoad(node);
			if (null == this.fk) 			this.fk = this.part.Modules.GetModule<FundsKeeper>();
		}

		#endregion

		protected override void Calculate() { }

		protected override void Recalculate()
		{
			// Mitigates the squashing effect caused by the iPartCostModifier being a float.
			// See https://github.com/net-lisias-ksp/KSP-Recall/issues/60
			decimal effectiveCostFix = this.fk.CurrentCost;
			float realCostFix = Convert.ToSingle(effectiveCostFix);
			int i = 16; // 16 interactions max.
			while (i > 0 && (this.fk.CurrentCost - Convert.ToDecimal(realCostFix) > THRESHOLD))
			{
				effectiveCostFix += (effectiveCostFix - Convert.ToDecimal(realCostFix));
				realCostFix = Convert.ToSingle(effectiveCostFix);
				--i;
				Log.dbg("Attempt {0} to minimizing the float squashing effect: effective={1} ; real={2}", i, effectiveCostFix, realCostFix);
			} 

			this.costFix = Convert.ToSingle(this.fk.CurrentCost) - realCostFix;
			if (Math.Abs(this.costFix) < Abstract.THRESHOLD)
				this.costFix = 0;
			else
				Log.warn("Your refunding for {0} was squashed by `IPartCostModifier` and was mangled to prevent losses ( see https://github.com/net-lisias-ksp/KSP-Recall/issues/60 ). Ideal value:{1} ; hack used instead:{2}", this.PartInstanceId, this.fk.CurrentCost, realCostFix);
		}
	}
}