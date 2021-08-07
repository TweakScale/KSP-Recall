﻿/*
	This file is part of LetsStayTogether, a component of KSP-Recall
	(C) 2020-2021 Lisias T : http://lisias.net <support@lisias.net>

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

namespace KSP_Recall { namespace StayingTogether 

{
	[KSPAddon(KSPAddon.Startup.Flight, false)]
	public class FlightHelper : MonoBehaviour
	{
		private readonly FlightKscTrackingHelper helper = new FlightKscTrackingHelper();
		private void Awake() { this.helper.Awake(); }
		private void OnDestroy() { this.helper.OnDestroy(); }
	}

	[KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
	public class SpaceCentreHelper : MonoBehaviour
	{
		private readonly FlightKscTrackingHelper helper = new FlightKscTrackingHelper();
		private void Awake() { this.helper.Awake(); }
		private void OnDestroy() { this.helper.OnDestroy(); }
	}

	[KSPAddon(KSPAddon.Startup.TrackingStation, false)]
	public class TrackingStationHelper : MonoBehaviour
	{
		private readonly FlightKscTrackingHelper helper = new FlightKscTrackingHelper();
		private void Awake() { this.helper.Awake(); }
		private void OnDestroy() { this.helper.OnDestroy(); }
	}

	internal class FlightKscTrackingHelper
	{
		#region Unity Life Cycle

		internal void Awake()
		{
			Log.dbg("Awake on {0}", HighLogic.LoadedScene);
			if (Globals.Instance.Refunding)
			{
				GameEvents.onVesselGoOnRails.Add(OnVesselGoOnRails);
				GameEvents.onVesselGoOffRails.Add(OnVesselGoOffRails);
			}
		}

		internal void OnDestroy()
		{
			Log.dbg("OnDestroy");
			if (Globals.Instance.Refunding)
			{
				GameEvents.onVesselGoOffRails.Add(OnVesselGoOffRails);
				GameEvents.onVesselGoOnRails.Remove(OnVesselGoOnRails);
			}
		}

		#endregion

		#region Events Handlers

		private void OnVesselGoOnRails(Vessel vessel)
		{
			Log.dbg("OnVesselGo ON Rails {0}", vessel.vesselName);
			this.ImmediateUpdate(vessel);
		}

		// Continuing from the the OnRails stunt, when the vessel is unpacked we need to remove the Resource from
		// the stackable parts, so they could be stackable again.
		//
		// Since vessels are recovered while flying or while packed, I think this stunt my stick...
		private void OnVesselGoOffRails(Vessel vessel)
		{
			Log.dbg("OnVesselGo OFF Rails {0}", vessel.vesselName);
			this.ImediateRestore(vessel);
		}

		#endregion

		private void ImmediateUpdate(Vessel vessel)
		{
			foreach (Part p in vessel.Parts) if (p.Modules.Contains<LetsStayTogether>())
				p.Modules.GetModule<LetsStayTogether>().SynchronousUpdate();
		}

		private void ImediateRestore(Vessel vessel)
		{
			foreach (Part p in vessel.Parts) if (p.Modules.Contains<LetsStayTogether>())
				p.Modules.GetModule<LetsStayTogether>().SynchronousRestore();
		}
		private static KSPe.Util.Log.Logger Log = KSPe.Util.Log.Logger.CreateForType<FlightHelper>("KSP-Recall", "StayingTogether-FlightHelper");
	}
} }