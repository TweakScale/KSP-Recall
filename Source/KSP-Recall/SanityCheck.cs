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
using System.Collections;

using KSPe.Annotations;

using UnityEngine;

namespace KSP_Recall
{
	[KSPAddon(KSPAddon.Startup.Instantly, true)]
	internal class SanityCheck : MonoBehaviour
	{
		private class FatalException : System.Exception
		{
			public FatalException(string message) : base(message) { }
		}

		private class NonFatalException : System.Exception
		{
			public NonFatalException(string message) : base(message) { }
		}

		private const string RESOURCEFUL_MODULE_NAME = "Resourceful";
		private const string DRIFTLESS_MODULE_NAME = "Driftless";
		private const string ATTACHED_MODULE_NAME = "Attached";
		private const string CHILLINGOUT_MODULE_NAME = "ChillingOut";
		private const string REFUNDING_MODULE_NAME = "RefundingForKSP111x";
		private const string ATTACHEDONEDITOR_MODULE_NAME = "AttachedOnEditor";

		internal static bool isConcluded = false;
		internal static int showstoppers_count = 0;

		[UsedImplicitly]
		private void Start()
		{
			GameEvents.onGameSceneSwitchRequested.Add(this.OnGameSceneSwitchRequested);

			try
			{
				// No need to do this check if KSPe is installed!
				if (!KSPe.Util.SystemTools.Assembly.Exists.ByName("KSPe"))
				{
					Log.detail("KSPe is not installed. Checking `pwd`.");
					this.checkPwd();
				}
				else
					Log.detail("KSPe is installed. Trusting it for checking the `pwd`.");
			}
			catch (NonFatalException e)
			{
				GUI.NonFatalAlertBox.Show(e.Message);
			}
			catch (FatalException e)
			{
				GUI.FatalAlertBox.Show(e.Message);
			}
		}

		private void checkPwd()
		{
			string origin = KSPe.IO.Path.Origin();
			string approot = KSPe.IO.Path.AppRoot();
			string pwd = KSPe.IO.Directory.GetCurrentDirectory();	// See https://github.com/net-lisias-ksp/KSPe/issues/37#issuecomment-1327960782
																	// for the reason why this is not the ideal solution!
			Log.detail("origin is  {0}", origin);
			Log.detail("approot is {0}", approot);
			Log.detail("pwd is     {0}", pwd);

			if (!pwd.EndsWith(KSPe.IO.Path.DirectorySeparatorStr))
				pwd += KSPe.IO.Path.DirectorySeparatorStr;
			if (!origin.Equals(approot))
			{
				Log.error("origin != Application Root! -- origin={0} ; AppRot={1}", origin, approot);
				throw new FatalException("Your 'Origin' doesn't match KSP's 'Application Root'!");
			}
			if (!pwd.Equals(approot))
			{
				Log.error("pwd != Application Root! -- pwd={0} ; AppRot={1}", pwd, approot);
				throw new NonFatalException("Your 'pwd' doesn't match KSP's 'Application Root'!");
			}
			if (!pwd.Equals(origin))
			{
				Log.error("pwd != origin! -- pwd={0} ; origin={1}", pwd, approot);
				throw new NonFatalException("Your 'pwd' doesn't match KSP's 'Origin'!");
			}
		}

		private void OnGameSceneSwitchRequested(GameEvents.FromToAction<GameScenes, GameScenes> data)
		{
			Log.detail("Switching scene from {0} to {1}.", data.from, data.to);
			GameEvents.onGameSceneSwitchRequested.Remove(this.OnGameSceneSwitchRequested);
			this.ExecuteSanityChecks();
		}

