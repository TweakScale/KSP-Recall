/*
	This file is part of TestPersistentId, a component of KSP-Recall
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

namespace KSP_Recall.Test.PersistentId 
{
	[KSPAddon(KSPAddon.Startup.Flight, false)]
	public class FlightHelper : MonoBehaviour
	{
		private readonly FlightEditorTrackingHelper helper = new FlightEditorTrackingHelper();
		private void Awake() { this.helper.Awake(); }
		private void OnDestroy() { this.helper.OnDestroy(); }
	}

	[KSPAddon(KSPAddon.Startup.EditorAny, false)]
	public class EditorAnyHelper : MonoBehaviour
	{
		private readonly FlightEditorTrackingHelper helper = new FlightEditorTrackingHelper();
		private void Awake() { this.helper.Awake(); }
		private void OnDestroy() { this.helper.OnDestroy(); }
	}

	[KSPAddon(KSPAddon.Startup.TrackingStation, false)]
	public class TrackingStationHelper : MonoBehaviour
	{
		private readonly FlightEditorTrackingHelper helper = new FlightEditorTrackingHelper();
		private void Awake() { this.helper.Awake(); }
		private void OnDestroy() { this.helper.OnDestroy(); }
	}

	internal class FlightEditorTrackingHelper
	{
		#region Unity Life Cycle

		internal void Awake()
		{
			Log.info("Registering callbacks for {0}", HighLogic.LoadedScene);
			GameEvents.onPartPersistentIdChanged.Add(this.OnPartPersistentIdChanged);
		}

		internal void OnDestroy()
		{
			Log.info("Unregistering callbacks from {0}", HighLogic.LoadedScene);
			GameEvents.onPartPersistentIdChanged.Remove(this.OnPartPersistentIdChanged);
		}

		#endregion

		#region Events Handlers

		private void OnPartPersistentIdChanged(uint vesselPersistentId, uint fromPartPersistentId, uint toPartPersistentId)
		{
			Log.info("OnPartPersistentIdChanged!! Vessel {0} had the part with persistentId {1} renamed to {2}.", vesselPersistentId, fromPartPersistentId, toPartPersistentId);
		}

		#endregion

		private static KSPe.Util.Log.Logger Log = KSPe.Util.Log.Logger.CreateForType<FlightHelper>("KSP-Recall", "Test.PersistentId.Helper", 0);
	}
}
