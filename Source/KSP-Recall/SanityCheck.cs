﻿/*
	This file is part of KSP-Recall
	(C) 2020-2021 Lisias T : http://lisias.net <support@lisias.net>

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

using UnityEngine;

namespace KSP_Recall
{
	[KSPAddon(KSPAddon.Startup.MainMenu, true)]
	internal class SanityCheck : MonoBehaviour
	{
		private static readonly int WAIT_ROUNDS = 120; // @60fps, would render 2 secs.

		private const string RESOURCEFUL_MODULE_NAME = "Resourceful";
		private const string DRIFTLESS_MODULE_NAME = "Driftless";
		private const string ATTACHED_MODULE_NAME = "Attached";
		private const string CHILLINGOUT_MODULE_NAME = "ChillingOut";
		private const string REFUNDING_MODULE_NAME = "RefundingForKSP111x";
		private const string STAYINGTOGETHER_MODULE_NAME = "LetsStayTogether";

		internal static bool isConcluded = false;

		private void Start()
		{
			StartCoroutine("SanityCheckCoroutine");
		}

		private IEnumerator SanityCheckCoroutine()
		{
			SanityCheck.isConcluded = false;
			Log.info("SanityCheck: Started");

			{  // Toe Stomping Fest prevention
				for (int i = WAIT_ROUNDS; i >= 0 && null == PartLoader.LoadedPartsList; --i)
				{
					yield return null;
					if (0 == i) Log.warn("Timeout waiting for PartLoader.LoadedPartsList!!");
				}
	
				 // I Don't know if this is needed, but since I don't know that this is not needed,
				 // I choose to be safe than sorry!
				{
					int last_count = int.MinValue;
					for (int i = WAIT_ROUNDS; i >= 0; --i)
					{
						if (last_count == PartLoader.LoadedPartsList.Count) break;
						last_count = PartLoader.LoadedPartsList.Count;
						yield return null;
						if (0 == i) Log.warn("Timeout waiting for PartLoader.LoadedPartsList.Count!!");
					}
				}
			}

			int total_count = 0;
			int parts_with_resourceful_count = 0;
			int parts_with_driftless_count = 0;
			int parts_with_attached_count = 0;
			int parts_with_chillingout_count = 0;
			int parts_with_refunding_count = 0;
			int parts_with_stayingtogether_count = 0;
			int showstoppers_count = 0;

			foreach (AvailablePart p in PartLoader.LoadedPartsList)
			{
				for (int i = WAIT_ROUNDS; i >= 0 && (null == p.partPrefab || null == p.partPrefab.Modules); --i)
				{
					yield return null;
					if (0 == i) Log.error("Timeout waiting for {0}.prefab.Modules!!", p.name);
				}
			  
				Part prefab;
				{ 
					// Historically, we had problems here.
					// However, that co-routine stunt appears to have solved it.
					// But we will keep this as a ghinea-pig in the case the problem happens again.
					int retries = WAIT_ROUNDS;
					bool containsResourceful = false;
					bool containsDriftless = false;
					bool containsAttached = false;
					bool containsChillingOut = false;
					bool containsRefunding = false;
					bool containsStayingTogether = false;
					Exception culprit = null;
					
					prefab = p.partPrefab; // Reaching the prefab here in the case another Mod recreates it from zero. If such hypothecical mod recreates the whole part, we're doomed no matter what.
					
					while (retries > 0)
					{ 
						bool should_yield = false;
						try 
						{
							containsResourceful = prefab.Modules.Contains(RESOURCEFUL_MODULE_NAME);
							containsDriftless = prefab.Modules.Contains(DRIFTLESS_MODULE_NAME);
							containsAttached = prefab.Modules.Contains(ATTACHED_MODULE_NAME);
							containsChillingOut = prefab.Modules.Contains(CHILLINGOUT_MODULE_NAME);
							containsRefunding = prefab.Modules.Contains(REFUNDING_MODULE_NAME);
							containsStayingTogether = prefab.Modules.Contains(STAYINGTOGETHER_MODULE_NAME);
							++total_count;
							break;  // Yeah. This while stunt was done just to be able to do this. All the rest is plain clutter! :D 
						}
						catch (Exception e)
						{
							culprit = e;
							--retries;
							should_yield = true;
						}
						if (should_yield) // This stunt is needed as we can't yield from inside a try-catch!
							yield return null;
					}

					if (0 == retries)
					{
						Log.error("Exception on {0}.prefab.Modules.Contains: {1}", p.name, culprit);
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

						if (containsStayingTogether) if (null != (due = this.checkForStayingTogether(prefab)))
						{
							Log.info("Removing {0} support for {1} ({2}) due {3}.", STAYINGTOGETHER_MODULE_NAME, p.name, p.title, due);
							prefab.RemoveModule(prefab.Modules[STAYINGTOGETHER_MODULE_NAME]);
						}
						else ++parts_with_stayingtogether_count;
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

			Log.info("SanityCheck Concluded : {0} parts found ; {1} parts using {2} ; {3} parts using {4} ; {5} parts using {6} ; {7} parts using {8} ; {9} parts using {10} ; {11} parts using {12} ; {13} show stoppers detected ."
				, total_count
				, parts_with_resourceful_count, RESOURCEFUL_MODULE_NAME
				, parts_with_driftless_count, DRIFTLESS_MODULE_NAME
				, parts_with_attached_count, ATTACHED_MODULE_NAME
				, parts_with_chillingout_count, CHILLINGOUT_MODULE_NAME
				, parts_with_refunding_count, REFUNDING_MODULE_NAME
				, parts_with_stayingtogether_count, STAYINGTOGETHER_MODULE_NAME
				, showstoppers_count);
			SanityCheck.isConcluded = true;

			if (showstoppers_count > 0)
			{
				GUI.ShowStopperAlertBox.Show(showstoppers_count);
			}
		}

		private const string MSG_INSTALLATION_FORCED = "Installation of {0} forced on KSP-Recall.cfg. Proceed with caution!";

		private const string MSG_KSP_NO_SUPPORTED = "your KSP version doesn't need it.";
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

			return null;
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

			return null;
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

		private string checkForStayingTogether(Part p)
		{
			Log.dbg("Checking {0} Sanity for {1} at {2}", STAYINGTOGETHER_MODULE_NAME, p.name, p.partInfo.partUrl ?? "<NO URL>");

			if ( KSPe.Util.KSP.Version.Current < KSPe.Util.KSP.Version.FindByVersion(1,12,0) )
			{
				if (Globals.Instance.Refunding) Log.warn(MSG_INSTALLATION_FORCED, STAYINGTOGETHER_MODULE_NAME);
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
}
