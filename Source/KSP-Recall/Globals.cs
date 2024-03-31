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
using KSPe;

namespace KSP_Recall
{
	public class Globals
	{
		private static Globals INSTANCE = null;

		public static Globals Instance => INSTANCE ?? (INSTANCE = new Globals());

		public bool DebugMode => KSPe.Globals<Globals>.DebugMode;

		public readonly bool Attached;
		public readonly bool ChillingOut;
		public readonly bool Driftless;
		public readonly bool Resourceful;
		public readonly bool AttachedOnEditor;

		public readonly bool Refunding;
		public readonly bool StealBackMyFunds;
		public bool FundsKeeper => this.Refunding || this.StealBackMyFunds;

		public readonly bool ProceduralPartsAttachmentNodes;

		public readonly bool PawEntries;

		private Globals()
		{
			try
			{
				UrlDir.UrlConfig urlc = GameDatabase.Instance.GetConfigs("KSP-Recall")[0];
				{ 
					ConfigNodeWithSteroids cn = ConfigNodeWithSteroids.from(urlc.config);
					try					{ this.PawEntries = cn.GetValue<bool>("PawEntries"); }
					catch (Exception)	{ this.PawEntries = true; }
				}
				{
					ConfigNodeWithSteroids cn = ConfigNodeWithSteroids.from(urlc.config.GetNode("INSTALLED"));

					try					{ this.Attached = cn.GetValue<bool>("Attached"); }
					catch (Exception)	{ this.Attached = false; }

					try					{ this.ChillingOut = cn.GetValue<bool>("ChillingOut"); }
					catch (Exception)	{ this.ChillingOut = false; }

					try					{ this.Driftless = cn.GetValue<bool>("Driftless"); }
					catch (Exception)	{ this.Driftless = false; }

					try					{ this.Refunding = cn.GetValue<bool>("Refunding"); }
					catch (Exception)	{ this.Refunding = false; }

					try					{ this.Resourceful = cn.GetValue<bool>("Resourceful"); }
					catch (Exception)	{ this.Resourceful = false; }

					try					{ this.AttachedOnEditor = cn.GetValue<bool>("AttachedOnEditor"); }
					catch (Exception)	{ this.AttachedOnEditor = false; }

					try					{ this.StealBackMyFunds = cn.GetValue<bool>("StealBackMyFunds"); }
					catch (Exception)	{ this.StealBackMyFunds = false; }
				}
				{
					ConfigNodeWithSteroids cn = ConfigNodeWithSteroids.from(urlc.config.GetNode("INTERVENTIONS"));

					try					{ this.ProceduralPartsAttachmentNodes = cn.GetValue<bool>("ProceduralPartsAttachmentNodes"); }
					catch (Exception)	{ this.ProceduralPartsAttachmentNodes = false; }
				}
			}
			catch (Exception e)
			{
				this.Attached = false;
				this.ChillingOut = false;
				this.Driftless = false;
				this.Refunding = false;
				this.Resourceful = false;
				this.AttachedOnEditor = false;
				this.StealBackMyFunds = false;
				this.ProceduralPartsAttachmentNodes = false;
				Log.error(e, this);
			}
		}
	}
}
