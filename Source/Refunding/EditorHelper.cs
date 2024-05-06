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
using UnityEngine;

namespace KSP_Recall.Refunds 
{
	[KSPAddon(KSPAddon.Startup.EditorAny, false)]
	public class EditorHelper : MonoBehaviour
	{
		#region Unity Life Cycle

		private void Awake()
		{
			Log.dbg("Awake on {0}", HighLogic.LoadedScene);
			if (Globals.Instance.FundsKeeper)
			{
				Log.dbg("Registering callbacks...");
				GameEvents.onEditorShipModified.Add(OnEditorShipModified);
			}
		}

		private void OnDestroy()
		{
			Log.dbg("OnDestroy");
			if (Globals.Instance.FundsKeeper)
			{
				Log.dbg("Unregistering callbacks...");
				GameEvents.onEditorShipModified.Remove(OnEditorShipModified);
			}
		}

		#endregion

		#region Events Handlers

		private void OnEditorShipModified(ShipConstruct data)
		{
			Log.dbg("OnEditorShipModified {0}", data.shipName);

			foreach (Part p in data.Parts) if (p.Modules.Contains<FundsKeeper>())
				p.Modules.GetModule<FundsKeeper>().AsynchronousUpdate();
		}

		#endregion

		private static KSPe.Util.Log.Logger Log = KSPe.Util.Log.Logger.CreateForType<EditorHelper>("KSP-Recall", "Refunding.EditorHelper", 0);
	}
}
