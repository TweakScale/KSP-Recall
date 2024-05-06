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
using UnityEngine;

namespace KSP_Recall { namespace Driftless
{
	public class Driftless : PartModule
	{
		#region KSP UI

		[KSPField(isPersistant = true, guiActive = true, guiActiveEditor = false, guiName = "KSP-Recall::Driftless")]
		[UI_Toggle(disabledText = "Disabled", enabledText = "Enabled", scene = UI_Scene.Flight)]
		public bool active = false;

		#endregion


		#region KSP Life Cycle

		public override void OnAwake()
		{
			Log.dbg("OnAwake {0}:{1:X}", this.name, this.part.GetInstanceID());
			base.OnAwake();
			this.active = Globals.Instance.Driftless;
		}

		public override void OnStart(StartState state)
		{
			Log.dbg("OnStart {0}:{1:X} {2} {3}", this.name, this.part.GetInstanceID(), state, this.active);
			base.OnStart(state);
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

		private float deltaV = 0f;
		private Rigidbody rb = null;
		private void FixedUpdate()
		{
			if (null == this.rb) return;
			if (!HighLogic.LoadedSceneIsFlight || part.vessel.situation != Vessel.Situations.LANDED) return;
			if (this.rb.velocity.magnitude > DELTA) return;

			this.rb.AddTorque(0,0,0,ForceMode.Force);
		}

		private void OnDestroy()
		{
			Log.dbg("OnDestroy {0}:{1:X}", this.name, this.part.GetInstanceID());
		}

		#endregion


		private const float DELTA = 0.01f;	// 0.01 = 1cm. On my machine, the maximum spurious velocity on rest was ~8.5 mm/s, says KER.
											// I will probably need to make this adjustable somehow, this can be machine speed dependent...
		private void init()
		{
			this.rb = this.part.GetComponent<Rigidbody>();

			if(null == this.rb)
				Log.dbg("{0}:{1:X} has no RigidBody.", this.name, this.part.GetInstanceID());

			this.deltaV = DELTA;
			if(this.part.Modules.Contains("KerbalEVA"))
			{
				this.deltaV *= 20;		// Kerbals have *way* more spurious velocities!
				Log.dbg("{0}:{1:X} is a Kerbal on EVA. Multiplying the DELTA_V.", this.name, this.part.GetInstanceID());
			}
		}

		private void deinit()
		{
			this.rb = null;
		}

		private static readonly KSPe.Util.Log.Logger Log = KSPe.Util.Log.Logger.CreateForType<Driftless>("KSP-Recall", "Driftless", 0);
	}
} }