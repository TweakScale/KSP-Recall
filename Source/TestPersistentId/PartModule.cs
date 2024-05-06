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
using System;

namespace KSP_Recall.Test.PersistentId 
{
	public class TestPersistentId : PartModule
	{
		#region KSP UI

		#endregion


		#region KSP Life Cycle

		public override void OnInitialize()
		{
			Log.dbg("OnInitialize {0}:{1:X}", this.name, this.part.GetInstanceID());
			base.OnAwake();
			this.doLog("OnInitialize");
		}

		public override void OnAwake()
		{
			Log.dbg("OnAwake {0}:{1:X}", this.name, this.part.GetInstanceID());
			base.OnAwake();
			this.doLog("OnAwake");
		}

		public override void OnStart(StartState state)
		{
			Log.dbg("OnStart {0}:{1:X} {2}", this.name, this.part.GetInstanceID(), state);
			base.OnStart(state);
			this.doLog("OnStart");
		}

		public override void OnCopy(PartModule fromModule)
		{
			Log.dbg("OnCopy {0}:{1:X} from {2:X}", this.name, this.part.GetInstanceID(), fromModule.part.GetInstanceID());
			base.OnCopy(fromModule);
			this.doLog("OnCopy");
		}

		public override void OnLoad(ConfigNode node)
		{
			Log.dbg("OnLoad {0}:{1:X} {2}", this.name, this.part.GetInstanceID(), null != node);
			base.OnLoad(node);
			this.doLog("OnLoad");
		}

		public override void OnSave(ConfigNode node)
		{
			Log.dbg("OnSave {0}:{1:X} {2}", this.name, this.part.GetInstanceID(), null != node);
			base.OnSave(node);
			this.doLog("OnSave");
		}

		#endregion

		#region Unity Life Cycle

		private void OnDestroy()
		{
			Log.dbg("OnDestroy {0}:{1:X}", this.name, this.part.GetInstanceID());
			this.doLog("OnDestroy");
		}

		#endregion

		private void doLog(string situation)
		{
			Log.info("Part {0}:{1:X} has persistentId={2} at {3}. The ClassID is {4} and the CraftId (cid) is {5}.", this.name, this.part.GetInstanceID(), this.part.persistentId, situation, this.part.ClassID, this.part.craftID);
		}

		private static readonly KSPe.Util.Log.Logger Log = KSPe.Util.Log.Logger.CreateForType<TestPersistentId>("KSP-Recall", "Test.PersistentId", 0);
	}
}
