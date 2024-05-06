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
using System.Collections.Generic;

using KSPe.Util.Log;

namespace KSP_Recall.Refunds 
{
	internal class PartCostHelper
	{
		private static readonly HashSet<String> IGNORED_RESOURCES = new HashSet<String> { Refunding.RESOURCENAME, StealBackMyFunds.RESOURCENAME};
		private readonly Part part;
		private Logger Log { get; }

		public PartCostHelper(Part part, KSPe.Util.Log.Logger log)
		{
			this.part = part;
			this.Log = log;
		}

		public decimal CalculateCurrentCost()
		{
			decimal r = Convert.ToDecimal(this.part.partInfo.cost);
			r += this.CalculateResourcesCost();
			r += this.CalculateModulesCost();
			return r;
		}

		public decimal CalculateResourcesCost()
		{
			decimal r = 0;
			foreach (PartResource pr in this.part.Resources) if (!IGNORED_RESOURCES.Contains(pr.resourceName))
				r += CalculateResourceCost(pr);
			return r;
		}

		public decimal CalculateModulesCost()
		{
			decimal r = 0;
			foreach (PartModule pm in this.part.Modules) if (
				(pm is IPartCostModifier)
				&& !(pm is Abstract)
				&& !"ModuleInventoryPart".Equals(pm.GetType().Name) // Avoiding a hardcoded dependency
			)
			{
				r += CalculateModuleCost(pm);
			}
			return r;
		}

		private decimal CalculateResourceCost(PartResource pr)
		{
			decimal cost = (null != pr.info ? Convert.ToDecimal(pr.amount) * Convert.ToDecimal(pr.info.unitCost) : 0M); // Why some resources have no info? o.O
			// Why this.part.vessel is NULL at this point? :/
			//Log.dbg("CalculateResourcesCost({0},{1},{2}) => {3}", this.VesselName, this.part.partInfo.partName, pr.resourceName, cost);
			// Answer: because the part was detached from a vessel, being moved from a storage to another!
			Log.dbg("CalculateResourcesCost({0},{1}) => {2}", this.part.partInfo.name, pr.resourceName, cost);
			return cost;
		}

		private decimal CalculateResourceCost(ProtoPartResourceSnapshot pr)
		{
			decimal r = Convert.ToDecimal(pr.amount) * Convert.ToDecimal(pr.definition.unitCost);
			Log.dbg("CalculateResourcesCost({0},Proto:{1}) => {2}", this.part.partInfo.name, pr.resourceName, r);
			return r;
		}

		private decimal CalculateModuleCost(PartModule pm)
		{
			decimal r = Convert.ToDecimal(((IPartCostModifier)pm).GetModuleCost(0, ModifierStagingSituation.CURRENT));

			// That could had been a good idea - if it had worked.
			// But it was denying the refund of complex parts with dry cost and resources together, so...
			// if ("ModuleInventoryPart".Equals(pm.moduleName)) r *= -1; // Fix the Overrefunding from Stock

			Log.dbg("CalculateModulesCost({0},{1}) => {2}", this.part.partInfo.name, pm.moduleName, r);
			return r;
		}
	}
}
