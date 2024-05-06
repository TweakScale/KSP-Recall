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
using UnityEngine;

namespace KSP_Recall.GUI
{
	internal static class FatalAlertBox
	{
		private static readonly string MSG = @"KSP Recall detected an unrecoverable problem!

{0}";

		private static readonly string AMSG = @"close KSP and open KSP Recall's Support page, and ask for help";

		internal static void Show(string message)
		{
			KSPe.Common.Dialogs.ShowStopperAlertBox.Show(
				string.Format(MSG, message),
				AMSG,
				() => { Application.OpenURL("https://ksp.lisias.net/add-ons/KSP-Recall/Support"); Application.Quit(); }
			);
			Log.force("\"Houston, we have a problem!\" about \"{0}\" was displayed", message);
		}
	}
}
