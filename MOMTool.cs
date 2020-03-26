using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MetroOverhaul
{
    public class MOMTool : BuildingTool
    {
        public static MOMTool instance;
        private ushort m_HoveredBuilding;
        private ushort m_HoveredSegment;
        private ushort m_HoveredNode;

        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            base.RenderOverlay(cameraInfo);
            //if (m_hoverInstance.NetNode == 0)
            //{
            //    return;
            //}

            //var node = NetManager.instance.m_nodes.m_buffer[m_hoverInstance.NetNode];
            //RenderManager.instance.OverlayEffect.DrawCircle(
            //    cameraInfo,
            //    GetToolColor(false, m_selectErrors != ToolErrors.None),
            //    node.m_position,
            //    15f,
            //    node.m_position.y - 1f,
            //    node.m_position.y + 1f,
            //    true,
            //    true);
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            m_toolController.CurrentTool = this;
        }
        public override void SimulationStep()
        {
            base.SimulationStep();
            if (!enabled)
            {
                return;
            }
            
            //if (!RayCastSegmentAndNode(out var hoveredSegment, out var hoveredNode))
            //    return;
            //var segments = new Dictionary<ushort, SegmentAndNode>();

            //if (hoveredSegment != 0)
            //{
            //    var segment = NetManager.instance.m_segments.m_buffer[hoveredSegment];
            //    var startNode = NetManager.instance.m_nodes.m_buffer[segment.m_startNode];
            //    var endNode = NetManager.instance.m_nodes.m_buffer[segment.m_endNode];
            //    var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            //    if (startNode.CountSegments() > 0)
            //    {
            //        var bounds = startNode.m_bounds;
            //        if (hoveredNode != 0)
            //        {
            //            bounds.extents /= 2f;
            //        }

            //        if (bounds.IntersectRay(mouseRay))
            //        {
            //            hoveredNode = segment.m_startNode;
            //        }
            //    }

            //    if (hoveredSegment != 0 && endNode.CountSegments() > 0)
            //    {
            //        var bounds = endNode.m_bounds;
            //        if (hoveredNode != 0)
            //        {
            //            bounds.extents /= 2f;
            //        }

            //        if (bounds.IntersectRay(mouseRay))
            //        {
            //            hoveredNode = segment.m_endNode;
            //        }
            //    }
            //}

            //m_HoveredBuilding = m_hoverInstance.Building;

            //if (hoveredNode > 0)
            //{
            //    m_hoverInstance.NetNode = hoveredNode;
            //}
            //else if (hoveredSegment > 0)
            //{
            //    m_hoverInstance.NetSegment = hoveredSegment;
            //}

            //m_HoveredSegment = hoveredSegment > 0 ? hoveredSegment : m_hoverInstance.NetSegment;
        }
        //private static bool RayCastSegmentAndNode(out ushort netSegment, out ushort netNode)
        //{
        //    if (RayCastSegmentAndNode(out var output))
        //    {
        //        netSegment = output.m_netSegment;
        //        netNode = output.m_netNode;
        //        return true;
        //    }

        //    netSegment = 0;
        //    netNode = 0;
        //    return false;
        //}

        //private static bool RayCastSegmentAndNode(out RaycastOutput output)
        //{
        //    var input = new RaycastInput(Camera.main.ScreenPointToRay(Input.mousePosition), Camera.main.farClipPlane)
        //    {
        //        m_netService = { m_itemLayers = ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels },
        //        m_ignoreSegmentFlags = NetSegment.Flags.None,
        //        m_ignoreNodeFlags = NetNode.Flags.None,
        //        m_ignoreTerrain = true,
        //    };

        //    return RayCast(input, out output);
        //}

        //private struct SegmentAndNode
        //{
        //    public ushort SegmentId;
        //    public ushort TargetNode;
        //}
    }
}
