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

using System.Collections.Generic;

namespace KSP_Recall
{
	public class ModuleManagerSupport : UnityEngine.MonoBehaviour
	{
		public static IEnumerable<string> ModuleManagerAddToModList()
		{
			List<string> tags = new List<string>();

			if (checkForResourceful())		tags.Add("KSPRECALL-RESOURCEFUL");
			if (checkForDriftless())		tags.Add("KSPRECALL-DRIFTLESS");
			if (checkForAttached())			tags.Add("KSPRECALL-ATTACHED");
			if (checkForChillingOut())		tags.Add("KSPRECALL-CHILLINGOUT");
			if (checkForAttachedOnEditor())	tags.Add("KSPRECALL-ATTACHED-ON-EDITOR");

			if (checkForRefunding() || checkForStealBack())
											tags.Add("KSPRECALL-FUNDSKEEPER");
			if (checkForRefunding())		tags.Add("KSPRECALL-REFUNDING");
			if (checkForStealBack())		tags.Add("KSPRECALL-STEALBACKFUNDS");

			if (checkForProceduralPartsAttachmentNodes())
											tags.Add("KSPRECALL-PROCEDURALPARTS-AN");

			return tags.ToArray();
		}

		private static bool checkForResourceful()
		{
			return (1 == KSPe.Util.KSP.Version.Current.MAJOR && 9 == KSPe.Util.KSP.Version.Current.MINOR);
		}

		private static bool checkForDriftless()
		{
			return (KSPe.Util.KSP.Version.Current >= KSPe.Util.KSP.Version.FindByVersion(1,8,0))
				&&
				(KSPe.Util.KSP.Version.Current < KSPe.Util.KSP.Version.FindByVersion(1,11,0))
			;
		}

		private static bool checkForAttached()
		{
			//return (KSPe.Util.KSP.Version.Current >= KSPe.Util.KSP.Version.FindByVersion(1,9,0));

			// I misundertood the problem - the original problem plaguing KSP >= 1.9 only happens on the Editor
			// It's completely unrelated to the Resources problem.
			//
			// I will keep this code alive, however, it may be useful for someone if some 3rd-Party misbehave.
			return false; 
		}

		private static bool checkForAttachedOnEditor()
		{
			return
				(KSPe.Util.KSP.Version.Current >= KSPe.Util.KSP.Version.FindByVersion(1,4,3))
				;
		}

		private static bool checkForChillingOut()
		{
			return (KSPe.Util.KSP.Version.Current == KSPe.Util.KSP.Version.FindByVersion(1,11,0));
		}

		private static bool checkForRefunding()
		{
			return KSPe.Util.KSP.Version.Current >= KSPe.Util.KSP.Version.FindByVersion(1,11,0); 
		}

		private static bool checkForProceduralPartsAttachmentNodes()
		{
			return KSPe.Util.SystemTools.Assembly.Exists.ByName("ProceduralParts")
				&& KSPe.Util.SystemTools.Assembly.Exists.ByName("Scale")
				;
		}

		// Every KSP release is prone to this problem at this moment.
		private static bool checkForStealBack() => true;

	}
}
