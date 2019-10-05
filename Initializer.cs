using System;
using System.Collections.Generic;
using System.Linq;
using ICities;
using MetroOverhaul.InitializationSteps;
using UnityEngine;
using MetroOverhaul.NEXT;
using MetroOverhaul.NEXT.Extensions;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using MetroOverhaul.Extensions;

namespace MetroOverhaul {
    public class Initializer: AbstractInitializer {
        public AppMode AppMode { get; set; }

        protected override void InitializeNetInfoImpl()
        {
            CreateConcreteTracks();
            CreateSteelTracks();
            ExtendVanillaTracks();
            if (AppMode != AppMode.AssetEditor)
            {
                AssetsUpdater.PreventVanillaMetroTrainSpawning();
                AssetsUpdater.UpdateVanillaMetroTracks();
            }
        }
        public override void InitializeBuildingInfoImpl(BuildingInfo info)
        {
            CreateCloneBuilding(info);
        }

        #region CONCRETE
        private void CreateConcreteTracks()
        {
            var elevatedInfo = FindOriginalNetInfo("Basic Road Elevated");
            var trainTrackInfo = FindOriginalNetInfo("Train Track");
            var metroInfo = FindOriginalNetInfo(Util.GetMetroTrackName(NetInfoVersion.Tunnel, TrackStyle.Vanilla));
            var metroStationInfo = FindOriginalNetInfo(Util.GetMetroStationTrackName(NetInfoVersion.Tunnel, TrackStyle.Vanilla));
            try
            {
                var replacements = new Dictionary<NetInfoVersion, string> { { NetInfoVersion.Tunnel, Util.GetMetroTrackName(NetInfoVersion.Tunnel, TrackStyle.Modern) } };
                CreateFullPrefab(
                    ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
//                        Chain(CustomizationSteps.SetupTrackProps).
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
                    NetInfoVersion.All, null, prefabName => "Concrete " + prefabName, replacements
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
//                        Chain(CustomizationSteps.SetupTrackProps).
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
                    NetInfoVersion.All,
                    ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
                        Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(
                            LinkToNonGroundVersions, null, NetInfoVersion.None)
                    , prefabName => "Concrete " + prefabName + " NoBar"
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
//                        Chain(CustomizationSteps.SetupTrackProps).
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
                    NetInfoVersion.All,
                    ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
                        Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(
                            LinkToNonGroundVersions, null, NetInfoVersion.None)
                    , prefabName => "Concrete " + prefabName + " Two-Lane One-Way"
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
//                        Chain(CustomizationSteps.SetupTrackProps).
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
                    , prefabName => "Concrete " + prefabName + " Two-Lane One-Way NoBar"
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
//                        Chain(CustomizationSteps.SetupTrackProps).
                        Chain(CustomizationSteps.CommonCustomizationLarge).
                        Chain(CustomizationSteps.CommonConcreteCustomization).
                        Chain(CustomizationSteps.ReplaceTrackIcon).
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
                    , prefabName => "Concrete " + prefabName + " Large"
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
//                        Chain(CustomizationSteps.SetupTrackProps).
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
                    , prefabName => "Concrete " + prefabName + " Large NoBar"
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
//                        Chain(CustomizationSteps.SetupTrackProps).
                        Chain(CustomizationSteps.CommonCustomizationSmall).
                        Chain(CustomizationSteps.CommonConcreteCustomization).
                        Chain(CustomizationSteps.ReplaceTrackIcon).
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
                    , prefabName => "Concrete " + prefabName + " Small"
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
//                        Chain(CustomizationSteps.SetupTrackProps).
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
                    , prefabName => "Concrete " + prefabName + " Small NoBar"
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
//                        Chain(CustomizationSteps.SetupTrackProps).
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
                    , prefabName => "Concrete " + prefabName + " Small Two-Way"
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
//                        Chain(CustomizationSteps.SetupTrackProps).
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
                    , prefabName => "Concrete " + prefabName + " Small Two-Way NoBar"
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
                        //Chain(CustomizationSteps.SetupStationProps).
                        Chain(CustomizationSteps.CommonCustomization).
                        Chain(CustomizationSteps.SetStandardStationTrackWidths).
                        Chain(CustomizationSteps.CommonConcreteCustomization).
                        Chain(SetupMesh.Setup10mStationMesh, elevatedInfo, metroStationInfo).
                        Chain(SetupTexture.Setup10mTexture).
                        Chain(
                            (info, version) =>
                            {
                                LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUp.BuildUp(info, version); });
                            }),
                   NetInfoVersion.Ground | NetInfoVersion.Elevated | NetInfoVersion.Tunnel,
                    ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
                        Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(
                            LinkToNonGroundVersions, null, NetInfoVersion.None)
                    , prefabName => "Concrete " + prefabName, replacements
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
                        //Chain(CustomizationSteps.SetupStationProps).
                        Chain(CustomizationSteps.CommonCustomization).
                        Chain(CustomizationSteps.SetStandardStationTrackWidths).
                        Chain(CustomizationSteps.CommonConcreteCustomization).
                        Chain(SetupMesh.Setup10mStationMesh, elevatedInfo, metroStationInfo).
                        Chain(SetupTexture.Setup10mTexture).
                        Chain(
                            (info, version) =>
                            {
                                LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUp.BuildUp(info, version); });
                            }),
                   NetInfoVersion.Ground,
                    ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
                        Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(
                            LinkToNonGroundVersions, null, NetInfoVersion.None)
                    , prefabName => prefabName
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
                        //Chain(CustomizationSteps.SetupStationProps).
                        Chain(CustomizationSteps.CommonIslandCustomization).
                        Chain(CustomizationSteps.SetIslandTrackWidths).
                        Chain(CustomizationSteps.CommonConcreteCustomization).
                        Chain(SetupMesh.Setup19mStationMesh, elevatedInfo, metroStationInfo).
                        Chain(SetupTexture.Setup19mTexture).
                        Chain(
                            (info, version) =>
                            {
                                LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUp.BuildUp(info, version); });
                            }),
                   NetInfoVersion.Ground | NetInfoVersion.Elevated | NetInfoVersion.Tunnel,
                    ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
                        Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(
                            LinkToNonGroundVersions, null, NetInfoVersion.None)
                    , prefabName => "Concrete " + prefabName + " Island"
                    );
            }
            catch (Exception e)
            {
                Next.Debug.Log("Exception happened when setting up concrete island station tracks");
                UnityEngine.Debug.LogException(e);
            }

            //try
            //{
            //    CreateFullStationPrefab(
            //        ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
            //            Chain(CustomizationSteps.SetupStationProps).
            //            Chain(CustomizationSteps.CommonLargeSideIslandCustomization).
            //            Chain(CustomizationSteps.SetLargeIslandTrackWidths).
            //            Chain(CustomizationSteps.CommonConcreteCustomization).
            //            Chain(SetupMesh.Setup19mStationMesh, elevatedInfo, metroStationInfo).
            //            Chain(SetupTexture.Setup19mTexture).
            //            Chain(
            //                (info, version) =>
            //                {
            //                    LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUp.BuildUp(info, version); });
            //                }),
            //       NetInfoVersion.Ground | NetInfoVersion.Elevated | NetInfoVersion.Tunnel,
            //        ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
            //            Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(
            //                LinkToNonGroundVersions, null, NetInfoVersion.None)
            //        , prefabName => prefabName + " Large Side Island"
            //        );
            //}
            //catch (Exception e)
            //{
            //    Next.Debug.Log("Exception happened when setting up concrete large island station tracks");
            //    UnityEngine.Debug.LogException(e);
            //}
            try
            {
                CreateFullStationPrefab(
                    ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                        //Chain(CustomizationSteps.SetupStationProps).
                        Chain(CustomizationSteps.CommonLargeDualIslandCustomization).
                        Chain(CustomizationSteps.SetLargeIslandTrackWidths).
                        Chain(CustomizationSteps.CommonConcreteCustomization).
                        Chain(SetupMesh.Setup19mStationMesh, elevatedInfo, metroStationInfo).
                        Chain(SetupTexture.Setup19mTexture).
                        Chain(
                            (info, version) =>
                            {
                                LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUp.BuildUp(info, version); });
                            }),
                   NetInfoVersion.Ground | NetInfoVersion.Elevated | NetInfoVersion.Tunnel,
                    ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
                        Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(
                            LinkToNonGroundVersions, null, NetInfoVersion.None)
                    , prefabName => "Concrete " + prefabName + " Large Dual Island"
                    );
            }
            catch (Exception e)
            {
                Next.Debug.Log("Exception happened when setting up concrete large island station tracks");
                UnityEngine.Debug.LogException(e);
            }
            try
            {
                CreateFullStationPrefab(
                    ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                        //Chain(CustomizationSteps.SetupStationProps).
                        Chain(CustomizationSteps.CommonCustomizationSmall).
                        Chain(CustomizationSteps.CommonConcreteCustomization).
                        Chain(CustomizationSteps.SetSmallStationTrackWidths).
                        Chain(SetupMesh.Setup6mStationMesh, elevatedInfo, metroStationInfo).
                        Chain(SetupTexture.Setup6mTexture).
                        Chain(
                            (info, version) =>
                            {
                                LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUp.BuildUp(info, version); });
                            }),
                   NetInfoVersion.Ground | NetInfoVersion.Elevated | NetInfoVersion.Tunnel,
                    ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
                        Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(
                            LinkToNonGroundVersions, null, NetInfoVersion.None)
                    , prefabName => "Concrete " + prefabName + " Small"
                    );
            }
            catch (Exception e)
            {
                Next.Debug.Log("Exception happened when setting up concrete small station tracks");
                UnityEngine.Debug.LogException(e);
            }

            try
            {
                CreateFullStationPrefab(
                    ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                        //Chain(CustomizationSteps.SetupStationProps).
                        Chain(CustomizationSteps.CommonCustomizationLarge).
                        Chain(CustomizationSteps.SetLargeStationTrackWidths).
                        Chain(CustomizationSteps.CommonConcreteCustomization).
                        Chain(SetupMesh.Setup10mStationMesh, elevatedInfo, metroStationInfo).
                        Chain(SetupTexture.Setup10mTexture).
                        Chain(
                            (info, version) =>
                            {
                                LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUp.BuildUp(info, version); });
                            }),
                   NetInfoVersion.Ground | NetInfoVersion.Elevated | NetInfoVersion.Tunnel,
                    ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
                        Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(
                            LinkToNonGroundVersions, null, NetInfoVersion.None)
                    , prefabName => "Concrete " + prefabName + " Large"
                    );
            }
            catch (Exception e)
            {
                Next.Debug.Log("Exception happened when setting up concrete large station tracks");
                UnityEngine.Debug.LogException(e);
            }
        }
        #endregion

        #region STEEL
        private void CreateSteelTracks()
        {
            var elevatedInfo = FindOriginalNetInfo("Basic Road Elevated");
            var metroInfo = FindOriginalNetInfo(Util.GetMetroTrackName(NetInfoVersion.Tunnel, TrackStyle.Modern));
            var metroStationInfo = FindOriginalNetInfo(Util.GetMetroStationTrackName(NetInfoVersion.Tunnel, TrackStyle.Modern));
            var trainTrackInfo = FindOriginalNetInfo("Train Track");

            try
            {
                CreateFullPrefab(
                    ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
//                        Chain(CustomizationSteps.SetupTrackProps).
                        Chain(CustomizationSteps.SetStandardTrackWidths).
                        Chain(CustomizationSteps.CommonCustomization).
                        Chain(CustomizationSteps.CommonSteelCustomization).
                        Chain(SetupSteelMesh.Setup10mSteelMesh, elevatedInfo).
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
//                        Chain(CustomizationSteps.SetupTrackProps).
                        Chain(CustomizationSteps.CommonCustomizationLarge).
                        Chain(CustomizationSteps.CommonSteelCustomization).
                        Chain(CustomizationSteps.SetLargeTrackWidths).
                        Chain(SetupSteelMesh.Setup10mSteelMesh, elevatedInfo).
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
//                        Chain(CustomizationSteps.SetupTrackProps).
                        Chain(CustomizationSteps.CommonCustomizationLarge).
                        Chain(CustomizationSteps.CommonSteelCustomization).
                        Chain(CustomizationSteps.SetLargeTrackWidths).
                        Chain(SetupSteelMesh.Setup10mSteelMesh, elevatedInfo).
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
//                        Chain(CustomizationSteps.SetupTrackProps).
                        Chain(CustomizationSteps.SetStandardTrackWidths).
                        Chain(SetupSteelMesh.Setup10mSteelMesh, elevatedInfo).
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
//                        Chain(CustomizationSteps.SetupTrackProps).
                        Chain(CustomizationSteps.CommonCustomizationTwoLaneOneWay).
                        Chain(CustomizationSteps.CommonSteelCustomization).
                        Chain(CustomizationSteps.SetStandardTrackWidths).
                        Chain(SetupSteelMesh.Setup10mSteelMesh, elevatedInfo).
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
//                        Chain(CustomizationSteps.SetupTrackProps).
                        Chain(CustomizationSteps.CommonCustomizationTwoLaneOneWay).
                        Chain(CustomizationSteps.CommonSteelCustomization).
                        Chain(CustomizationSteps.SetStandardTrackWidths).
                        Chain(SetupSteelMesh.Setup10mSteelMesh, elevatedInfo).
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
//                        Chain(CustomizationSteps.SetupTrackProps).
                        Chain(CustomizationSteps.CommonCustomizationSmall).
                        Chain(CustomizationSteps.CommonSteelCustomization).
                        Chain(CustomizationSteps.SetSmallTrackWidths).
                        Chain(SetupSteelMesh.Setup6mSteelMesh, elevatedInfo).
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
//                        Chain(CustomizationSteps.SetupTrackProps).
                        Chain(CustomizationSteps.CommonCustomizationSmall).
                        Chain(CustomizationSteps.CommonSteelCustomization).
                        Chain(CustomizationSteps.SetSmallTrackWidths).
                        Chain(SetupSteelMesh.Setup6mSteelMesh, elevatedInfo).
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
//                        Chain(CustomizationSteps.SetupTrackProps).
                        Chain(CustomizationSteps.CommonCustomizationSmall).
                        Chain(CustomizationSteps.CommonSteelCustomization).
                        Chain(CustomizationSteps.SetSmallTrackWidths).
                        Chain(SetupSteelMesh.Setup6mSteelMesh, elevatedInfo).
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
//                        Chain(CustomizationSteps.SetupTrackProps).
                        Chain(CustomizationSteps.CommonCustomizationSmall).
                        Chain(CustomizationSteps.CommonSteelCustomization).
                        Chain(CustomizationSteps.SetSmallTrackWidths).
                        Chain(SetupSteelMesh.Setup6mSteelMesh, elevatedInfo).
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
                        //Chain(CustomizationSteps.SetupStationProps).
                        Chain(CustomizationSteps.CommonCustomization).
                        Chain(CustomizationSteps.CommonSteelCustomization).
                        Chain(SetupSteelMesh.Setup10mStationSteelMesh, elevatedInfo).
                        Chain(SetupSteelTexture.Setup10mSteelTexture)
                   , NetInfoVersion.Ground | NetInfoVersion.Elevated,
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
            //try
            //{
            //    CreateFullStationPrefab(
            //        ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
            //            Chain(CustomizationSteps.SetupStationProps).
            //            Chain(CustomizationSteps.CommonLargeSideIslandCustomization).
            //            Chain(CustomizationSteps.CommonSteelCustomization).
            //            Chain(CustomizationSteps.SetLargeIslandTrackWidths).
            //            Chain(SetupSteelMesh.Setup19mSteelStationMesh, elevatedInfo, metroStationInfo).
            //            Chain(SetupSteelTexture.Setup19mSteelTexture).
            //            Chain(
            //                (info, version) =>
            //                {
            //                    LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUpSteel.BuildUp(info, version); });
            //                }),
            //       NetInfoVersion.Ground | NetInfoVersion.Elevated,
            //        ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
            //            Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(
            //                LinkToNonGroundVersions, null, NetInfoVersion.None)
            //        , prefabName => "Steel " + prefabName + " Large Island"
            //        );
            //}
            //catch (Exception e)
            //{
            //    Next.Debug.Log("Exception happened when setting up steel island station tracks");
            //    UnityEngine.Debug.LogException(e);
            //}
            try
            {
                CreateFullStationPrefab(
                    ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                        //Chain(CustomizationSteps.SetupStationProps).
                        Chain(CustomizationSteps.CommonLargeDualIslandCustomization).
                        Chain(CustomizationSteps.SetLargeIslandTrackWidths).
                        Chain(CustomizationSteps.CommonSteelCustomization).
                        Chain(SetupSteelMesh.Setup19mSteelStationMesh, elevatedInfo, metroStationInfo).
                        Chain(SetupSteelTexture.Setup19mSteelTexture).
                        Chain(
                            (info, version) =>
                            {
                                LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUpSteel.BuildUp(info, version); });
                            }),
                   NetInfoVersion.Ground | NetInfoVersion.Elevated,
                    ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
                        Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(
                            LinkToNonGroundVersions, null, NetInfoVersion.None)
                    , prefabName => "Steel " + prefabName + " Large Dual Island"
                    );
            }
            catch (Exception e)
            {
                Next.Debug.Log("Exception happened when setting up steel large dual island station tracks");
                UnityEngine.Debug.LogException(e);
            }
            try
            {
                CreateFullStationPrefab(
                    ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                        //Chain(CustomizationSteps.SetupStationProps).
                        Chain(CustomizationSteps.CommonIslandCustomization).
                        Chain(CustomizationSteps.CommonSteelCustomization).
                        Chain(CustomizationSteps.SetIslandTrackWidths).
                        Chain(SetupSteelMesh.Setup19mSteelStationMesh, elevatedInfo, metroStationInfo).
                        Chain(SetupSteelTexture.Setup19mSteelTexture),
                   NetInfoVersion.Ground | NetInfoVersion.Elevated,
                    ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
                        Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(
                            LinkToNonGroundVersions, null, NetInfoVersion.None)
                    , prefabName => "Steel " + prefabName + " Island"
                    );
            }
            catch (Exception e)
            {
                Next.Debug.Log("Exception happened when setting up steel large island station tracks");
                UnityEngine.Debug.LogException(e);
            }

            try
            {
                CreateFullStationPrefab(
                    ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                        //Chain(CustomizationSteps.SetupStationProps).
                        Chain(CustomizationSteps.CommonCustomizationSmall).
                        Chain(CustomizationSteps.CommonSteelCustomization).
                        Chain(CustomizationSteps.SetSmallTrackWidths).
                        Chain(SetupSteelMesh.Setup6mStationSteelMesh, elevatedInfo, metroStationInfo).
                        Chain(SetupSteelTexture.Setup6mSteelTexture),
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

            try
            {
                CreateFullStationPrefab(
                    ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                        //Chain(CustomizationSteps.SetupStationProps).
                        Chain(CustomizationSteps.CommonSteelCustomization).
                        Chain(CustomizationSteps.CommonCustomizationLarge).
                        Chain(CustomizationSteps.SetLargeStationTrackWidths).
                        Chain(SetupSteelMesh.Setup10mStationSteelMesh, elevatedInfo).
                        Chain(SetupSteelTexture.Setup10mSteelTexture),
                   NetInfoVersion.Ground | NetInfoVersion.Elevated,
                    ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
                        Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(
                            LinkToNonGroundVersions, null, NetInfoVersion.None)
                    , prefabName => "Steel " + prefabName + " Large"
                    );
            }
            catch (Exception e)
            {
                Next.Debug.Log("Exception happened when setting up large steel station tracks");
                UnityEngine.Debug.LogException(e);
            }
        }
        #endregion

        #region Vanilla
        private void ExtendVanillaTracks()
        {
            ModifyVanillaTrackPrefabs();
            ModifyVanillaStationTrackPrefabs();
        }
        #endregion
        #region StationClones
        private void CreateCloneBuilding(BuildingInfo info)
        {
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                CreateFullStationClone(info, ActionExtensions.BeginChain<BuildingInfo>().
                    Chain(CustomizationSteps.CommonClonedStationCustomization));
                    //Chain(CustomizationSteps.ReplaceBuildingIcon));
                sw.Stop();
                UnityEngine.Debug.Log("Items took " + sw.ElapsedMilliseconds + "ms");
            }
            catch (Exception e)
            {
                Next.Debug.Log("Exception happened when setting up station clone");
                UnityEngine.Debug.LogException(e);
            }
        }


        #endregion
        #region COMMON
        private void ModifyVanillaTrackPrefabs()
        {
            var groundVersion = Prefabs.Find<NetInfo>(Util.GetMetroTrackName(NetInfoVersion.Ground, TrackStyle.Vanilla));
            foreach (var version in new[] { NetInfoVersion.Ground, NetInfoVersion.Elevated, NetInfoVersion.Bridge, NetInfoVersion.Slope, NetInfoVersion.Tunnel  })
            {
                var info = Prefabs.Find<NetInfo>(Util.GetMetroTrackName(version, TrackStyle.Vanilla));
                ReplaceAI(info, version);
                if (version != NetInfoVersion.Ground)
                    CommonSteps.SetVersion(info, groundVersion, version);
                CustomizationSteps.CommonVanillaCustomization(info, version);
                LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUpVanilla.BuildUp(info, version); });
            }
        }

        private void ModifyVanillaStationTrackPrefabs()
        {
            var groundVersion = Prefabs.Find<NetInfo>(Util.GetMetroStationTrackName(NetInfoVersion.Ground, TrackStyle.Vanilla));
            foreach (var version in new[] { NetInfoVersion.Ground, NetInfoVersion.Elevated, NetInfoVersion.Tunnel })
            {
                var info = Prefabs.Find<NetInfo>(Util.GetMetroStationTrackName(version, TrackStyle.Vanilla));
                ReplaceAI(info, version);
                if (version != NetInfoVersion.Ground)
                    CommonSteps.SetVersion(info, groundVersion, version);
                CustomizationSteps.CommonVanillaCustomization(info, version);
                LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUpVanilla.BuildUp(info, version); });
            }
        }

        private void CreateFullStationClone(BuildingInfo info, Action<BuildingInfo> customizationStep)
        {
            CreateStationClone(info, ActionExtensions.BeginChain<BuildingInfo>().
                        Chain(ReplaceBuildingAI).
                        Chain(SetupAnalogStationMeta).
                        Chain(SetupBuildingModel, customizationStep));
        }
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
                groundVersion = CreateNetInfo(nameModifier.Invoke("Metro Track Ground"), Util.GetMetroTrackName(NetInfoVersion.Ground, TrackStyle.Vanilla),
                    ActionExtensions.BeginChain<NetInfo>().
                        Chain(ReplaceAI, NetInfoVersion.Ground).
                        Chain(SetupMetroTrackMeta, NetInfoVersion.Ground).
                        Chain(p =>
                        {
                            //p.GetComponent<MetroTrackAI>().m_elevatedInfo = null;
                            //p.GetComponent<MetroTrackAI>().m_info = null;
                            setupOtherVersionsStep?.Invoke(p, customizationStep);
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
                CreateNetInfo(nameModifier.Invoke("Metro Track " + version), Util.GetMetroTrackName(version, TrackStyle.Vanilla),
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
                CommonSteps.SetVersion(FindCustomNetInfo(nameModifier.Invoke(Util.GetMetroTrackName(version, TrackStyle.Vanilla))), p, version);
            }
        }

        private void CreateFullStationPrefab(Action<NetInfo, NetInfoVersion> customizationStep,
            NetInfoVersion versions,
            Action<NetInfo, Action<NetInfo, NetInfoVersion>> setupOtherVersionsStep,
            Func<string, string> nameModifier = null, Dictionary<NetInfoVersion, string> replacements = null)
        {
            NetInfo groundVersion = null;
            if ((versions & NetInfoVersion.Ground) != 0)
            {
                Debug.Log("Looking at version overground");
                groundVersion = CreateNetInfo(nameModifier.Invoke("Metro Station Track Ground"), Util.GetMetroStationTrackName(NetInfoVersion.Ground, TrackStyle.Vanilla),
                    ActionExtensions.BeginChain<NetInfo>().
                        Chain(ReplaceAI, NetInfoVersion.Ground).
                        Chain(SetupMetroTrackMeta, NetInfoVersion.Ground).
                        Chain(SetupStationTrack, NetInfoVersion.Ground).
                        Chain(p =>
                        {
                            setupOtherVersionsStep?.Invoke(p, customizationStep);
                            //p.GetComponent<MetroTrackAI>().m_elevatedInfo = null;
                            //p.GetComponent<MetroTrackAI>().m_info = null;
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
                Debug.Log("Looking at version " + version.ToString());
                CreateNetInfo(nameModifier.Invoke("Metro Station Track " + version), Util.GetMetroStationTrackName(version, TrackStyle.Vanilla),
                    ActionExtensions.BeginChain<NetInfo>().
                        Chain(ReplaceAI, version).
                        Chain(SetupMetroTrackMeta, version).
                        Chain(SetupStationTrack, version).
                        Chain(CommonSteps.SetVersion, groundVersion, version).
                        Chain(Modifiers.MakePedestrianLanesNarrow, version).
                        Chain(SetupTrackModel, customizationStep.Chain(SetCosts)), replaces
                    );
            }
            CreateNetInfo(nameModifier.Invoke("Metro Station Track Sunken"), "Metro Station Track Overground", //TODO(earalov): test. check if AI to be replaced with MetroTrackAI
                ActionExtensions.BeginChain<NetInfo>().
                    Chain(ReplaceAI, NetInfoVersion.Tunnel).
                    Chain(SetupMetroTrackMeta, NetInfoVersion.Ground).
                    Chain(SetupStationTrack, NetInfoVersion.Ground).
                    Chain(SetupSunkenStationTrack).
                    Chain(SetupTrackModel, customizationStep)
            );
        }

        private static bool m_TrainTrackAnalogAISet = false;
        private static void ReplaceBuildingAI(BuildingInfo prefab)
        {
            if (prefab.name.IndexOf("Train Station" + ModTrackNames.ANALOG_PREFIX) == -1)
            {
                var ai = prefab.GetComponent<PlayerBuildingAI>();
                if (ai != null)
                {
                    var stationAi = ai as TransportStationAI;
                    if (stationAi != null)
                    {
                        var clonedAi = stationAi.CloneBuildingAI(stationAi.name);
                        if (prefab.IsTrainStation())
                        {
                            var metroEntrance = PrefabCollection<BuildingInfo>.FindLoaded("Metro Entrance");
                            clonedAi.m_transportLineInfo = PrefabCollection<NetInfo>.FindLoaded("Metro Line");
                            clonedAi.m_transportInfo = PrefabCollection<TransportInfo>.FindLoaded("Metro");
                            clonedAi.m_maxVehicleCount = 0;
                            //clonedAi.m_createPassMilestone = metroEntrance.GetComponent<PlayerBuildingAI>().m_createPassMilestone;
                        }
                        else if (prefab.IsMetroStation())
                        {
                            var vanillaTrainStation = PrefabCollection<BuildingInfo>.FindLoaded("Train Station");
                            var vanillaVehicleCount = 0;
                            ManualMilestone createPassMilestone = null;
                            if (vanillaTrainStation != null)
                            {
                                var vanillaAi = vanillaTrainStation.GetComponent<TransportStationAI>();
                                if (vanillaAi != null)
                                {
                                    vanillaVehicleCount = vanillaAi.m_maxVehicleCount;
                                    createPassMilestone = vanillaAi.m_createPassMilestone;
                                }
                            }

                            clonedAi.m_transportLineInfo = PrefabCollection<NetInfo>.FindLoaded("Train Line");
                            clonedAi.m_transportInfo = PrefabCollection<TransportInfo>.FindLoaded("Train");
                            clonedAi.m_maxVehicleCount = vanillaVehicleCount;
                            //clonedAi.m_createPassMilestone = createPassMilestone;
                            if (!m_TrainTrackAnalogAISet)
                            {
                                m_TrainTrackAnalogAISet = true;
                                var trainStationAnalog = PrefabCollection<BuildingInfo>.FindLoaded("Train Station" + ModTrackNames.ANALOG_PREFIX + TrackVehicleType.Metro.ToString());
                                var ttsaAI = trainStationAnalog.GetComponent<TransportStationAI>();
                                if (ttsaAI != null)
                                {
                                    var clonedTtsaAi = ttsaAI.CloneBuildingAI(ttsaAI.name);
                                    var metroEntrance = PrefabCollection<BuildingInfo>.FindLoaded("Metro Entrance");
                                    clonedTtsaAi.m_transportLineInfo =PrefabCollection<NetInfo>.FindLoaded("Metro Line");
                                    clonedTtsaAi.m_transportInfo =PrefabCollection<TransportInfo>.FindLoaded("Metro");
                                    clonedTtsaAi.m_maxVehicleCount = 0;
                                    //clonedTtsaAi.m_createPassMilestone = metroEntrance.GetComponent<PlayerBuildingAI>().m_createPassMilestone;
                                    clonedTtsaAi.m_info = trainStationAnalog;
                                    trainStationAnalog.m_buildingAI = clonedTtsaAi;
                                }
                            }
                        }

                        clonedAi.m_info = prefab;
                        prefab.m_buildingAI = clonedAi;
                    }
                }
            }
        }
        private static void ReplaceAI(NetInfo prefab, NetInfoVersion version)
        {
            var originalAi = prefab.GetComponent<PlayerNetAI>(); //milestone, construction and maintenance costs to be overriden later
            var canModify = originalAi.CanModify();
            int noiseAccumulation;
            float noiseRadius;
            originalAi.GetNoiseAccumulation(out noiseAccumulation, out noiseRadius);
            if (originalAi is MetroTrackTunnelAI || version == NetInfoVersion.Tunnel)
            {
                if ((originalAi is MetroTrackTunnelAI && version == NetInfoVersion.Slope))
                {
                    GameObject.DestroyImmediate(originalAi);
                    var ai = prefab.gameObject.AddComponent<MOMMetroTrackTunnelAI>();
                    ai.m_canModify = canModify;
                    ai.m_noiseAccumulation = noiseAccumulation;
                    ai.m_noiseRadius = noiseRadius;
                    ai.m_info = prefab;
                    prefab.m_netAI = ai;
                }
                else
                {
                    GameObject.DestroyImmediate(originalAi);
                    var ai = prefab.gameObject.AddComponent<MOMMetroTrackTunnelAI>();
                    ai.m_info = prefab;
                    //ai.m_transportInfo = PrefabCollection<TransportInfo>.FindLoaded("Train");
                    prefab.m_netAI = ai;
                }

            }
            else if (version == NetInfoVersion.Bridge || version == NetInfoVersion.Elevated)
            {
                GameObject.DestroyImmediate(originalAi);
                var ai = prefab.gameObject.AddComponent<MOMMetroTrackBridgeAI>();
                ai.m_canModify = canModify;
                ai.m_noiseAccumulation = noiseAccumulation;
                ai.m_noiseRadius = noiseRadius;
                ai.m_info = prefab;
                prefab.m_netAI = ai;
            }
            else
            {
                GameObject.DestroyImmediate(originalAi);
                var ai = prefab.gameObject.AddComponent<MOMMetroTrackAI>();
                ai.m_noiseAccumulation = noiseAccumulation;
                ai.m_noiseRadius = noiseRadius;
                ai.m_info = prefab;
                prefab.m_netAI = ai;
            }
        }

        public static void SetupSunkenStationTrack(NetInfo prefab)
        {
            prefab.m_maxHeight = -1;
            prefab.m_minHeight = -3;
            prefab.m_lowerTerrain = false;
            prefab.m_class.m_layer = ItemClass.Layer.MetroTunnels | ItemClass.Layer.Default;
        }

        public static void SetupStationTrack(NetInfo prefab, NetInfoVersion version)
        {
            prefab.m_followTerrain = false;
            prefab.m_flattenTerrain = version == NetInfoVersion.Ground && !prefab.name.Contains("Sunken");
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
                //prefab.m_maxHeight = 0.5f;
            }
            else
            {
                prefab.m_clipTerrain = false;
                if (version == NetInfoVersion.Elevated)
                {
                    //prefab.m_maxHeight = 0.5f;
                }
                else if (version == NetInfoVersion.Tunnel)
                {
                    prefab.m_maxHeight = -1;
                    prefab.m_minHeight = -5;
                    prefab.m_lowerTerrain = false;
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

        private static void SetupBuildingModel(BuildingInfo prefab, Action<BuildingInfo> customizationStep)
        {
            customizationStep.Invoke(prefab);
        }

        private static void SetupAnalogStationMeta(BuildingInfo prefab)
        {
            var newClass = ScriptableObject.CreateInstance<ItemClass>();
            if (prefab.m_class.m_subService == ItemClass.SubService.PublicTransportTrain)
            {
                newClass.m_subService = ItemClass.SubService.PublicTransportMetro;
                prefab.SetUICategory("PublicTransportMetro");
            }
            else if (prefab.m_class.m_subService == ItemClass.SubService.PublicTransportMetro)
            {
                newClass.m_subService = ItemClass.SubService.PublicTransportTrain;
                prefab.SetUICategory("PublicTransportTrain");
            }
            newClass.m_service = ItemClass.Service.PublicTransport;
            newClass.m_layer = ItemClass.Layer.Default;
            newClass.name = prefab.m_class.name + "_Analog";
            newClass.hideFlags = HideFlags.None;
            prefab.m_class = newClass;
        }
        private static void SetupMetroTrackMeta(NetInfo prefab, NetInfoVersion version)
        {
            var vanillaMetroTrack = FindOriginalNetInfo(Util.GetMetroTrackName(NetInfoVersion.Tunnel, TrackStyle.Vanilla));
            var vanillaTrainTrack = FindOriginalNetInfo("Train Track");
            if (!prefab.name.Contains("Station"))
            {
                var milestone = vanillaMetroTrack.GetComponent<PlayerNetAI>().m_createPassMilestone;
                prefab.GetComponent<PlayerNetAI>().m_createPassMilestone = milestone;
                prefab.m_minCornerOffset = 18;
            }

            //prefab.m_connectionClass = vanillaTrainTrack.m_class;
            prefab.m_class = ScriptableObject.CreateInstance<ItemClass>();
            prefab.m_class.m_subService = ItemClass.SubService.PublicTransportMetro;
            //prefab.m_class.m_layer = ItemClass.Layer.MetroTunnels;
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
                prefab.m_class.m_layer = ItemClass.Layer.Default;// | ItemClass.Layer.MetroTunnels;
            }

            prefab.m_createGravel = version == NetInfoVersion.Ground;
            prefab.m_availableIn = ItemClass.Availability.AssetEditor;
            prefab.m_class.hideFlags = HideFlags.None;
            prefab.m_class.name = prefab.name;
            prefab.m_maxBuildAngle = 60;
            prefab.m_maxTurnAngleCos = Mathf.Cos(prefab.m_maxBuildAngle);
            prefab.m_maxTurnAngle = 60;
            prefab.m_maxTurnAngleCos = Mathf.Cos(prefab.m_maxTurnAngle);
            prefab.m_averageVehicleLaneSpeed = vanillaMetroTrack.m_averageVehicleLaneSpeed;
            prefab.m_UnlockMilestone = vanillaMetroTrack.m_UnlockMilestone;
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
                NetInfo metroTrackInfo = null;
                if (newPrefab.name.Contains("Station"))
                {
                    metroTrackInfo = FindOriginalNetInfo(Util.GetMetroStationTrackName(version, TrackStyle.Vanilla));
                }
                else
                {
                    metroTrackInfo = FindOriginalNetInfo(Util.GetMetroTrackName(version, TrackStyle.Vanilla));
                }
                
                var baseConstructionCost = metroTrackInfo.GetComponent<PlayerNetAI>().m_constructionCost;
                var baseMaintenanceCost = metroTrackInfo.GetComponent<PlayerNetAI>().m_maintenanceCost;
                var newAi = newPrefab.GetComponent<PlayerNetAI>();

                var multiplier = GetCostMultiplier(newPrefab, version);
                newAi.m_constructionCost = (int)(baseConstructionCost * multiplier);
                newAi.m_maintenanceCost = (int)(baseMaintenanceCost * multiplier);
            }
        }

        public static float GetCostMultiplier(PrefabInfo newPrefab, NetInfoVersion version)
        {
            if (newPrefab.name.Contains("Small"))
            {
                return 2f / 3f;
            }
            else if (newPrefab.name.Contains("Large"))
            {
                return 1.5f;
            }
            return 1;
        }
        #endregion
    }
}
