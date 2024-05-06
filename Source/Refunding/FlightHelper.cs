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

namespace KSP_Recall.Refunds 
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
			if (Globals.Instance.FundsKeeper)
			{
				Log.dbg("Registering callbacks...");
				//GameEvents.OnFundsChanged.Add(OnFundsChanged);
				//GameEvents.onGameSceneSwitchRequested.Add(OnSceneSwitchRequested);
				//GameEvents.onNewVesselCreated.Add(OnNewVesselCreated);
				//GameEvents.onVesselLoaded.Add(OnVesselLoaded);
				//GameEvents.onVesselDestroy.Add(OnVesselDestroy);
				//GameEvents.onVesselWasModified.Add(OnVesselWasModified);
				//GameEvents.onVesselPartCountChanged.Add(OnVesselPartCountChanged);
				GameEvents.OnVesselRecoveryRequested.Add(OnVesselRecoveryRequested);
				GameEvents.onVesselChange.Add(OnVesselChange);
				//GameEvents.onVesselSwitching.Add(OnVesselSwitching);
				//GameEvents.onVesselSwitchingToUnloaded.Add(OnVesselSwitching);
				GameEvents.onVesselGoOnRails.Add(OnVesselGoOnRails);
				//GameEvents.onVesselGoOffRails.Add(OnVesselGoOffRails);
			}
		}

		internal void OnDestroy()
		{
			Log.dbg("OnDestroy");
			if (Globals.Instance.FundsKeeper)
			{
				Log.dbg("Unregistering callbacks...");
				//GameEvents.onVesselGoOffRails.Add(OnVesselGoOffRails);
				GameEvents.onVesselGoOnRails.Remove(OnVesselGoOnRails);
				//GameEvents.onVesselSwitchingToUnloaded.Remove(OnVesselSwitching);
				//GameEvents.onVesselSwitching.Remove(OnVesselSwitching);
				GameEvents.onVesselChange.Remove(OnVesselChange);
				GameEvents.OnVesselRecoveryRequested.Remove(OnVesselRecoveryRequested);
				//GameEvents.onVesselPartCountChanged.Remove(OnVesselPartCountChanged);
				//GameEvents.onVesselWasModified.Remove(OnVesselWasModified);
				//GameEvents.onVesselDestroy.Remove(OnVesselDestroy);
				//GameEvents.onVesselLoaded.Remove(OnVesselLoaded);
				//GameEvents.onNewVesselCreated.Remove(OnNewVesselCreated);
				//GameEvents.onGameSceneSwitchRequested.Remove(OnSceneSwitchRequested);
				//GameEvents.OnFundsChanged.Remove(OnFundsChanged);
			}

			// note to myself - its of little use to calculate the resources lately after the OnSave event is called on the part.
			// TODO: Find a way to induce KSP to save the parts gain after the resource is reapplied, this will ensure
			// recovering costs on any situation. See NotifyResourcesChanged on PartModule.
			// Right now things are working on a fickle equilibrium. I don't like this. :(
		}

		#endregion

		#region Events Handlers

		private void OnSceneSwitchRequested(GameEvents.FromToAction<GameScenes, GameScenes> data)
		{
			Log.dbg("OnSceneSwitchRequested from {0} to {1}", data.from, data.to);
			if (GameScenes.FLIGHT != data.from) return;
			this.ImmediateUpdateAllAndClear();
		}

		// It's terribly important to clear this on every scene change (so the 3 distincts KSPAddOns above)
		// as only vessels Changed In or Spawned on the current scene are subject to be recalculated.
		// Starting from scratch on every supported scene is a quick & dirty way to guarantee that.
		private readonly List<Vessel> SpawnnedVessels = new List<Vessel>();

		// This is not being called when the vessel is spawned from the Editor. :/
		private void OnNewVesselCreated(Vessel vessel)
		{
			Log.dbg("OnNewVesselCreated {0}", vessel.vesselName);
			this.SpawnnedVessels.Add(vessel);
		}

		// This is not being called as expected. :(
		private void OnVesselLoaded(Vessel vessel)
		{
			Log.dbg("OnVesselLoaded {0}", vessel.vesselName);
			this.SpawnnedVessels.Add(vessel);
		}

		// Destroyed vessels do not need to be updated, not to mention that keeping them
		// on this list unecessarily will postpone the garbage colleting.
		private void OnVesselDestroy(Vessel vessel)
		{
			lock (this.SpawnnedVessels) // Prevents race conditions in case we have concurrency on KSP on spawning vessels...
			{
				if (this.SpawnnedVessels.Contains(vessel))
					this.SpawnnedVessels.Remove(vessel);
			}
		}

		private void OnVesselWasModified(Vessel vessel)
		{
			Log.dbg("OnVesselWasModified {0}", vessel.vesselName);
			this.DelayedUpdate(vessel);
		}

		// The KSP API (https://kerbalspaceprogram.com/api/class_game_events.htm) says
		// that OnVesselWasModified can be missed sometimes, so...
		// Alert: This is called when the vessel spawns!!
		private void OnVesselPartCountChanged(Vessel vessel)
		{
			Log.dbg("OnVesselPartCountChanged {0}", vessel.vesselName);
			this.DelayedUpdate(vessel);
		}

		// We have been billed! Now we can initialise the Refunding on the vessels!
		private void OnFundsChanged(double amount, TransactionReasons reason)
		{
			Log.dbg("OnFundsChanged {0} due {1}", amount, reason);
			if (!TransactionReasons.VesselRollout.Equals(reason)) return;

			this.ImmediateUpdateAllAndClear();
		}

		private void OnVesselRecoveryRequested(Vessel vessel)
		{
			Log.dbg("OnVesselRecoveryRequested {0}", vessel.vesselName);
			this.ImmediateUpdate(vessel);
		}

		// This is called when the vessel is switched IN, including on launch
		// So this is the point where we trigger a (delayed) update of the
		// Refunding Resource.
		// Ideally we would not need this event, as it will be called on every
		// vessel switch on the current scene. However, OnNewVesselCreated is
		// not called when we spawn the Vessel from the Editor, so I still need
		// to handle this event somehow.
		private void OnVesselChange(Vessel vessel)
		{
			Log.dbg("OnVesselChange {0}", vessel.vesselName);

			this.DelayedUpdate(vessel);
		}

		// Probably not needed, here for testing purposes
		private void OnVesselSwitching(Vessel from, Vessel to)
		{
			Log.dbg("OnVesselSwitching from {0} to {1}", from.vesselName, to.vesselName);
			this.ImmediateUpdate(from);
		}

		// That's the history: I'm trying to force my hand on Parts with ModuleCargoPart that are stackable.
		// The presence of the Refunding Resource make these parts unstackable, as stackable parts must
		// be stateless (and resources add state to the part).
		//
		// So, while going on rails, we add the Refunding Resource so if the vessel is recovered while packed,
		// the costs are correctly recovered.
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
			this.RemoveResourceWhenNeeded(vessel);
		}

		#endregion

		private void ImmediateUpdateAllAndClear()
		{
			Log.dbg("ImmediateUpdateAllAndClear");

			Vessel[] a;
			lock (this.SpawnnedVessels) // Prevents race conditions in case we have concurrency on KSP on spawning vessels...
			{
				a = new Vessel[this.SpawnnedVessels.Count];
				this.SpawnnedVessels.CopyTo(a);
				this.SpawnnedVessels.Clear();
			}

			foreach (Vessel v in a) if (Vessel.State.ACTIVE == v.state)	// Better safe than sorry.
				this.ImmediateUpdate(v);
		}

		private void ImmediateUpdate(Vessel vessel)
		{
			Log.dbg("ImmediateUpdate {0}", vessel.vesselName);

			foreach (Part p in vessel.Parts)
			{
				if (p.Modules.Contains<FundsKeeper>())		p.Modules.GetModule<FundsKeeper>().SynchronousFullUpdate();
				if (p.Modules.Contains<Refunding>())		p.Modules.GetModule<Refunding>().SynchronousFullUpdate();
				if (p.Modules.Contains<StealBackMyFunds>())	p.Modules.GetModule<StealBackMyFunds>().SynchronousFullUpdate();
			}
		}

		// The delay allows the vessel's cost to be billed before the Refund is calculated
		// Preventing overbilling on launch.
		private void DelayedUpdate(Vessel vessel)
		{
			Log.dbg("DelayedUpdate {0}", vessel.vesselName);

			foreach (Part p in vessel.Parts)
			{
				if (p.Modules.Contains<FundsKeeper>())		p.Modules.GetModule<FundsKeeper>().AsynchronousUpdate(5);
				if (p.Modules.Contains<Refunding>())		p.Modules.GetModule<Refunding>().AsynchronousUpdate(10);
				if (p.Modules.Contains<StealBackMyFunds>())	p.Modules.GetModule<StealBackMyFunds>().AsynchronousUpdate(15);
			}
		}

		private void RemoveResourceWhenNeeded(Vessel vessel)
		{
			Log.dbg("RemoveResourceWhenNeeded {0}", vessel.vesselName);

			foreach (Part p in vessel.Parts)
			{
				if (p.Modules.Contains<Refunding>())		p.Modules.GetModule<Refunding>().RemoveResourceWhenNeeded();
				if (p.Modules.Contains<StealBackMyFunds>())	p.Modules.GetModule<StealBackMyFunds>().RemoveResourceWhenNeeded();
			}
		}

		private static KSPe.Util.Log.Logger Log = KSPe.Util.Log.Logger.CreateForType<FlightHelper>("KSP-Recall", "Refunding.FlightHelper", 0);
	}
}