		private void ExecuteSanityChecks()
		{
			SanityCheck.isConcluded = false;
			Log.info("SanityCheck: Started");

			int total_count = 0;
			int parts_with_resourceful_count = 0;
			int parts_with_driftless_count = 0;
			int parts_with_attached_count = 0;
			int parts_with_chillingout_count = 0;
			int parts_with_refunding_count = 0;
			int parts_with_attachedoneditor_count = 0;

			foreach (AvailablePart p in PartLoader.LoadedPartsList)
			{
				Part prefab;
				{ 
					bool containsResourceful = false;
					bool containsDriftless = false;
					bool containsAttached = false;
					bool containsChillingOut = false;
					bool containsRefunding = false;
					bool containsAttachedOnEditor = false;

					prefab = p.partPrefab; // Reaching the prefab here in the case another Mod recreates it from zero. If such hypothecical mod recreates the whole part, we're doomed no matter what.
					try 
					{
						containsResourceful = prefab.Modules.Contains(RESOURCEFUL_MODULE_NAME);
						containsDriftless = prefab.Modules.Contains(DRIFTLESS_MODULE_NAME);
						containsAttached = prefab.Modules.Contains(ATTACHED_MODULE_NAME);
						containsChillingOut = prefab.Modules.Contains(CHILLINGOUT_MODULE_NAME);
						containsRefunding = prefab.Modules.Contains(REFUNDING_MODULE_NAME);
						containsAttachedOnEditor = prefab.Modules.Contains(ATTACHEDONEDITOR_MODULE_NAME);
						++total_count;
					}
					catch (Exception e)
					{
						Log.error("Exception on {0}.prefab.Modules.Contains: {1}", p.name, e.Message);
						Log.detail("{0}", prefab.Modules);
						continue;
					}

					try
					{
						string due = null;

						if (containsResourceful) if (null != (due = this.checkForResourceful(prefab)))
						{
							Log.info("Removing {0} support for {1} ({2}) due {3}.", RESOURCEFUL_MODULE_NAME, p.name, p.title, due);
							prefab.RemoveModule(prefab.Modules[RESOURCEFUL_MODULE_NAME]);
						}
						else ++parts_with_resourceful_count;

						if (containsDriftless) if (null != (due = this.checkForDriftless(prefab)))
						{
							Log.info("Removing {0} support for {1} ({2}) due {3}.", DRIFTLESS_MODULE_NAME, p.name, p.title, due);
							prefab.RemoveModule(prefab.Modules[DRIFTLESS_MODULE_NAME]);
						}
						else ++parts_with_driftless_count;

						if (containsAttached) if (null != (due = this.checkForAttached(prefab)))
						{
							Log.info("Removing {0} support for {1} ({2}) due {3}.", ATTACHED_MODULE_NAME, p.name, p.title, due);
							prefab.RemoveModule(prefab.Modules[ATTACHED_MODULE_NAME]);
						}
						else ++parts_with_attached_count;

						if (containsChillingOut) if (null != (due = this.checkForChillingOut(prefab)))
						{
							Log.info("Removing {0} support for {1} ({2}) due {3}.", CHILLINGOUT_MODULE_NAME, p.name, p.title, due);
							prefab.RemoveModule(prefab.Modules[CHILLINGOUT_MODULE_NAME]);
						}
						else ++parts_with_chillingout_count;

						if (containsRefunding) if (null != (due = this.checkForRefunding(prefab)))
						{
							Log.info("Removing {0} support for {1} ({2}) due {3}.", REFUNDING_MODULE_NAME, p.name, p.title, due);
							prefab.RemoveModule(prefab.Modules[REFUNDING_MODULE_NAME]);
						}
						else ++parts_with_refunding_count;

						if (containsAttachedOnEditor) if (null != (due = this.checkForAttachedOnEditor(prefab)))
						{
							Log.info("Removing {0} support for {1} ({2}) due {3}.", ATTACHEDONEDITOR_MODULE_NAME, p.name, p.title, due);
							prefab.RemoveModule(prefab.Modules[ATTACHEDONEDITOR_MODULE_NAME]);
						}
						else ++parts_with_attachedoneditor_count;
					}
					catch (Exception e)
					{
						++showstoppers_count;
						Log.error("part={0} ({1}) Exception on Sanity Checks: {2}", p.name, p.title, e);
					}
					// End of hack. Ugly, uh? :P
				}
#if DEBUG
				{
					Log.dbg("Found part named {0} ; title {1}:", p.name, p.title);
					foreach (PartModule m in prefab.Modules)
						Log.dbg("\tPart {0} has module {1}", p.name, m.moduleName);
				}
#endif
			}

			Log.info("SanityCheck Concluded : {0} parts found ; {1} parts using {2} ; {3} parts using {4} ; {5} parts using {6} ; {7} parts using {8} ; {9} parts using {10}, {11} parts using {12}, {13} show stoppers detected ."
				, total_count
				, parts_with_resourceful_count, RESOURCEFUL_MODULE_NAME
				, parts_with_driftless_count, DRIFTLESS_MODULE_NAME
				, parts_with_attached_count, ATTACHED_MODULE_NAME
				, parts_with_chillingout_count, CHILLINGOUT_MODULE_NAME
				, parts_with_refunding_count, REFUNDING_MODULE_NAME
				, parts_with_attachedoneditor_count, ATTACHEDONEDITOR_MODULE_NAME
				, showstoppers_count);
			SanityCheck.isConcluded = true;
		}

		private const string MSG_INSTALLATION_FORCED = "Installation of {0} forced on KSP-Recall.cfg. Proceed with caution!";

		private const string MSG_KSP_NO_SUPPORTED = "your KSP version doesn't support it.";
		private const string MSG_PART_DOES_NOT_NEED = "this part doesn't need it.";
		private const string MSG_PART_NOT_SUPPORTED = "this part is not supported.";

		private string checkForResourceful(Part p)
		{
			Log.dbg("Checking {0} Sanity for {1} at {2}", RESOURCEFUL_MODULE_NAME, p.name, p.partInfo.partUrl ?? "<NO URL>");

			if ( !(1 == KSPe.Util.KSP.Version.Current.MAJOR && 9 == KSPe.Util.KSP.Version.Current.MINOR) )
			{
				if (Globals.Instance.Resourceful) Log.warn(MSG_INSTALLATION_FORCED, RESOURCEFUL_MODULE_NAME);
				else return MSG_PART_DOES_NOT_NEED ;
			}

			// if (0 == p.Resources.Count) return MSG_PART_DOES_NOT_NEED; Some AddOn can add Resources later, so I commented it out
			return this.checkForCommonUnsupportedParts(p);
		}

