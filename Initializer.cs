using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.Globalization;
using ICities;
using MetroOverhaul.InitializationSteps;
using UnityEngine;
using MetroOverhaul.NEXT;
using MetroOverhaul.NEXT.Extensions;
using MetroOverhaul.OptionsFramework;

namespace MetroOverhaul
{
    public class Initializer : AbstractInitializer
    {
        public AppMode AppMode { get; set; }

        protected override void InitializeImpl()
        {
            CreateConcreteTracks();
            CreateSteelTracks();
            if (AppMode != AppMode.AssetEditor)
            {
                AssetsUpdater.PreventVanillaMetroTrainSpawning();
                //AssetsUpdater.UpdateVanillaMetroTracks();
            }
        }

        #region CONCRETE
        private void CreateConcreteTracks()
        {
            var elevatedInfo = FindOriginalNetInfo("Basic Road Elevated");
            var trainTrackInfo = FindOriginalNetInfo("Train Track");
            var metroInfo = FindOriginalNetInfo("Metro Track");
            var metroStationInfo = FindOriginalNetInfo("Metro Station Track");
            try
            {
                var replacements = new Dictionary<NetInfoVersion, string> { { NetInfoVersion.Tunnel, "Metro Track" } };
                CreateFullPrefab(
                    ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                        Chain(CustomizationSteps.SetupTrackProps).
                        Chain(CustomizationSteps.SetStandardTrackWidths).
                        Chain(CustomizationSteps.CommonCustomization).
                        Chain(CustomizationSteps.CommonConcreteCustomization).
                        Chain(CustomizationSteps.ReplaceTrackIcon).
                        Chain(SetupMesh.Setup10mMesh, elevatedInfo, metroInfo).
                        Chain(SetupMesh.Setup10mBarMesh, elevatedInfo).
                        Chain(SetupTexture.Setup10mTexture).
                        Chain(
                            (info, version) =>
                            {
                                LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUp.BuildUp(info, version); });
                            }),
                    NetInfoVersion.All, null, null, replacements
                    );
            }
            catch (Exception e)
            {
                Next.Debug.Log("Exception happened when setting up concrete tracks");
                UnityEngine.Debug.LogException(e);
            }

