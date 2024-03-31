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

namespace KSP_Recall { namespace Attached
{
	public class Attached : PartModule
	{
		private const bool visibleOnFlight = false;
		private const bool visibleOnEditor = true;

		#region KSP UI

		[KSPField(isPersistant = true, guiActive = visibleOnFlight, guiActiveEditor = visibleOnEditor, guiName = "KSP-Recall::Attached")]
		[UI_Toggle(disabledText = "Disabled", enabledText = "Enabled", scene = UI_Scene.Editor)]
		public bool active = false;

		#endregion


		#region KSP Life Cycle

		public override void OnAwake()
		{
			Log.dbg("OnAwake {0}:{1:X}", this.name, this.part.GetInstanceID());
			base.OnAwake();
			this.active = Globals.Instance.Attached;
			if (this.active && Pool.ATTACHMENTS.HasSomething(this.part)) this.RestoreList();
		}

		public override void OnStart(StartState state)
		{
			Log.dbg("OnStart {0}:{1:X} {2} {3}", this.name, this.part.GetInstanceID(), state, this.active);
			base.OnStart(state);
			{
				BaseField bf = this.Fields["active"];
				bf.guiActive = visibleOnFlight && Globals.Instance.PawEntries;
				bf.guiActiveEditor = visibleOnEditor && Globals.Instance.PawEntries;
			}
		}

		public override void OnCopy(PartModule fromModule)
		{
			Log.dbg("OnCopy {0}:{1:X} from {2:X}", this.name, this.part.GetInstanceID(), fromModule.part.GetInstanceID());
			base.OnCopy(fromModule);
			if (Pool.ATTACHMENTS.HasSomething(fromModule.part))
			{
				Pool.ATTACHMENTS.Copy(fromModule.part, this.part);
				this.RestoreList();
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
			Pool.ATTACHMENTS.Destroy(this.part);
		}

		#endregion

		#region Part Events Handlers]

		[Obsolete("Coding late night can bite you in the arse. Due a mishap of mine on TweakScale, I need to support this thing for some time. :(")]
		[KSPEvent(guiActive = false, active = true)]
		void NotifyAttachmentNodesChanged(BaseEventDetails data)
		{
			int instanceId = data.Get<int>("InstanceID");
			if (this.part.GetInstanceID() != instanceId) return;

			Log.dbg("(Deprecated) NotifyAttachmentNodesChanged for InstanceId {0:X}", instanceId);
			this.UpdateList();
		}

		[KSPEvent(guiActive = false, active = true)]
		void OnPartAttachmentNodesChanged(BaseEventDetails data)
		{
			int instanceId = data.Get<int>("InstanceID");
			if (this.part.GetInstanceID() != instanceId) return;

			Type issuer = data.Get<Type>("issuer");
			Log.dbg("OnPartAttachmentNodesChanged for InstanceId {0:X}, issued by {1}", instanceId, issuer);
			this.UpdateList();
		}
		#endregion

		private void UpdateList()
		{
			Pool.ATTACHMENTS.Update(this.part);
			Log.dbg("Updated {0} attach nodes for {1}:{2:X}", Pool.ATTACHMENTS.Count(this.part), this.name, this.part.GetInstanceID());
		}

		private void RestoreList()
		{
			if (!this.active)
			{
				Log.dbg("Ignoring {0} attach nodes for {1}:{2:X}", Pool.ATTACHMENTS.Count(this.part), this.name, this.part.GetInstanceID());
				return;
			}
			Log.dbg("Restoring {0} attach nodes for {1}:{2:X}", Pool.ATTACHMENTS.Count(this.part), this.name, this.part.GetInstanceID());
			Pool.ATTACHMENTS.Restore(this.part);
		}

		private static readonly KSPe.Util.Log.Logger Log = KSPe.Util.Log.Logger.CreateForType<Attached>("KSP-Recall", "Attached", 0);
	}
} }