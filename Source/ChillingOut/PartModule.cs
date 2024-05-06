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
using UnityEngine;

namespace KSP_Recall { namespace ChillingOut
{
	public class ChillingOut : PartModule
	{
		private const bool visibleOnFlight = true;
		private const bool visibleOnEditor = false;

		#region KSP UI

		[KSPField(isPersistant = true, guiActive = visibleOnFlight, guiActiveEditor = visibleOnEditor, guiName = "KSP-Recall::ChillingOut")]
		[UI_Toggle(disabledText = "Disabled", enabledText = "Enabled", scene = UI_Scene.All)]
		public bool active = false;

		#endregion

		[KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
		private long spawnTime = -1;

		#region KSP Life Cycle

		public override void OnAwake()
		{
			Log.dbg("OnAwake {0}:{1:X}", this.name, this.part.GetInstanceID());
			base.OnAwake();
			this.active = Globals.Instance.ChillingOut;
		}

		public override void OnStart(StartState state)
		{
			Log.dbg("OnStart {0}:{1:X} {2} {3}", this.name, this.part.GetInstanceID(), state, this.active);
			base.OnStart(state);
			if (HighLogic.LoadedSceneIsFlight && this.spawnTime < 0)
			{
				this.spawnTime = System.DateTime.Now.Ticks;
				Log.dbg("Spawned at {0}", this.spawnTime);
			}
			{
				BaseField bf = this.Fields["active"];
				bf.guiActive = Globals.Instance.DebugMode || (visibleOnFlight && Globals.Instance.PawEntries);
				bf.guiActiveEditor = Globals.Instance.DebugMode || (visibleOnEditor && Globals.Instance.PawEntries);
			}
		}

		public override void OnCopy(PartModule fromModule)
		{
			Log.dbg("OnCopy {0}:{1:X} from {2:X}", this.name, this.part.GetInstanceID(), fromModule.part.GetInstanceID());
			base.OnCopy(fromModule);
		}

		public override void OnLoad(ConfigNode node)
		{
			Log.dbg("OnLoad {0}:{1:X} {2}", this.name, this.part.GetInstanceID(), null != node);
			base.OnLoad(node);
		}

		public override void OnSave(ConfigNode node)
		{
			Log.dbg("OnSave {0}:{1:X} {2}", this.name, this.part.GetInstanceID(), null != node);
			base.OnSave(node);
		}

		public override void OnInitialize()
		{
			Log.dbg("OnInitialize {0}:{1:X}", this.name, this.part.GetInstanceID());
			base.OnInitialize();
			this.init();
		}

		public override void OnActive()
		{
			Log.dbg("OnActive {0}:{1:X}", this.name, this.part.GetInstanceID());
			base.OnActive();
			this.init();
		}

		// Needed because I had overriden OnActive.
		// See https://kerbalspaceprogram.com/api/class_part_module.html#a6f2dd76038326c527e64d2ce96bb45fe
		public override bool IsStageable()
		{
			return false;
		}

		public override void OnInactive()
		{
			Log.dbg("OnInactive {0}:{1:X}", this.name, this.part.GetInstanceID());
			base.OnInactive();
			this.deinit();
		}

		#endregion


		#region Unity Life Cycle

		private void FixedUpdate()
		{
			if (!HighLogic.LoadedSceneIsFlight) return;
			if (this.spawnTime < 0) return;
			TimeSpan ts = new TimeSpan(DateTime.Now.Ticks - this.spawnTime);
			if (ts.TotalMilliseconds > DELTA) this.enabled = false; // We are not needed anymore

			double t = this.GetTemperature();
			this.part.temperature = 
				this.part.skinTemperature =
				this.part.skinUnexposedTemperature =
				this.part.skinUnexposedExternalTemp =
					t;
			Log.dbg("FixedUpdated for {0}:{1:X}", this.name, this.part.GetInstanceID());
		}

		private void OnDestroy()
		{
			Log.dbg("OnDestroy {0}:{1:X}", this.name, this.part.GetInstanceID());
		}

		#endregion


		private readonly long DELTA = 1000 ;	// 1 second
		private void init()
		{
		}

		private void deinit()
		{
		}

		public double GetTemperature()
		{
			Vector3d position = this.part.vessel.GetWorldPos3D(); // I hope this is right?
			CelestialBody body = this.part.vessel.mainBody; // I hope this is right?

			// Brute force, half baked, local temperature calculation.
			double zeroAltitude = body.position.magnitude - body.Radius;
			double currentAltitude = position.magnitude - zeroAltitude;

			if (body.atmosphere && (currentAltitude < body.atmosphereDepth))
				return body.GetTemperature(currentAltitude);
			return PhysicsGlobals.SpaceTemperature;
		}

		private static readonly KSPe.Util.Log.Logger Log = KSPe.Util.Log.Logger.CreateForType<ChillingOut>("KSP-Recall", "ChillingOut", 0);
	}
} }