            try
            {
                CreateFullPrefab(
                    ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                        Chain(CustomizationSteps.SetupTrackProps).
                        Chain(CustomizationSteps.CommonCustomization).
                        Chain(CustomizationSteps.CommonConcreteCustomization).
                        Chain(CustomizationSteps.SetStandardTrackWidths).
                        Chain(SetupMesh.Setup10mMesh, elevatedInfo, metroInfo).
                        Chain(SetupTexture.Setup10mTexture).
                        Chain(
                            (info, version) =>
                            {
                                LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUp.BuildUp(info, version); });
                            }),
                    NetInfoVersion.Ground | NetInfoVersion.Elevated | NetInfoVersion.Bridge,
                    ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
                        Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(
                            LinkToNonGroundVersions, null,
                            NetInfoVersion.Slope | NetInfoVersion.Tunnel)
                    , prefabName => prefabName + " NoBar"
                    );
            }
            catch (Exception e)
            {
                Next.Debug.Log("Exception happened when setting up nobar concrete tracks");
                UnityEngine.Debug.LogException(e);
            }

            try
            {
                CreateFullPrefab(
                    ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                        Chain(CustomizationSteps.SetupTrackProps).
                        Chain(CustomizationSteps.CommonCustomizationTwoLaneOneWay).
                        Chain(CustomizationSteps.CommonConcreteCustomization).
                        Chain(CustomizationSteps.SetStandardTrackWidths).
                        Chain(SetupMesh.Setup10mMesh, elevatedInfo, metroInfo).
                        Chain(SetupMesh.Setup10mBarMesh, elevatedInfo).
                        Chain(SetupTexture.Setup10mTexture).
                        Chain(
                            (info, version) =>
                            {
                                LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUp.BuildUp(info, version); });
                            }),
                    NetInfoVersion.Ground | NetInfoVersion.Elevated | NetInfoVersion.Bridge,
                    ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
                        Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(
                            LinkToNonGroundVersions, null,
                            NetInfoVersion.Slope | NetInfoVersion.Tunnel)
                    , prefabName => prefabName + " Two-Lane One-Way"
                    );
            }
            catch (Exception e)
            {
                Next.Debug.Log("Exception happened when setting up nobar concrete tracks");
                UnityEngine.Debug.LogException(e);
            }

            try
            {
                CreateFullPrefab(
                    ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                        Chain(CustomizationSteps.SetupTrackProps).
                        Chain(CustomizationSteps.CommonCustomizationTwoLaneOneWay).
                        Chain(CustomizationSteps.CommonConcreteCustomization).
                        Chain(CustomizationSteps.SetStandardTrackWidths).
                        Chain(SetupMesh.Setup10mMesh, elevatedInfo, metroInfo).
                        Chain(SetupTexture.Setup10mTexture).
                        Chain(
                            (info, version) =>
                            {
                                LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUp.BuildUp(info, version); });
                            }),
                    NetInfoVersion.Ground | NetInfoVersion.Elevated | NetInfoVersion.Bridge | NetInfoVersion.Slope | NetInfoVersion.Tunnel,
                    ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
                        Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(
                            LinkToNonGroundVersions, null, NetInfoVersion.None)
                    , prefabName => prefabName + " Two-Lane One-Way NoBar"
                    );
            }
            catch (Exception e)
            {
                Next.Debug.Log("Exception happened when setting up nobar concrete tracks");
                UnityEngine.Debug.LogException(e);
            }
            try
            {
                CreateFullPrefab(
                    ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                        Chain(CustomizationSteps.SetupTrackProps).
                        Chain(CustomizationSteps.CommonCustomizationLarge).
                        Chain(CustomizationSteps.CommonConcreteCustomization).
                        Chain(CustomizationSteps.SetLargeTrackWidths).
                        Chain(SetupMesh.Setup10mMesh, elevatedInfo, metroInfo).
                        Chain(SetupMesh.Setup10mBarMesh, elevatedInfo).
                        Chain(SetupTexture.Setup10mTexture).
                        Chain(
                            (info, version) =>
                            {
                                LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUp.BuildUp(info, version); });
                            }),
                    NetInfoVersion.Ground | NetInfoVersion.Elevated | NetInfoVersion.Bridge | NetInfoVersion.Slope | NetInfoVersion.Tunnel,
                    ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
                        Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(
                            LinkToNonGroundVersions, null, NetInfoVersion.None)
                    , prefabName => prefabName + " Large"
                    );
            }
            catch (Exception e)
            {
                Next.Debug.Log("Exception happened when setting up nobar concrete tracks");
                UnityEngine.Debug.LogException(e);
            }

            try
            {
                CreateFullPrefab(
                    ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                        Chain(CustomizationSteps.SetupTrackProps).
                        Chain(CustomizationSteps.CommonCustomizationLarge).
                        Chain(CustomizationSteps.CommonConcreteCustomization).
                        Chain(CustomizationSteps.SetLargeTrackWidths).
                        Chain(SetupMesh.Setup10mMesh, elevatedInfo, metroInfo).
                        Chain(SetupTexture.Setup10mTexture).
                        Chain(
                            (info, version) =>
                            {
                                LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUp.BuildUp(info, version); });
                            }),
                    NetInfoVersion.Ground | NetInfoVersion.Elevated | NetInfoVersion.Bridge | NetInfoVersion.Slope | NetInfoVersion.Tunnel,
                    ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
                        Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(
                            LinkToNonGroundVersions, null, NetInfoVersion.None)
                    , prefabName => prefabName + " Large NoBar"
                    );
            }
            catch (Exception e)
            {
                Next.Debug.Log("Exception happened when setting up nobar concrete tracks");
                UnityEngine.Debug.LogException(e);
            }
            try
            {
                CreateFullPrefab(
                    ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                        Chain(CustomizationSteps.SetupTrackProps).
                        Chain(CustomizationSteps.CommonCustomizationSmall).
                        Chain(CustomizationSteps.CommonConcreteCustomization).
                        Chain(CustomizationSteps.SetSmallTrackWidths).
                        Chain(SetupMesh.Setup6mMesh).
                        Chain(SetupMesh.Setup6mMeshBar).
                        Chain(SetupTexture.Setup6mTexture).
                        Chain(
                            (info, version) =>
                            {
                                LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUp.BuildUp(info, version); });
                            }),
                   NetInfoVersion.All,
                    ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
                        Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(
                            LinkToNonGroundVersions, null, NetInfoVersion.None)
                    , prefabName => prefabName + " Small"
                    );
            }
            catch (Exception e)
            {
                Next.Debug.Log("Exception happened when setting up nobar concrete tracks");
                UnityEngine.Debug.LogException(e);
            }

            try
            {
                CreateFullPrefab(
                    ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                        Chain(CustomizationSteps.SetupTrackProps).
                        Chain(CustomizationSteps.CommonCustomizationSmall).
                        Chain(CustomizationSteps.CommonConcreteCustomization).
                        Chain(CustomizationSteps.SetSmallTrackWidths).
                        Chain(SetupMesh.Setup6mMesh).
                        Chain(SetupTexture.Setup6mTexture).
                        Chain(
                            (info, version) =>
                            {
                                LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUp.BuildUp(info, version); });
                            }),
                   NetInfoVersion.All,
                    ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
                        Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(
                            LinkToNonGroundVersions, null, NetInfoVersion.None)
                    , prefabName => prefabName + " Small NoBar"
                    );
            }
            catch (Exception e)
            {
                Next.Debug.Log("Exception happened when setting up nobar concrete tracks");
                UnityEngine.Debug.LogException(e);
            }

            try
            {
                CreateFullPrefab(
                    ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                        Chain(CustomizationSteps.SetupTrackProps).
                        Chain(CustomizationSteps.CommonCustomizationSmall).
                        Chain(CustomizationSteps.CommonConcreteCustomization).
                        Chain(CustomizationSteps.SetSmallTrackWidths).
                        Chain(SetupMesh.Setup6mMesh).
                        Chain(SetupMesh.Setup6mMeshBar).
                        Chain(SetupTexture.Setup6mTexture).
                        Chain(
                            (info, version) =>
                            {
                                LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUp.BuildUp(info, version); });
                            }),
                   NetInfoVersion.All,
                    ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
                        Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(
                            LinkToNonGroundVersions, null, NetInfoVersion.None)
                    , prefabName => prefabName + " Small Two-Way"
                    );
            }
            catch (Exception e)
            {
                Next.Debug.Log("Exception happened when setting up nobar concrete tracks");
                UnityEngine.Debug.LogException(e);
            }

            try
            {
                CreateFullPrefab(
                    ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                        Chain(CustomizationSteps.SetupTrackProps).
                        Chain(CustomizationSteps.CommonCustomizationSmall).
                        Chain(CustomizationSteps.CommonConcreteCustomization).
                        Chain(CustomizationSteps.SetSmallTrackWidths).
                        Chain(SetupMesh.Setup6mMesh).
                        Chain(SetupTexture.Setup6mTexture).
                        Chain(
                            (info, version) =>
                            {
                                LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUp.BuildUp(info, version); });
                            }),
                   NetInfoVersion.All,
                    ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
                        Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(
                            LinkToNonGroundVersions, null, NetInfoVersion.None)
                    , prefabName => prefabName + " Small Two-Way NoBar"
                    );
            }
            catch (Exception e)
            {
                Next.Debug.Log("Exception happened when setting up nobar concrete tracks");
                UnityEngine.Debug.LogException(e);
            }

            try
            {
                var replacements = new Dictionary<NetInfoVersion, string> { { NetInfoVersion.Tunnel, "Metro Station Track" } };
                CreateFullStationPrefab(
                    ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                        Chain(CustomizationSteps.SetupStationProps).
                        Chain(CustomizationSteps.CommonCustomization).
                        //Chain(CustomizationSteps.CommonConcreteCustomization).
                        Chain(SetupMesh.Setup10mStationMesh, elevatedInfo, metroStationInfo).
                        Chain(SetupTexture.Setup10mTexture).
                        Chain(
                            (info, version) =>
                            {
                                LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUp.BuildUp(info, version); });
                            }), false,
                   NetInfoVersion.Ground | NetInfoVersion.Elevated | NetInfoVersion.Tunnel,
                    ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
                        Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(
                            LinkToNonGroundVersions, null, NetInfoVersion.None)
                    , prefabName => prefabName, replacements
                    );
            }
            catch (Exception e)
            {
                Next.Debug.Log("Exception happened when setting up concrete station tracks");
                UnityEngine.Debug.LogException(e);
            }

            try
            {
                CreateFullStationPrefab(
                    ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                        Chain(CustomizationSteps.SetupStationProps).
                        Chain(CustomizationSteps.CommonIsland16mCustomization).
                        Chain(CustomizationSteps.Set16mTrackWidths).
                        Chain(SetupMesh.Setup16mStationMesh, elevatedInfo, metroStationInfo).
                        Chain(SetupTexture.Setup1416mTexture).
                        Chain(
                            (info, version) =>
                            {
                                LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUp.BuildUp(info, version); });
                            }), false,
                   NetInfoVersion.Ground | NetInfoVersion.Elevated | NetInfoVersion.Tunnel,
                    ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
                        Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(
                            LinkToNonGroundVersions, null, NetInfoVersion.None)
                    , prefabName => prefabName + " Island"
                    );
            }
            catch (Exception e)
            {
                Next.Debug.Log("Exception happened when setting up concrete island station tracks");
                UnityEngine.Debug.LogException(e);
            }

            try
            {
                CreateFullStationPrefab(
                    ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                        Chain(CustomizationSteps.SetupStationProps).
                        Chain(CustomizationSteps.CommonCustomizationSmall).
                        //Chain(CustomizationSteps.CommonConcreteCustomization).
                        Chain(CustomizationSteps.SetSmallStationTrackWidths).
                        Chain(SetupMesh.Setup6mStationMesh, elevatedInfo, metroStationInfo).
                        Chain(SetupTexture.Setup6mTexture).
                        Chain(
                            (info, version) =>
                            {
                                LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUp.BuildUp(info, version); });
                            }), false,
                   NetInfoVersion.Ground | NetInfoVersion.Elevated | NetInfoVersion.Tunnel,
                    ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
                        Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(
                            LinkToNonGroundVersions, null, NetInfoVersion.None)
                    , prefabName => prefabName + " Small"
                    );
            }
            catch (Exception e)
            {
                Next.Debug.Log("Exception happened when setting up concrete small station tracks");
                UnityEngine.Debug.LogException(e);
            }

        }
        #endregion

        #region STEEL
        private void CreateSteelTracks()
        {
            var elevatedInfo = FindOriginalNetInfo("Basic Road Elevated");
            var metroInfo = FindOriginalNetInfo("Metro Track");
            var metroStationInfo = FindOriginalNetInfo("Metro Station Track");
            var trainTrackInfo = FindOriginalNetInfo("Train Track");

            try
            {
                CreateFullPrefab(
                    ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                        Chain(CustomizationSteps.SetupTrackProps).
                        Chain(CustomizationSteps.SetStandardTrackWidths).
                        Chain(CustomizationSteps.CommonCustomization).
                        Chain(CustomizationSteps.CommonSteelCustomization).
                        Chain(SetupSteelMesh.Setup10mSteelMesh, elevatedInfo, metroInfo).
                        Chain(SetupSteelMesh.Setup10mSteelBarMesh, elevatedInfo).
                        Chain(SetupSteelTexture.Setup10mSteelTexture).
                        Chain(
                            (info, version) =>
                            {
                                LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUpSteel.BuildUp(info, version); });
                            }),
                    NetInfoVersion.Ground | NetInfoVersion.Elevated | NetInfoVersion.Bridge | NetInfoVersion.Slope | NetInfoVersion.Tunnel,
                    ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
                        Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(
                            LinkToNonGroundVersions,
                            null,
                            //TODO(earalov): replace wuth prefabName => "Steel " + prefabName when tunnel/bridge/slope are ready
                            NetInfoVersion.None)
                    , prefabName => "Steel " + prefabName
                    );
            }
            catch (Exception e)
            {
                Next.Debug.Log("Exception happened when setting up steel tracks");
                UnityEngine.Debug.LogException(e);
            }
            try
            {
                CreateFullPrefab(
                    ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                        Chain(CustomizationSteps.SetupTrackProps).
                        Chain(CustomizationSteps.CommonCustomizationLarge).
                        Chain(CustomizationSteps.CommonSteelCustomization).
                        Chain(CustomizationSteps.SetLargeTrackWidths).
                        Chain(SetupSteelMesh.Setup10mSteelMesh, elevatedInfo, trainTrackInfo).
                        Chain(SetupSteelMesh.Setup10mSteelBarMesh, elevatedInfo).
                        Chain(SetupSteelTexture.Setup10mSteelTexture).
                        Chain(
                            (info, version) =>
                            {
                                LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUpSteel.BuildUp(info, version); });
                            }),
                    NetInfoVersion.Ground | NetInfoVersion.Elevated | NetInfoVersion.Bridge | NetInfoVersion.Slope | NetInfoVersion.Tunnel,
                    ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
                        Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(
                            LinkToNonGroundVersions,
                            null,
                            //TODO(earalov): replace wuth prefabName => "Steel " + prefabName when tunnel/bridge/slope are ready
                            NetInfoVersion.None)
                    , prefabName => "Steel " + prefabName + " Large"
                    );
            }
            catch (Exception e)
            {
                Next.Debug.Log("Exception happened when setting up steel large tracks");
                UnityEngine.Debug.LogException(e);
            }

            try
            {
                CreateFullPrefab(
                    ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                        Chain(CustomizationSteps.SetupTrackProps).
                        Chain(CustomizationSteps.CommonCustomizationLarge).
                        Chain(CustomizationSteps.CommonSteelCustomization).
                        Chain(CustomizationSteps.SetLargeTrackWidths).
                        Chain(SetupSteelMesh.Setup10mSteelMesh, elevatedInfo, trainTrackInfo).
                        Chain(SetupSteelMesh.Setup10mSteelNoBarMesh, elevatedInfo, trainTrackInfo).
                        Chain(SetupSteelTexture.Setup10mSteelTexture).
                        Chain(
                            (info, version) =>
                            {
                                LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUpSteel.BuildUp(info, version); });
                            }),
                    NetInfoVersion.Ground | NetInfoVersion.Elevated | NetInfoVersion.Bridge | NetInfoVersion.Slope | NetInfoVersion.Tunnel,
                    ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
                        Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(
                            LinkToNonGroundVersions,
                            null,
                            //TODO(earalov): replace wuth prefabName => "Steel " + prefabName when tunnel/bridge/slope are ready
                            NetInfoVersion.None)
                    , prefabName => "Steel " + prefabName + " Large NoBar"
                    );
            }
            catch (Exception e)
            {
                Next.Debug.Log("Exception happened when setting up steel large nobar tracks");
                UnityEngine.Debug.LogException(e);
            }
            try
            {
                CreateFullPrefab(
                    ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                        Chain(CustomizationSteps.CommonCustomization).
                        Chain(CustomizationSteps.CommonSteelCustomization).
                        Chain(CustomizationSteps.SetupTrackProps).
                        Chain(CustomizationSteps.SetStandardTrackWidths).
                        Chain(SetupSteelMesh.Setup10mSteelMesh, elevatedInfo, metroInfo).
                        Chain(SetupSteelMesh.Setup10mSteelNoBarMesh, elevatedInfo, metroInfo).
                        Chain(SetupSteelTexture.Setup10mSteelTexture).
                        Chain(
                            (info, version) =>
                            {
                                LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUpSteel.BuildUp(info, version); });
                            }),
                    NetInfoVersion.Ground | NetInfoVersion.Elevated | NetInfoVersion.Bridge | NetInfoVersion.Slope | NetInfoVersion.Tunnel,
                    ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
                        Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(
                            LinkToNonGroundVersions,
                            null,
                            //TODO(earalov): replace wuth prefabName => "Steel " + prefabName when tunnel/bridge/slope are ready
                            NetInfoVersion.None)
                    , prefabName => "Steel " + prefabName + " NoBar"
                    );
            }
            catch (Exception e)
            {
                Next.Debug.Log("Exception happened when setting up nobar steel tracks");
                UnityEngine.Debug.LogException(e);
            }

            try
            {
                CreateFullPrefab(
                    ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                        Chain(CustomizationSteps.SetupTrackProps).
                        Chain(CustomizationSteps.CommonCustomizationTwoLaneOneWay).
                        Chain(CustomizationSteps.CommonSteelCustomization).
                        Chain(CustomizationSteps.SetStandardTrackWidths).
                        Chain(SetupSteelMesh.Setup10mSteelMesh, elevatedInfo, metroInfo).
                        Chain(SetupSteelMesh.Setup10mSteelBarMesh, elevatedInfo).
                        Chain(SetupSteelTexture.Setup10mSteelTexture).
                        Chain(
                            (info, version) =>
                            {
                                LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUpSteel.BuildUp(info, version); });
                            }),
                    NetInfoVersion.Ground | NetInfoVersion.Elevated | NetInfoVersion.Bridge | NetInfoVersion.Slope | NetInfoVersion.Tunnel,
                    ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
                        Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(
                            LinkToNonGroundVersions,
                            null,
                            //TODO(earalov): replace wuth prefabName => "Steel " + prefabName when tunnel/bridge/slope are ready
                            NetInfoVersion.None)
                    , prefabName => "Steel " + prefabName + " Two-Lane One-Way"
                    );
            }
            catch (Exception e)
            {
                Next.Debug.Log("Exception happened when setting up two lane one way steel tracks");
                UnityEngine.Debug.LogException(e);
            }

            try
            {
                CreateFullPrefab(
                    ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                        Chain(CustomizationSteps.SetupTrackProps).
                        Chain(CustomizationSteps.CommonCustomizationTwoLaneOneWay).
                        Chain(CustomizationSteps.CommonSteelCustomization).
                        Chain(CustomizationSteps.SetStandardTrackWidths).
                        Chain(SetupSteelMesh.Setup10mSteelMesh, elevatedInfo, metroInfo).
                        Chain(SetupSteelMesh.Setup10mSteelNoBarMesh, elevatedInfo, metroInfo).
                        Chain(SetupSteelTexture.Setup10mSteelTexture).
                        Chain(
                            (info, version) =>
                            {
                                LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUpSteel.BuildUp(info, version); });
                            }),
                    NetInfoVersion.Ground | NetInfoVersion.Elevated | NetInfoVersion.Bridge | NetInfoVersion.Slope | NetInfoVersion.Tunnel,
                    ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
                        Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(
                            LinkToNonGroundVersions,
                            null,
                            //TODO(earalov): replace wuth prefabName => "Steel " + prefabName when tunnel/bridge/slope are ready
                            NetInfoVersion.None)
                    , prefabName => "Steel " + prefabName + " Two-Lane One-Way NoBar"
                    );
            }
            catch (Exception e)
            {
                Next.Debug.Log("Exception happened when setting up two lane one way nobar steel tracks");
                UnityEngine.Debug.LogException(e);
            }

            try
            {
                CreateFullPrefab(
                    ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                        Chain(CustomizationSteps.SetupTrackProps).
                        Chain(CustomizationSteps.CommonCustomizationSmall).
                        Chain(CustomizationSteps.CommonSteelCustomization).
                        Chain(CustomizationSteps.SetSmallTrackWidths).
                        Chain(SetupSteelMesh.Setup6mSteelMesh, elevatedInfo, trainTrackInfo).
                        Chain(SetupSteelMesh.Setup6mSteelMeshBar, elevatedInfo).
                        Chain(SetupSteelTexture.Setup6mSteelTexture).
                        Chain(
                            (info, version) =>
                            {
                                LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUpSteel.BuildUp(info, version); });
                            }),
                    NetInfoVersion.Ground | NetInfoVersion.Elevated | NetInfoVersion.Bridge | NetInfoVersion.Slope | NetInfoVersion.Tunnel,
                    ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
                        Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(
                            LinkToNonGroundVersions,
                            null,
                            //TODO(earalov): replace wuth prefabName => "Steel " + prefabName when tunnel/bridge/slope are ready
                            NetInfoVersion.None)
                    , prefabName => "Steel " + prefabName + " Small"
                    );
            }
            catch (Exception e)
            {
                Next.Debug.Log("Exception happened when setting up steel small tracks");
                UnityEngine.Debug.LogException(e);
            }

            try
            {
                CreateFullPrefab(
                    ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                        Chain(CustomizationSteps.SetupTrackProps).
                        Chain(CustomizationSteps.CommonCustomizationSmall).
                        Chain(CustomizationSteps.CommonSteelCustomization).
                        Chain(CustomizationSteps.SetSmallTrackWidths).
                        Chain(SetupSteelMesh.Setup6mSteelMesh, elevatedInfo, trainTrackInfo).
                        Chain(SetupSteelMesh.Setup6mSteelMeshNoBar, elevatedInfo, trainTrackInfo).
                        Chain(SetupSteelTexture.Setup6mSteelTexture).
                        Chain(
                            (info, version) =>
                            {
                                LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUpSteel.BuildUp(info, version); });
                            }),
                    NetInfoVersion.Ground | NetInfoVersion.Elevated | NetInfoVersion.Bridge | NetInfoVersion.Slope | NetInfoVersion.Tunnel,
                    ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
                        Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(
                            LinkToNonGroundVersions,
                            null,
                            //TODO(earalov): replace wuth prefabName => "Steel " + prefabName when tunnel/bridge/slope are ready
                            NetInfoVersion.None)
                    , prefabName => "Steel " + prefabName + " Small NoBar"
                    );
            }
            catch (Exception e)
            {
                Next.Debug.Log("Exception happened when setting up steel small nobar tracks");
                UnityEngine.Debug.LogException(e);
            }
            try
            {
                CreateFullPrefab(
                    ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                        Chain(CustomizationSteps.SetupTrackProps).
                        Chain(CustomizationSteps.CommonCustomizationSmall).
                        Chain(CustomizationSteps.CommonSteelCustomization).
                        Chain(CustomizationSteps.SetSmallTrackWidths).
                        Chain(SetupSteelMesh.Setup6mSteelMesh, elevatedInfo, trainTrackInfo).
                        Chain(SetupSteelMesh.Setup6mSteelMeshBar, elevatedInfo).
                        Chain(SetupSteelTexture.Setup6mSteelTexture).
                        Chain(
                            (info, version) =>
                            {
                                LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUpSteel.BuildUp(info, version); });
                            }),
                    NetInfoVersion.Ground | NetInfoVersion.Elevated | NetInfoVersion.Bridge | NetInfoVersion.Slope | NetInfoVersion.Tunnel,
                    ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
                        Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(
                            LinkToNonGroundVersions,
                            null,
                            //TODO(earalov): replace wuth prefabName => "Steel " + prefabName when tunnel/bridge/slope are ready
                            NetInfoVersion.None)
                    , prefabName => "Steel " + prefabName + " Small Two-Way"
                    );
            }
            catch (Exception e)
            {
                Next.Debug.Log("Exception happened when setting up steel small two way tracks");
                UnityEngine.Debug.LogException(e);
            }

            try
            {
                CreateFullPrefab(
                    ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                        Chain(CustomizationSteps.SetupTrackProps).
                        Chain(CustomizationSteps.CommonCustomizationSmall).
                        Chain(CustomizationSteps.CommonSteelCustomization).
                        Chain(CustomizationSteps.SetSmallTrackWidths).
                        Chain(SetupSteelMesh.Setup6mSteelMesh, elevatedInfo, trainTrackInfo).
                        Chain(SetupSteelMesh.Setup6mSteelMeshNoBar, elevatedInfo, trainTrackInfo).
                        Chain(SetupSteelTexture.Setup6mSteelTexture).
                        Chain(
                            (info, version) =>
                            {
                                LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUpSteel.BuildUp(info, version); });
                            }),
                    NetInfoVersion.Ground | NetInfoVersion.Elevated | NetInfoVersion.Bridge | NetInfoVersion.Slope | NetInfoVersion.Tunnel,
                    ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
                        Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(
                            LinkToNonGroundVersions,
                            null,
                            //TODO(earalov): replace wuth prefabName => "Steel " + prefabName when tunnel/bridge/slope are ready
                            NetInfoVersion.None)
                    , prefabName => "Steel " + prefabName + " Small Two-Way NoBar"
                    );
            }

            catch (Exception e)
            {
                Next.Debug.Log("Exception happened when setting up steel small two way nobar tracks");
                UnityEngine.Debug.LogException(e);
            }

            try
            {
                CreateFullStationPrefab(
                    ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                        Chain(CustomizationSteps.SetupStationProps).
                        Chain(CustomizationSteps.CommonCustomization).
                        Chain(CustomizationSteps.CommonSteelCustomization).
                        Chain(SetupSteelMesh.Setup10mStationSteelMesh, elevatedInfo, metroInfo)
                        . //TODO(earalov): probably change to station specific method
                        Chain(SetupSteelMesh.Setup10mStationSteelMesh, elevatedInfo, metroStationInfo)
                        . //TODO(earalov): probably change to station specific method
                        Chain(SetupSteelTexture.Setup10mSteelTexture).
                        Chain(
                            (info, version) =>
                            {
                                LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUpSteel.BuildUp(info, version); });
                            }), false,
                   NetInfoVersion.Ground | NetInfoVersion.Elevated,
                    ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
                        Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(
                            LinkToNonGroundVersions, null, NetInfoVersion.None)
                    , prefabName => "Steel " + prefabName
                    );
            }
            catch (Exception e)
            {
                Next.Debug.Log("Exception happened when setting up steel station tracks");
                UnityEngine.Debug.LogException(e);
            }

            try
            {
                CreateFullStationPrefab(
                    ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                        Chain(CustomizationSteps.SetupStationProps).
                        Chain(CustomizationSteps.CommonIsland16mCustomization).
                        Chain(CustomizationSteps.CommonSteelCustomization).
                        Chain(CustomizationSteps.Set16mTrackWidths).
                        Chain(SetupSteelMesh.Setup16mSteelStationMesh, elevatedInfo, metroStationInfo).
                        Chain(SetupSteelTexture.Setup1416mSteelTexture).
                        Chain(
                            (info, version) =>
                            {
                                LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUpSteel.BuildUp(info, version); });
                            }), false,
                   NetInfoVersion.Ground | NetInfoVersion.Elevated,
                    ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
                        Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(
                            LinkToNonGroundVersions, null, NetInfoVersion.None)
                    , prefabName => "Steel " + prefabName + " Island"
                    );
            }
            catch (Exception e)
            {
                Next.Debug.Log("Exception happened when setting up steel island station tracks");
                UnityEngine.Debug.LogException(e);
            }

            try
            {
                CreateFullStationPrefab(
                    ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                        Chain(CustomizationSteps.SetupStationProps).
                        Chain(CustomizationSteps.CommonCustomizationSmall).
                        Chain(CustomizationSteps.CommonSteelCustomization).
                        Chain(CustomizationSteps.SetSmallTrackWidths).
                        Chain(SetupSteelMesh.Setup6mStationSteelMesh, elevatedInfo, metroStationInfo).
                        Chain(SetupSteelTexture.Setup6mSteelTexture).
                        Chain(
                            (info, version) =>
                            {
                                LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUpSteel.BuildUp(info, version); });
                            }), false,
                   NetInfoVersion.Ground | NetInfoVersion.Elevated,
                    ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
                        Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(
                            LinkToNonGroundVersions, null, NetInfoVersion.None)
                    , prefabName => "Steel " + prefabName + " Small"
                    );
            }
            catch (Exception e)
            {
                Next.Debug.Log("Exception happened when setting up small steel station tracks");
                UnityEngine.Debug.LogException(e);
            }
        }
        #endregion

        #region COMMON
        protected void CreateFullPrefab(Action<NetInfo, NetInfoVersion> customizationStep,
            NetInfoVersion versions,
            Action<NetInfo, Action<NetInfo, NetInfoVersion>> setupOtherVersionsStep,
            Func<string, string> nameModifier = null, Dictionary<NetInfoVersion, string> replacements = null)
        {
            if (nameModifier == null)
            {
                nameModifier = s => s;
            }
            NetInfo groundVersion = null;
            if ((versions & NetInfoVersion.Ground) != 0)
            {
                string replaces = null;
                replacements?.TryGetValue(NetInfoVersion.Ground, out replaces);
                groundVersion = CreateNetInfo(nameModifier.Invoke("Metro Track Ground"), "Train Track",
                    ActionExtensions.BeginChain<NetInfo>().
                        Chain(ReplaceAI, NetInfoVersion.Ground).
                        Chain(SetupMetroTrackMeta, NetInfoVersion.Ground).
                        Chain(p =>
                        {
                            setupOtherVersionsStep?.Invoke(p, customizationStep);
                            p.GetComponent<TrainTrackAI>().m_connectedElevatedInfo = null;
                            p.GetComponent<TrainTrackAI>().m_connectedInfo = null;
                        }).
                        Chain(SetupTrackModel, customizationStep.Chain(SetCosts)), replaces
                );
            }
            foreach (var version in new[] { NetInfoVersion.Bridge, NetInfoVersion.Tunnel, NetInfoVersion.Elevated, NetInfoVersion.Slope, })
            {
                if ((versions & version) == 0)
                {
                    continue;
                }
                string replaces = null;
                replacements?.TryGetValue(version, out replaces);
                CreateNetInfo(nameModifier.Invoke("Metro Track " + version), "Train Track " + version,
                    ActionExtensions.BeginChain<NetInfo>().
                        Chain(ReplaceAI, version).
                        Chain(SetupMetroTrackMeta, version).
                        Chain(CommonSteps.SetVersion, groundVersion, version).
                        Chain(SetupTrackModel, customizationStep.Chain(SetCosts)), replaces
                    );
            }
        }

        protected void LinkToNonGroundVersions(NetInfo p, Action<NetInfo, NetInfoVersion> customizationStep,
            Func<string, string> nameModifier = null, NetInfoVersion versions = NetInfoVersion.Slope | NetInfoVersion.Tunnel | NetInfoVersion.Elevated | NetInfoVersion.Bridge)
        {
            if (nameModifier == null)
            {
                nameModifier = s => s;
            }
            foreach (var version in new[] { NetInfoVersion.Bridge, NetInfoVersion.Tunnel, NetInfoVersion.Elevated, NetInfoVersion.Slope, })
            {
                if ((versions & version) == 0)
                {
                    continue;
                }
                CommonSteps.SetVersion(FindCustomNetInfo(nameModifier.Invoke("Metro Track " + version)), p, version);
            }
        }

        //TODO(earalov): refactor like CreateFullPrefab()
        private void CreateFullStationPrefab(Action<NetInfo, NetInfoVersion> customizationStep, bool provideSunken,
            NetInfoVersion versions,
            Action<NetInfo, Action<NetInfo, NetInfoVersion>> setupOtherVersionsStep,
            Func<string, string> nameModifier = null, Dictionary<NetInfoVersion, string> replacements = null)
        {
            NetInfo groundVersion = null;
            if ((versions & NetInfoVersion.Ground) != 0)
            {

                groundVersion = CreateNetInfo(nameModifier.Invoke("Metro Station Track Ground"), "Train Station Track",
                    ActionExtensions.BeginChain<NetInfo>().
                        Chain(ReplaceAI, NetInfoVersion.Ground).
                        Chain(SetupMetroTrackMeta, NetInfoVersion.Ground).
                        Chain(SetupStationTrack, NetInfoVersion.Ground).
                        Chain(CustomizationSteps.SetStandardTrackWidths, NetInfoVersion.Ground).
                        Chain(p =>
                        {
                            setupOtherVersionsStep?.Invoke(p, customizationStep);
                            p.GetComponent<TrainTrackAI>().m_connectedElevatedInfo = null;
                            p.GetComponent<TrainTrackAI>().m_connectedInfo = null;
                        }).
                        Chain(SetupTrackModel, customizationStep)
                );
            }
            foreach (var version in new[] { NetInfoVersion.Tunnel, NetInfoVersion.Elevated })
            {
                if ((versions & version) == 0)
                {
                    continue;
                }
                string replaces = null;
                replacements?.TryGetValue(version, out replaces);
                CreateNetInfo(nameModifier.Invoke("Metro Station Track " + version), "Train Station Track",
                    ActionExtensions.BeginChain<NetInfo>().
                        Chain(ReplaceAI, version).
                        Chain(SetupMetroTrackMeta, version).
                        Chain(SetupStationTrack, version).
                        Chain(CommonSteps.SetVersion, groundVersion, version).
                        Chain(Modifiers.MakePedestrianLanesNarrow, version).
                        Chain(SetupTrackModel, customizationStep.Chain(SetCosts)), replaces
                    );
            }
            if (provideSunken)
            {
                CreateNetInfo(nameModifier.Invoke("Metro Station Track Sunken"), "Train Station Track", //TODO(earalov): test. check if AI to be replaced with MetroTrackAI
                    ActionExtensions.BeginChain<NetInfo>().
                    Chain(ReplaceAI, NetInfoVersion.Tunnel).
                    Chain(SetupMetroTrackMeta, NetInfoVersion.Ground).
                    Chain(SetupStationTrack, NetInfoVersion.Ground).
                    Chain(SetupSunkenStationTrack).
                    Chain(SetupTrackModel, customizationStep)
                );
            }
        }

        private static void ReplaceAI(NetInfo prefab, NetInfoVersion version)
        {
            var originalAi = prefab.GetComponent<PlayerNetAI>(); //milestone, construction and maintenance costs to be overriden later
            var canModify = originalAi.CanModify();
            int noiseAccumulation;
            float noiseRadius;
            originalAi.GetNoiseAccumulation(out noiseAccumulation, out noiseRadius);
            if (originalAi is TrainTrackTunnelAI || version == NetInfoVersion.Tunnel)
            {
                if ((originalAi is TrainTrackTunnelAI && version == NetInfoVersion.Slope))
                {
                    GameObject.DestroyImmediate(originalAi);
                    var ai = prefab.gameObject.AddComponent<TrainTrackTunnelAIMetro>();
                    ai.m_canModify = canModify;
                    ai.m_noiseAccumulation = noiseAccumulation;
                    ai.m_noiseRadius = noiseRadius;
                    ai.m_info = prefab;
                    prefab.m_netAI = ai;
                }
                else
                {
                    GameObject.DestroyImmediate(originalAi);
                    var ai = prefab.gameObject.AddComponent<MetroTrackAIMetro>();
                    ai.m_info = prefab;
                    ai.m_transportInfo = PrefabCollection<TransportInfo>.FindLoaded("Train");
                    prefab.m_netAI = ai;
                }

            }
            else if (version == NetInfoVersion.Bridge || version == NetInfoVersion.Elevated)
            {
                GameObject.DestroyImmediate(originalAi);
                var ai = prefab.gameObject.AddComponent<TrainTrackBridgeAIMetro>();
                ai.m_canModify = canModify;
                ai.m_noiseAccumulation = noiseAccumulation;
                ai.m_noiseRadius = noiseRadius;
                ai.m_info = prefab;
                prefab.m_netAI = ai;
            }
            else
            {
                GameObject.DestroyImmediate(originalAi);
                var ai = prefab.gameObject.AddComponent<TrainTrackAIMetro>();
                ai.m_noiseAccumulation = noiseAccumulation;
                ai.m_noiseRadius = noiseRadius;
                ai.m_info = prefab;
                prefab.m_netAI = ai;
            }
        }

        public static void SetupElevatedStationTrack(NetInfo prefab)
        {
        }

        public static void SetupSunkenStationTrack(NetInfo prefab)
        {
            prefab.m_maxHeight = -1;
            prefab.m_minHeight = -3;
            prefab.m_lowerTerrain = false;
            prefab.m_class.m_layer = ItemClass.Layer.MetroTunnels | ItemClass.Layer.Default;
        }

        public static void SetupTunnelStationTrack(NetInfo prefab)
        {
            prefab.m_maxHeight = -1;
            prefab.m_minHeight = -5;
            prefab.m_lowerTerrain = false;
            prefab.m_pavementWidth = 4.5f;
            prefab.m_halfWidth = 8;
            prefab.m_class.m_layer = ItemClass.Layer.MetroTunnels;
        }

        public static void SetupStationTrack(NetInfo prefab, NetInfoVersion version)
        {
            prefab.m_followTerrain = false;
            prefab.m_flattenTerrain = false;
            prefab.m_createGravel = false;
            prefab.m_createPavement = false;
            prefab.m_createRuining = false;
            prefab.m_requireSurfaceMaps = false;
            prefab.m_snapBuildingNodes = false;
            prefab.m_placementStyle = ItemClass.Placement.Procedural;
            prefab.m_useFixedHeight = true;
            prefab.m_availableIn = ItemClass.Availability.Game;
            prefab.m_intersectClass = null;
            if (version == NetInfoVersion.Ground)
            {
                prefab.m_lowerTerrain = false;
                prefab.m_clipTerrain = true;
            }
            else
            {
                prefab.m_clipTerrain = false;
                if (version == NetInfoVersion.Elevated)
                {

                }
                else if (version == NetInfoVersion.Tunnel)
                {
                    prefab.m_maxHeight = -1;
                    prefab.m_minHeight = -5;
                    prefab.m_lowerTerrain = false;
                    prefab.m_pavementWidth = 4.5f;
                    prefab.m_halfWidth = 8;
                    prefab.m_class.m_layer = ItemClass.Layer.MetroTunnels;
                }
            }
        }

        public static NetInfoVersion DetectVersion(string infoName)
        {
            if (infoName.Contains("Elevated"))
            {
                return NetInfoVersion.Elevated;
            }
            if (infoName.Contains("Bridge"))
            {
                return NetInfoVersion.Bridge;
            }
            if (infoName.Contains("Slope"))
            {
                return NetInfoVersion.Slope;
            }
            return infoName.Contains("Tunnel") ? NetInfoVersion.Tunnel : NetInfoVersion.Ground;
        }

        private static void SetupTrackModel(NetInfo prefab, Action<NetInfo, NetInfoVersion> customizationStep)
        {
            prefab.m_minHeight = 0; //TODO(earalov): is that minHeight correct for all types of tracks?
            var version = DetectVersion(prefab.name);

            customizationStep.Invoke(prefab, version);
        }


        private static void SetupMetroTrackMeta(NetInfo prefab, NetInfoVersion version)
        {
            var vanillaMetroTrack = FindOriginalNetInfo("Metro Track");
            if (!prefab.name.Contains("Station"))
            {
            var milestone = vanillaMetroTrack.GetComponent<PlayerNetAI>().m_createPassMilestone;
            prefab.GetComponent<PlayerNetAI>().m_createPassMilestone = milestone;
            }
            prefab.m_class = ScriptableObject.CreateInstance<ItemClass>();
            prefab.m_class.m_subService = ItemClass.SubService.PublicTransportMetro;
            prefab.m_class.m_layer = ItemClass.Layer.MetroTunnels;
            prefab.m_class.m_service = ItemClass.Service.PublicTransport;
            //prefab.m_class.m_level = ItemClass.Level.Level1;
            prefab.m_UIPriority = vanillaMetroTrack.m_UIPriority;
            prefab.SetUICategory("PublicTransportMetro");
            if (version == NetInfoVersion.Tunnel)
            {
                prefab.m_class.m_layer = ItemClass.Layer.MetroTunnels;
                prefab.m_setVehicleFlags = Vehicle.Flags.Transition | Vehicle.Flags.Underground;
                prefab.m_setCitizenFlags = CitizenInstance.Flags.Transition | CitizenInstance.Flags.Underground;
            }
            else
            {
                prefab.m_class.m_layer = ItemClass.Layer.Default;
            }
            prefab.m_availableIn = ItemClass.Availability.AssetEditor;
            prefab.m_class.hideFlags = HideFlags.None;
            prefab.m_class.name = prefab.name;
            prefab.m_maxBuildAngle = 60;
            prefab.m_maxTurnAngleCos = Mathf.Cos(prefab.m_maxBuildAngle);
            prefab.m_maxTurnAngle = 60;
            prefab.m_maxTurnAngleCos = Mathf.Cos(prefab.m_maxTurnAngle);
            prefab.m_averageVehicleLaneSpeed = vanillaMetroTrack.m_averageVehicleLaneSpeed;
            prefab.m_UnlockMilestone = vanillaMetroTrack.m_UnlockMilestone;
            prefab.m_createGravel = false;
            prefab.m_createPavement = false;
            prefab.m_isCustomContent = true; //this line is responsible for moving tracks to the end of the list and that's not what we're interested in  

            var speedLimit = vanillaMetroTrack.m_lanes.First(l => l.m_vehicleType != VehicleInfo.VehicleType.None).m_speedLimit;

            foreach (var lane in prefab.m_lanes)
            {
                if (lane.m_vehicleType == VehicleInfo.VehicleType.None)
                {
                    lane.m_stopType = VehicleInfo.VehicleType.Metro;
                }
                else
                {
                    lane.m_vehicleType = VehicleInfo.VehicleType.Metro;
                    lane.m_speedLimit = speedLimit;
                }
            }

            Modifiers.RemoveElectricityPoles(prefab);
        }

        public static void SetCosts(PrefabInfo newPrefab, NetInfoVersion version)
        {
            if (newPrefab.name.Contains("Metro"))
            {
                var metroTrackInfo = FindOriginalNetInfo("Metro Track");
                var baseConstructionCost = metroTrackInfo.GetComponent<PlayerNetAI>().m_constructionCost;
                var baseMaintenanceCost = metroTrackInfo.GetComponent<PlayerNetAI>().m_maintenanceCost;
                var newAi = newPrefab.GetComponent<PlayerNetAI>();

                var multiplier = GetCostMultiplier(version);
                newAi.m_constructionCost = (int)(baseConstructionCost * multiplier);
                newAi.m_maintenanceCost = (int)(baseMaintenanceCost * multiplier);
            }
        }

        public static float GetCostMultiplier(NetInfoVersion version)
        {
            float multiplier;
            switch (version)
            {
                case NetInfoVersion.Elevated:
                    multiplier = 2f;
                    break;
                case NetInfoVersion.Bridge:
                    multiplier = 3f;
                    break;
                case NetInfoVersion.Tunnel:
                case NetInfoVersion.Slope:
                    multiplier = 4f;
                    break;
                default:
                    multiplier = 1f;
                    break;
            }
            return multiplier;
        }
        #endregion
    }
}
