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
using UnityEngine;

namespace KSP_Recall { namespace Attached 

{
	[KSPAddon(KSPAddon.Startup.EditorAny, false)]
	public class EditorHelper : MonoBehaviour
	{
		#region Unity Life Cycle

		private void Awake()
		{
			Log.dbg("Awake on {0}", HighLogic.LoadedScene);
			if (Globals.Instance.Attached) GameEvents.onEditorShipModified.Add(OnEditorShipModified);
		}

		private void OnDestroy()
		{
			Log.dbg("OnDestroy");
			if (Globals.Instance.Attached) GameEvents.onEditorShipModified.Remove(OnEditorShipModified);
		}

		#endregion

		#region Events Handlers

		private void OnEditorShipModified(ShipConstruct data)
		{
			Log.dbg("OnEditorShipModified {0}", data.shipName);
			foreach (Part p in data.Parts) if (this.IsPartChanged(p))
				Pool.ATTACHMENTS.Update(p);
		}

		#endregion

		private Boolean IsPartChanged(Part p)
		{
			// Trying to overrule unchanged parts the earlier we can (saving time).
			if (!Pool.ATTACHMENTS.HasSomething(p)) return true;	// If we don't have anything from this part, we need to register it

			List<Pool.Attachment_t> list = Pool.ATTACHMENTS.Get(p);
			if (list.Count != p.Resources.Count) return true;	// If the count of resources are different from what we have, we need to refresh

			int count = 0;
			foreach (Pool.Attachment_t at in list)
			{
				bool found = false;
				foreach (AttachNode an in p.attachNodes)
					if(at.Info.id == an.id)
					{
						found = true;
						++count;
						if(!at.Info.Equals(an)) return true;
						break;
					}
				if(!found) return true;						// Some attach node on the Pool wasn't found on the Part. We need to refresh.
			}
			if(count != p.attachNodes.Count) return true;	// If any attach node from the Part wasn't compared, it's because we have a new attach node there. We need to refresh

			return false;
		}

		private static KSPe.Util.Log.Logger Log = KSPe.Util.Log.Logger.CreateForType<Attached>("KSP-Recall", "Attached-EditorHelper", 0);
	}
} }