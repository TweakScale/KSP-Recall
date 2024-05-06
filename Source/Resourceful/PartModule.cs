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

namespace KSP_Recall { namespace Resourcefull 
{
	public class Resourceful : PartModule
	{
		private const bool visibleOnFlight = false;
		private const bool visibleOnEditor = true;

		#region KSP UI

		[KSPField(isPersistant = true, guiActive = visibleOnFlight, guiActiveEditor = visibleOnEditor, guiName = "KSP-Recall::Resourceful")]
		[UI_Toggle(disabledText = "Disabled", enabledText = "Enabled", scene = UI_Scene.All)]
		public bool active = false;

		#endregion


		#region KSP Life Cycle

		public override void OnAwake()
		{
			Log.dbg("OnAwake {0}:{1:X}", this.name, this.part.GetInstanceID());
			base.OnAwake();
			this.active = Globals.Instance.Resourceful;
			if (this.active && Pool.RESOURCES.HasSomething(this.part)) this.RestoreResourceList();
		}

		public override void OnStart(StartState state)
		{
			Log.dbg("OnStart {0}:{1:X} {2} {3}", this.name, this.part.GetInstanceID(), state, this.active);
			base.OnStart(state);
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
			if (Pool.RESOURCES.HasSomething(fromModule.part))
			{
				Pool.RESOURCES.Copy(fromModule.part, this.part);
				this.RestoreResourceList();
			}
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

		#endregion

		#region Unity Life Cycle

		private void OnDestroy()
		{
			Log.dbg("OnDestroy {0}:{1:X}", this.name, this.part.GetInstanceID());
			Pool.RESOURCES.Destroy(this.part);
		}

		#endregion

		#region Part Events Handlers

		[Obsolete("Since I'm an idiot that struggles with the most basic English principles, I inadequately defined the event name on the first public release. Please use OnPartResourcesChanged instead.")]
		[KSPEvent(guiActive = false, active = true)]
		void OnPartResourceChanged(BaseEventDetails data)
		{
			int instanceId = data.Get<int>("InstanceID");
			if (this.part.GetInstanceID() != instanceId) return;

			Log.dbg("(Deprecated) OnPartResourceChanged for InstanceId {0:X}", instanceId);
			this.UpdateResourceList();
		}

		[KSPEvent(guiActive = false, active = true)]
		void OnPartResourcesChanged(BaseEventDetails data)
		{
			int instanceId = data.Get<int>("InstanceID");
			if (this.part.GetInstanceID() != instanceId) return;

			Type issuer = data.Get<Type>("issuer");
			Log.dbg("OnPartResourcesChanged for InstanceId {0:X}, issued by {1}", instanceId, issuer);
			this.UpdateResourceList();
		}
		#endregion

		private void UpdateResourceList()
		{
			Pool.RESOURCES.Update(this.part);
			Log.dbg("Updated {0} resources for {1}:{2:X}", Pool.RESOURCES.Count(this.part), this.name, this.part.GetInstanceID());
		}

		private void RestoreResourceList()
		{
			if (!this.active)
			{
				Log.dbg("Ignoring {0} resources for {1}:{2:X}", Pool.RESOURCES.Count(this.part), this.name, this.part.GetInstanceID());
				return;
			}
			Log.dbg("Restoring {0} resources for {1}:{2:X}", Pool.RESOURCES.Count(this.part), this.name, this.part.GetInstanceID());
			Pool.RESOURCES.Restore(this.part);
		}

		private static readonly KSPe.Util.Log.Logger Log = KSPe.Util.Log.Logger.CreateForType<Resourceful>("KSP-Recall", "Resourceful", 0);
	}
} }
