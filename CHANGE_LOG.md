# KSP-Recall :: Change Log

* 2024-0521: 0.5.0.2 (LisiasT) for KSP >= 1.4.1
	+ Fixes an unexpected misbehaviour on KSP >= 1.9.x that, frankly, I don't know how I had missed for so much time.
	+ Closes issues:
		- [#80](https://github.com/net-lisias-ksp/KSP-Recall/issues/80) Merge Craft is getting its surface attachments screwed on 1.9.x and newer 
* 2024-0519: 0.5.0.1 (LisiasT) for KSP >= 1.4.1
	+ ***WITHDRAWN***
		- Because sleepy developers should do more sleeping and less releases! :P
* 2024-0506: 0.5.0.0 (LisiasT) for KSP >= 1.4.1
	+ Rollbacks the PAW changes due an absolutely unexpected, unexplainable and crappy KSP behaviour that only manifested itself on KSP 1.12.x
		- I don't have the slightest clue about what happened yet. 
	+ Closes issues:
		- [#77](https://github.com/net-lisias-ksp/KSP-Recall/issues/77) 
Refunding is not working on 1.12.x!!!
* 2024-0407: 0.4.1.0 (LisiasT) for KSP >= 1.4.1
	+ Reworks `AttachedOnEditor` to be resilient a changes made by Monkey Patching inside Editor that changes the order in which some internal Editor events happen.
	+ Closes issues:
		- [#73](https://github.com/net-lisias-ksp/KSP-Recall/issues/74) Understand why `BetterEditorUndoRedo` ended up screwing `AttachedOnEditor` indirectly 
* 2023-0331: 0.4.0.5 (LisiasT) for KSP >= 1.4.1
	+ I overreacted while restoring the PAW's entries, and ended up allowing some entries to be shown when they shouldn't. Fixing it.
	+ Properly implemented what I intended to be the `DebugMode` at first place.
	+ Closes issues:
		- [#76](https://github.com/net-lisias-ksp/KSP-Recall/issues/76) Implement the PAW's DebugMode properly
		- [#75](https://github.com/net-lisias-ksp/KSP-Recall/issues/75) 0.4.0.4 is screwing PAW
* 2023-0328: 0.4.0.4 (LisiasT) for KSP >= 1.4.1
	+ Restores the presence of the PAW's entries, setting them to be shown by default from now on. A patch can turn them off. 
	+ Closes issues:
		- [#74](https://github.com/net-lisias-ksp/KSP-Recall/issues/74) Restore the KSP-Recall buttons on the PAW
* 2023-0323: 0.4.0.3 (LisiasT) for KSP >= 1.4.1
	+ Closes issues:
		- [#72](https://github.com/net-lisias-ksp/KSP-Recall/issues/72) Fix the Sanity Check report
		- [#68](https://github.com/net-lisias-ksp/KSP-Recall/issues/68) Add an option to to not show the KSP-Recall entries on the PAW
* 2023-0716: 0.4.0.2 (LisiasT) for KSP >= 1.4.1
	+ Implements a missing use case from TweakScale, `OnPartAttachmentNodesChanged`.
	+ Updates MMWD to 1.1.1.1
* 2023-0408: 0.4.0.1 (LisiasT) for KSP >= 1.4.1
	+ Fixes a nasty mishap of mine that gone undetected due another yet more nasty mishap of Module Manager (Forum).
	+ Closes issues:
		- [#65](https://github.com/net-lisias-ksp/KSP-Recall/issues/65) Yet a new batch of displacement problems on KSP
	+ Relates to Discussion:
		- [#64](https://github.com/net-lisias-ksp/KSP-Recall/issues/65) Yet a new batch of displacement problems on KSP
* 2023-0325: 0.4.0.0 (LisiasT) for KSP >= 1.4.1
	+ Reworks `Refunding` (yet again), splitting the `PartModule` into two, the second one (`StealBackMyFunds`) dedicated to the Funds on `float` problem.
	+ Reworks the `AttachedOnEditor`, fixing an annoying "gap" when merging crafts (that didn't happens on SubAssemblies!)
	+ Updates Module Manager Watch Dog to the latest.
	+ Closes issues:
		- [#62](https://github.com/net-lisias-ksp/KSP-Recall/issues/62) Find a way to survive KSPCF's ~~Stupidity~~ *Less Than Smartness*
		- [#61](https://github.com/net-lisias-ksp/KSP-Recall/issues/61) AttachedOnEditor is being screwed up when Merging crafts
		- [#28](https://github.com/net-lisias-ksp/KSP-Recall/issues/28) Refresh the ModuleManagerWatchDog DLL
* 2023-0226: 0.3.0.12 (LisiasT) for KSP >= 1.4.1
	+ Reworks `Refunding` (again), this time with a better workaround for the really expensive parts, the preferential targets.
		- Kudos to [GoAhead](https://forum.kerbalspaceprogram.com/index.php?/profile/179049-goahead/) for the heads up! 
	+ Reworks issue:
		- [#60](https://github.com/net-lisias-ksp/KSP-Recall/issues/60) Weird Misbehaviour on Interstellar Technologies
* 2023-0225: 0.3.0.11 (LisiasT) for KSP >= 1.4.1
	+ Rework `Refunding` to use `decimal` when handling currency to minimize losses due inaccuracies related to float point math.
		- Kudos to [GoAhead](https://forum.kerbalspaceprogram.com/index.php?/profile/179049-goahead/) for the heads up! 
	+ Closes issue:
		- [#60](https://github.com/net-lisias-ksp/KSP-Recall/issues/60) Weird Misbehaviour on Interstellar Technologies
* 2023-0201: 0.3.0.10 (LisiasT) for KSP >= 1.4.1
	+ Shoves back `Resourceful` and `AttachedFromEditor` into B9PS's SAS after some carnage in the field
* 2023-0128: 0.3.0.9 (LisiasT) for KSP >= 1.4.1
	+ Preventing `Resourceful` and `AttachedFromEditor` from being patched into parts using B9PartSwitch, as they don't need them.
	+ Updates `KSPe.Light` with more bugfixes
* 2023-0115: 0.3.0.8 (LisiasT) for KSP >= 1.4.1
	+ Updates the thing to KSP 1.12.5
	+ Updates `KSPe.Light` with bugfixes and support for 1.12.5
* 2022-1126: 0.3.0.7 (LisiasT) for KSP >= 1.4.1
	+ Fixing a naïve approach on checking the `pwd`.
		- Thanks to Fellow Kerbonaut [TurtleMountain](https://forum.kerbalspaceprogram.com/index.php?/profile/223946-turtlemountain/) for the [report](https://forum.kerbalspaceprogram.com/index.php?/topic/192048-143/&do=findComment&comment=4202553) and help on the [diagnose](https://forum.kerbalspaceprogram.com/index.php?/topic/192048-143/&do=findComment&comment=4202776)!
		- Also thanks to [HebaruSan](https://forum.kerbalspaceprogram.com/index.php?/profile/156657-hebarusan/) for the brainstorming while investigating the matter
		- And my apologies to the CKAN guys for the [trouble](https://forum.kerbalspaceprogram.com/index.php?/topic/192048-143/&do=findComment&comment=4202777).
* 2022-1123: 0.3.0.6 (LisiasT) for KSP >= 1.4.1
	+ Better diagnosing messages.
* 2022-1123: 0.3.0.5 (LisiasT) for KSP >= 1.4.1
	+ Adds a check for the `pwd` problem that started to happen when people tried to get rid of the PD-Launcher in an unfortunate way. 
		- Check [this](https://forum.kerbalspaceprogram.com/index.php?/topic/210419-get-rid-of-the-stupid-launcher-nobody-likes-them-and-they-do-nothing-but-ruin-the-gaming-experience/) for the whole history.
		- Additional links (**do not** try them, they cause this problem!)
			- [Forum](https://forum.kerbalspaceprogram.com/index.php?/topic/210409-how-to-bypass-the-new-launcher-added-in-update-1124-steam/) 
			- [Reddit](https://www.reddit.com/r/KerbalSpaceProgram/comments/ym04gx/solution_for_broken_mods_caused_by_the_launcher/)
	+ Better diagnosing logs.
	+ Updates Module Manager Watch Dog to the latest (1.1.0.3 at this time)
	+ Updates to the latest KSPe.Light. (2.4.2.4 at this time)
* 2022-1122: 0.3.0.4 (LisiasT) for KSP >= 1.4.1
	+ ***DITCHED***
* 2022-0908: 0.3.0.3 (LisiasT) for KSP >= 1.4.1
	+ Updating the Sanity Check to prevent an annoying "Proceed with caution" warning.
* 2022-0904: 0.3.0.2 (LisiasT) for KSP >= 1.4.1
	+ After some weeks of denying, it was **finally** realised that `AttachedOnEditor` is needed downto KSP 1.4.3 - on the very first `ModulePartVariant` implementation.
		- What a crap of a code they published, damn!
 	+ Fixes a small brain fart of mine on the [INSTALL.md](https://github.com/net-lisias-ksp/KSP-Recall/blob/master/INSTALL.md) file.
	+ Updates KSPe.Light to the latest release (2.4.2.1 at this time)
	+ Closes issues:
		- [#55](https://github.com/net-lisias-ksp/KSP-Recall/issues/55) The KSP Editor is screwing things since 1.4.3
		- [#50](https://github.com/net-lisias-ksp/KSP-Recall/issues/50) Not sure how to install 0.3.0.0
			- Thanks to [@tomtheisen](https://github.com/tomtheisen) for the heads up!
* 2022-0630: 0.3.0.1 (LisiasT) for KSP >= 1.4.1
	+ Fixes a **huge** brain fart of mine from 0.2.2.4 #facePalm
	+ Closes issues:
		- [#52](https://github.com/net-lisias-ksp/KSP-Recall/issues/52) REPORT: Diagnosing the GoAHead Issue
		- [#49](https://github.com/net-lisias-ksp/KSP-Recall/issues/49) ArgumentOutOfRangeException causes saving to break and makes my save file unplayable
* 2022-0626: 0.3.0.0 (LisiasT) for KSP >= 1.4.1
	+ Prevents a race condition with Making History on Main Menu on heavily loaded rigs. 
	+ Updates ModuleManagerWatchDog to 1.1.0.1
	+ Updates KSPe.Light.Recall to 2.4.1.16 due the [Proton fix](https://github.com/net-lisias-ksp/KSPe/issues/31).
	+ Closes issues:
		- [#45](https://github.com/net-lisias-ksp/KSP-Recall/issues/45) Move the Sanity Checks) out of the Main Menu startup
* 2022-0514: 0.2.2.4 (LisiasT) for KSP >= 1.4.1
	+ Closes issues:
		- [#41](https://github.com/net-lisias-ksp/KSP-Recall/issues/41) Investigate a possible (bad) iteraction with Procedural Parts (RO) V2.3.0
		- [#40](https://github.com/net-lisias-ksp/KSP-Recall/issues/40) Unhappy interaction with Deep Freeze?
		- [#37](https://github.com/net-lisias-ksp/KSP-Recall/issues/37) Check about a missing use-case on AttachedOnEditor
		- [#31](https://github.com/net-lisias-ksp/KSP-Recall/issues/31) Update KSPe.Light for KSPe
* 2022-0308: 0.2.2.3 (LisiasT) for KSP >= 1.4.1
	+ Allows patching `AttachedOnEditor` on every compatible part, no matter it has `TweakScale` installed or not.
		- Needed because a TweakScaled part borks when attached to another without it. 
* 2022-0228: 0.2.2.2 (LisiasT) for KSP >= 1.4.1
	+ Reworks:
		- [#35](https://github.com/net-lisias-ksp/KSP-Recall/issues/35) AttachedOnEditor is not working for SubAssemblies.
* 2022-0219: 0.2.2.1 (LisiasT) for KSP >= 1.4.1
	+ Formally closes issues:
		- [#35](https://github.com/net-lisias-ksp/KSP-Recall/issues/35) AttachedOnEditor is not working for SubAssemblies.
		- [#34](https://github.com/net-lisias-ksp/KSP-Recall/issues/34) **NEW** Misbehaviour on KSP introduced by AttachedOnEditor
* 2022-0214: 0.2.2.0 (LisiasT) for KSP >= 1.4.1 PRE-RELEASE
	+ Implements another missing use case, handled on #34
	+ Works the issues:
		- [#34](https://github.com/net-lisias-ksp/KSP-Recall/issues/34) **NEW** Misbehaviour on KSP introduced by AttachedOnEditor
* 2022-0211: 0.2.1.4 (LisiasT) for KSP >= 1.4.1
	+ Implements a missing use case, handled on #34
	+ Works the issues:
		- [#34](https://github.com/net-lisias-ksp/KSP-Recall/issues/34) **NEW** Misbehaviour on KSP introduced by AttachedOnEditor
* 2021-1221: 0.2.1.3 (LisiasT) for KSP >= 1.4.1
	+ Implements a missing Use Case from Issue #32, handled on #34
	+ Lifts the ban for KSP >= 1.10, the thing is known to work on everything since KSP 1.9
	+ Works the issues:
		- [#34](https://github.com/net-lisias-ksp/KSP-Recall/issues/34) **NEW** Misbehaviour on KSP introduced by AttachedOnEditor
* 2021-1221: 0.2.1.2 (LisiasT) for KSP >= 1.4.1
	+ Rework of issue #32.
	+ Closes issues:
		- [#32](https://github.com/net-lisias-ksp/KSP-Recall/issues/32) **Correctly** handle KSP 1.9 (and later) borking while loading Crafts on Editor with scaled Variants
* 2021-1221: 0.2.1.1 (LisiasT) for KSP >= 1.4.1
	+ Locks up the current `AttachedOnEditor` as it is working **fine** on KSP 1.9.x
		- KSP 1.10.0 and newer, however, apparently need a different workaround, currently WiP. 
* 2021-1220: 0.2.1.0 (LisiasT) for KSP >= 1.4.1
	+ **Finally** tackles down the Editor's Surface Attachment problem introduced by KSP 1.9.0.
		- By default, locked to Parts with TweakScale only - but the patch can be easily extended if needed. 
	+ Closes issues:
		- [#32](https://github.com/net-lisias-ksp/KSP-Recall/issues/32) **Correctly** handle KSP 1.9 (and later) borking while loading Crafts on Editor with scaled Variants
* 2021-0907: 0.2.0.6 (LisiasT) for KSP >= 1.4.1
	+ Refreshing [ModuleManager WatchDog](https://github.com/net-lisias-ksp/ModuleManagerWatchDog) binaries to 1.0.1.1
	+ Bug fixes.
	+ Closes issues:
		- [#28](https://github.com/net-lisias-ksp/KSP-Recall/issues/28) Refresh the ModuleManagerWatchDog DLL
		- [#26](https://github.com/net-lisias-ksp/KSP-Recall/issues/26) Kerbal going on EVA on Kerbin without helmet instantly dies
		- [#25](https://github.com/net-lisias-ksp/KSP-Recall/issues/25) ChillingOut apparently is screwing up KSPIE
* 2021-0818: 0.2.0.5 (LisiasT) for KSP >= 1.4.1
	+ Updating KSPe.Light.Recall
	+ Minor fixes and/or optimisations.
	+ **NO NEW WORKAROUNDS OR FEATURES**, this is a maintenance release.
* 2021-0728: 0.2.0.4 (LisiasT) for KSP >= 1.4.1
	+ `ChillingOut` is not working as expected. Deactivating it.
		- Any problems will need to be tackled out punctually. 
* 2021-0722: 0.2.0.3 (LisiasT) for KSP >= 1.4.1
	+ Release 0.2.0.2 was issued with broken sanity checks. Fixing them.
* 2021-0718: 0.2.0.2 (LisiasT) for KSP >= 1.4.1
	+ `ChillingOut` is still needed on 1.12.x series. Reactivating it.
	+ Some missing logging stats added.
* 2021-0627: 0.2.0.1 (LisiasT) for KSP >= 1.4.1
	+ Compatibility to KSP 1.12.0 is confirmed.
		- `Refunding` is still needed, sadly...
	+ Closes issues:
		- [#23](https://github.com/net-lisias-ksp/KSP-Recall/issues/23) Refunding is triggering a nasty memory leak on this.part.Modules.Add
		- [#22](https://github.com/net-lisias-ksp/KSP-Recall/issues/22) Allow Refunding to be used on KSP 1.12.0
		- [#21](https://github.com/net-lisias-ksp/KSP-Recall/issues/21) Unity's spinlocks are bullying the Garbage Collector, and Refunding is not helping on the situation.
* 2021-0508: 0.2.0.0 (LisiasT) for KSP >= 1.4.1
	+ Bumping version due a [mishap](https://github.com/net-lisias-ksp/KSP-Recall/issues/17) on the latest release on SpaceDock
	+ Trying to workaround [MAS being picky](https://github.com/net-lisias-ksp/KSP-Recall/issues/18) even on hidden resources.
* 2021-0417: 0.1.0.8 (LisiasT) for KSP >= 1.4.1
	+ **Finally** diagnosed and fixed an issue on refunding Kerbals on Command Seats (or standalones)
	+ Reworks the issue [#16](https://github.com/net-lisias-ksp/KSP-Recall/issues/16).
* 2021-0416: 0.1.0.7 (LisiasT) for KSP >= 1.4.1
	* **Ditched**
* 2021-0413: 0.1.0.6 (LisiasT) for KSP >= 1.4.1
	+ Reworks the Work Around for issue [#16](https://github.com/net-lisias-ksp/KSP-Recall/issues/16), fixing the Stock's over-refunding on `ModuleInventoryPart`.
* 2021-0412: 0.1.0.5 (LisiasT) for KSP >= 1.4.1
	+ Fixes the workaround implemented on 0.1.0.4
* 2021-0411: 0.1.0.4 (LisiasT) for KSP >= 1.4.1
	+ Implements a Work Around for issue [#16](https://github.com/net-lisias-ksp/KSP-Recall/issues/16).
		- O pulled this out from my hat, it's not known yet if this will work on every use case.
		- Further testings as work in progress. Use this with caution for now.
* 2021-0409: 0.1.0.3 (LisiasT) for KSP >= 1.4.1
	+ The problem fixed on 1.0.2 was masking another problem on `Refunding` that, once fixed, regressed the over-billing problem.
		- GameEvents related to vessels don't work as I expected.
		- The solution was to step back a bit, and risking some over-refunding on FMRS on automatic recovery.
* 2021-0408: 0.1.0.2 (LisiasT) for KSP >= 1.4.1
	+ Pretty stupid mistake on `Refunding` fixed.
	+ Updating KSPe Light.
* 2021-0408: 0.1.0.1 (LisiasT) for KSP >= 1.4.1
	+ Minor revision to make life easier for Package Managers as CKAN.
		- Will allow installing on any KSP >= 1.4.1, even by not having (yet :P) any fix for them.
	+ Closes Issues:
		- [#14](https://github.com/net-lisias-ksp/KSP-Recall/issues/14) Make Recall safe to be installed on any KSP version instead of yelling about not being compatible  
* 2021-0404: 0.1.0.0 (LisiasT) for KSP >= 1.8.0
	+ More versatile (and user hackable) mechanism to activate/deactivate the Fixes (i.e: a way to override the safeties checks)
	+ Allowing the inactivation of the fixes to be persisted on the craft file and savegame, so you can deactivate a fix on some parts and keep them on others on a craft by craft basis
		- Will allow the user to safely keep playing until a new version with a fix/workaround implemented is not released when things goes south.
	+ Reenabling support for parts with `ModuleCargoPart`
		- Not all Stackable parts will not be refunded yet.
	+ More reliable and robust Game Event handling.
	+ Compatibility with resource changing Add'Ons (as fuel switches) enhanced.
* 2021-0327: 0.0.7.7 (LisiasT) for KSP >= 1.8.0
	+ Properly implemented the `active` attribute, allowing `Refunding` to be deactivate on a part per part basis if something wrong happens on the field.
	+ Reworked the `Refunding` mechanism, injecting the Resource at runtime as needed.
		- This will hopefully workaround the problem with Fuel Switches and other add'ons that mangle the Resource Pool of the part.
	+ Preventing `Resourceful` to be applied on parts with `ModuleCargoPart`, `ModuleComet`, `ModuleAsteroid` and `ModuleKerbal` as these are proven problematic on the field with code that handles resources. 
	+ The `activate` property of all Modules are now persisted on saving.
		- Be cautious, this can screw up your savegame. 
* 2021-0322: 0.0.7.6 (LisiasT) for KSP >= 1.8.0
	+ Made the `Refunding` stunt invisible on the U.I., decluttering the widgets.
		- Thanks for the [tip](https://forum.kerbalspaceprogram.com/index.php?/topic/192048-18/&do=findComment&comment=3943384), [Hohmannson](https://forum.kerbalspaceprogram.com/index.php?/profile/202712-hohmannson/)
	+ Preventing `Refunding` from messing up Stock Stackable parts
		- At the expense of having its costs fixed on Recovering, however
		- Thanks for [Krazy1](https://forum.kerbalspaceprogram.com/index.php?/profile/203523-krazy1/) for reporting [this](https://forum.kerbalspaceprogram.com/index.php?/topic/192048-18/&do=findComment&comment=3943444).
	+ Preventing `Refunding` from messing up Stackable on KIS
		- No collateral effects expected on this one.
	+ Added a Sanity Check for `Refunding` on KSP-Recall Startup. 
* 2021-0308: 0.0.7.5 (LisiasT) for KSP >= 1.8.0
	+ Fixed a pretty lame mistake on initiating the `Refund` PartModule.
* 2021-0307: 0.0.7.4 (LisiasT) for KSP >= 1.8.0
	+ ***DITCHED*** due a lame mistake while fixing a lame mistake.
* 2021-0305: 0.0.7.3 (LisiasT) for KSP >= 1.8.0
	+ (Properly) Implements a ~~ugly hack~~, I mean, a workaround for the KSP 1.11.x bug on recovering funds described on Issue [#12](https://github.com/net-lisias-ksp/KSP-Recall/issues/12).
		- Thanks a lot to [firethorn6](https://forum.kerbalspaceprogram.com/index.php?/profile/210389-firethorn6/) and [DarthPointer](https://forum.kerbalspaceprogram.com/index.php?/profile/203932-darthpointer/) for reporting the problem and further help on diagnosing it and testing the solution!
	+ Fixes a deployment mishap for CurseForge
* 2021-0303: 0.0.7.2 **BETA** (LisiasT) for KSP >= 1.8.0
	+ Enhances further more that ~~ugly hack~~, I mean, a workaround for the KSP 1.11.x bug on recovering funds described on Issue [#12](https://github.com/net-lisias-ksp/KSP-Recall/issues/12).
		- Most, if not all, Add'Ons are expected to be supported directly or indirectly this time. 
	+ **Attention please**
		- **DO NOT** use this on "production". This thing may be unsafe, as I used some dirty tricks that can backfire later.
* 2021-0303: 0.0.7.1 **BETA** (LisiasT) for KSP >= 1.8.0
	+ **DITCHED** due a mishap on the craft charge on Launch 
* 2021-0302: 0.0.7.0 **BETA** (LisiasT) for KSP >= 1.8.0
	+ Implements a ~~ugly hack~~, I mean, a workaround for the KSP 1.11.x bug on recovering funds described on Issue [#12](https://github.com/net-lisias-ksp/KSP-Recall/issues/12).
	+ **Attention please**
		- **DO NOT** use this on "production". This thing may be unsafe, as I used some dirty tricks that can backfire later.
		- Not all add'ons are guaranteed to work yet, I need to study some affected add'ons in order to detect the most simple way to support them. 
* 2021-0209: 0.0.6.1 (LisiasT) for KSP >= 1.8.0
	+ Updating the KSPe.Light, with a fix on the installment check
	+ Some minor fixes on the stats of the Sanity Checks.
	+ Removing the Beta status, the `ChilliongOut` stunt appears to be working fine.
* 2021-0106: 0.0.6.0 **BETA** (LisiasT) for KSP >= 1.8.0
	+ Preliminary attempt to overcome the new bug on launching from KSP 1.11
	+ Check the following posts for more information:
		- [KAX](https://forum.kerbalspaceprogram.com/index.php?/topic/180268-131/page/9/&tab=comments#comment-3901075)
		- [Impossible Innovations](https://forum.kerbalspaceprogram.com/index.php?/topic/175694-131/&do=findComment&comment=3901072).
* 2020-1220: 0.0.5.0 (LisiasT) for KSP >= 1.8.0
	+ Preventing installing Driftless on KSP 1.11.
		- Check [Issue #10](https://github.com/net-lisias-ksp/KSP-Recall/issues/10)
* 2020-0827: 0.0.4.4 (LisiasT) for KSP >= 1.8.0
	+ Some tool (and I'm hunting this \*\*\*\*), with all its wisdom, decided to "help me" adding silently a configuration that automatically converts EoL between UNIX and Windows. And did the stunt on a DLL.
		+ This release fixes the DLL.
		+ Full history on [Forum](https://forum.kerbalspaceprogram.com/index.php?/topic/179030-ksp-141-tweakscale-under-lisias-management-24321-2020-00804/&do=findComment&comment=3845367). 
* 2020-0817: 0.0.4.3 (LisiasT) for KSP >= 1.8.0
	+ Fixes an annoying situation where Decouplers and Docking Ports with `Driftless` blocks fuel to engines above them on the stack. 
* 2020-0817: 0.0.4.2 (LisiasT) for KSP >= 1.8.0 PRE-RELEASE
	+ Fixes an annoying situation where Decouplers and Docking Ports with `Driftless` blocks fuel to engines above them on the stack. 
* 2020-0817: 0.0.4.1 (LisiasT) for KSP >= 1.8.0
	+ An error on handling parts without `RigidBody` was raining NREs on the KSP.log. Fixed.
	+ A slightly smarter handling of inactive and rigidbodyless parts may save a tiny little bit of CPU time.
	+ Specialised treatment for Kerbals on EVA, as it was realised that Kerbals drifts a lot more than crafts - by reasons still unknown at this moment.
* 2020-0815: 0.0.4.0 (LisiasT) for KSP >= 1.8.0
	+ Adds a Work Around for crafts drifting on the Heading at rest, even when without wheels attached.
		- There's another similar problem on the wheels themselves, KSP Recall are still working on this one
	+ **Way** smarter selective applying of the Modules when needed.  
* 2020-0714: 0.0.3.2 (LisiasT) for [1.9.0 <= KSP <= 1.9.1]
	+ Maintenance Release.
		- Better (and safer) deactivation code using info gathered from [TweakScale](https://github.com/net-lisias-ksp/TweakScale/issues/125).
		- Locking up the current features to work only on 1.9.x (as KSP 1.10 doesn't need them).
* 2020-0521: 0.0.3.1 (LisiasT) for [1.9.0 <= KSP <= 1.9.1]
	* Fixes the 1.9.x Editor glitch on cloning parts with Resources' amount changed.
* 2020-0518: 0.0.3.0 (LisiasT) for [1.9.0 <= KSP <= 1.9.1]
	* Adds Sanity Check to prevent misuse.
	* New Event `OnPartResourcesChanged`.
		* `OnPartResourceChanged` is deprecated, but still works.
		* Note the **S** on resources.
* 2020-0305: 0.0.2.3 (LisiasT) for [1.9.0 <= KSP <= 1.9.1] PRE-RELEASE
	+ Fixes Issue [#5](https://github.com/net-lisias-ksp/KSP-Recall/issues/5).
		- Thanks, [Vegetal](https://forum.kerbalspaceprogram.com/?app=core&module=members&controller=profile&id=147251), for the [heads up](https://forum.kerbalspaceprogram.com/index.php?/topic/192048-ksp-recall-0022-pre-release-2020-0304/&do=findComment&comment=3752047)! 
* 2020-0304: 0.0.2.2 (LisiasT) for [1.9.0 <= KSP <= 1.9.1] PRE-RELEASE
	+ Allowing Fuel Switches that eliminates all the Resources from a part to be protected
	+ Embedding KSPe.Light (avoiding an external dependency).
* 2020-0303: 0.0.2.1 (LisiasT) for [1.9.0 <= KSP <= 1.9.1] PRE-RELEASE
	+ ** DROPPED ** 
* 2020-0302: 0.0.2.0 (LisiasT) for KSP >= 1.4.1 PRE-RELEASE
	+ A not so initial Release for Evaluation, but really made right this time. Honest!
	+ Correctly (I think) implements the Issue [#1](https://github.com/net-lisias-ksp/KSP-Recall/issues/3) "KSP 1.9.x resets resources to prefab while cloning parts".
		- Listening to the new Event `OnPartResourceChanged`
	+ Fixes the Issue [#2](https://github.com/net-lisias-ksp/KSP-Recall/issues/2) "Resourceful is applying the last scaling of a part on every new part"
* 2020-0301: 0.0.1.1 (LisiasT) for KSP >= 1.4.1 PRE-RELEASE
	+ Initial Release for Evaluation made right this time.
* 2020-0301: 0.0.1.0 (LisiasT) for KSP >= 1.4.1 PRE-RELEASE
	+ Initial Release for Evaluation
	+ Currently compatible only with TweakScale
