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
	internal static class NonFatalAlertBox
	{
		private static readonly string MSG = @"THIS INSTALLATION IS NOT FULLY SUPPORTED!

{0}

It's better to close KSP and ask for help on Forum, as some Add'On Authors may not support you without this problem fixed.";
		internal static void Show(string message) {
			KSPe.Common.Dialogs.ErrorAlertBox.Show(
				string.Format(MSG, message)
			);
			Log.force("\"Houston, we have a problem!\" about \"{0}\" was displayed", message);
		}
	}
}