		private string checkForDriftless(Part p)
		{
			Log.dbg("Checking {0} Sanity for {1} at {2}", DRIFTLESS_MODULE_NAME, p.name, p.partInfo.partUrl ?? "<NO URL>");

			if ( KSPe.Util.KSP.Version.Current < KSPe.Util.KSP.Version.FindByVersion(1,8,0)
				||
				KSPe.Util.KSP.Version.Current > KSPe.Util.KSP.Version.FindByVersion(1,11,0)
			)
			{
				if (Globals.Instance.Driftless) Log.warn(MSG_INSTALLATION_FORCED, DRIFTLESS_MODULE_NAME);
				else return MSG_PART_DOES_NOT_NEED ;
			}

			return null;
		}

		private string checkForAttached(Part p)
		{
			Log.dbg("Checking {0} Sanity for {1} at {2}", ATTACHED_MODULE_NAME, p.name, p.partInfo.partUrl ?? "<NO URL>");

			if ( KSPe.Util.KSP.Version.Current < KSPe.Util.KSP.Version.FindByVersion(1,8,0) )
			{
				if (Globals.Instance.Attached) Log.warn(MSG_INSTALLATION_FORCED, ATTACHED_MODULE_NAME);
				else return MSG_PART_DOES_NOT_NEED ;
			}

			return this.checkForCommonUnsupportedParts(p);
		}

		private string checkForChillingOut(Part p)
		{
			Log.dbg("Checking {0} Sanity for {1} at {2}", CHILLINGOUT_MODULE_NAME, p.name, p.partInfo.partUrl ?? "<NO URL>");

			if ( KSPe.Util.KSP.Version.Current < KSPe.Util.KSP.Version.FindByVersion(1,11,0)
				|| KSPe.Util.KSP.Version.Current == KSPe.Util.KSP.Version.FindByVersion(1,11,1)
				|| KSPe.Util.KSP.Version.Current == KSPe.Util.KSP.Version.FindByVersion(1,11,2)
			)
			{
				if (Globals.Instance.ChillingOut) Log.warn(MSG_INSTALLATION_FORCED, CHILLINGOUT_MODULE_NAME);
				else return MSG_PART_DOES_NOT_NEED ;
			}

			return this.checkForCommonUnsupportedParts(p);
		}

		private string checkForRefunding(Part p)
		{
			Log.dbg("Checking {0} Sanity for {1} at {2}", REFUNDING_MODULE_NAME, p.name, p.partInfo.partUrl ?? "<NO URL>");

			if ( KSPe.Util.KSP.Version.Current < KSPe.Util.KSP.Version.FindByVersion(1,11,0) )
			{
				if (Globals.Instance.Refunding) Log.warn(MSG_INSTALLATION_FORCED, REFUNDING_MODULE_NAME);
				else return MSG_PART_DOES_NOT_NEED ;
			}

			if (p.Modules.Contains("ModuleCargoPart"))	return MSG_PART_NOT_SUPPORTED;

			return this.checkForCommonUnsupportedParts(p);
		}

		private string checkForAttachedOnEditor(Part p)
		{
			Log.dbg("Checking {0} Sanity for {1} at {2}", ATTACHEDONEDITOR_MODULE_NAME, p.name, p.partInfo.partUrl ?? "<NO URL>");

			if ( KSPe.Util.KSP.Version.Current < KSPe.Util.KSP.Version.FindByVersion(1,4,3) )
			{
				if (Globals.Instance.AttachedOnEditor) Log.warn(MSG_INSTALLATION_FORCED, ATTACHEDONEDITOR_MODULE_NAME);
				else return MSG_PART_DOES_NOT_NEED ;
			}

			return this.checkForCommonUnsupportedParts(p);
		}

		private string checkForCommonUnsupportedParts(Part p)
		{
			if (p.name.StartsWith("kerbalEVA"))			return MSG_PART_NOT_SUPPORTED;
			if (p.name.StartsWith("maleEVA"))			return MSG_PART_NOT_SUPPORTED;
			if (p.name.StartsWith("femaleEVA"))			return MSG_PART_NOT_SUPPORTED;
			if (p.Modules.Contains("KerbalEVA"))		return MSG_PART_NOT_SUPPORTED;
			if (p.Modules.Contains("ModuleAsteroid"))	return MSG_PART_NOT_SUPPORTED;
			if (p.Modules.Contains("ModuleComet"))		return MSG_PART_NOT_SUPPORTED;

			return null;
		}
	}

	[KSPAddon(KSPAddon.Startup.MainMenu, true)]
	internal class SanityCheckResults :MonoBehaviour
	{
		[UsedImplicitly]
		private void Start()
		{
			if (SanityCheck.showstoppers_count > 0)
				GUI.ShowStopperAlertBox.Show(SanityCheck.showstoppers_count);
		}
	}
}
