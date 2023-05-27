using ColossalFramework;
using UnityEngine;

namespace MetroOverhaul.Extensions {
    public static class NetToolExtensions
    {
        public static void RenderNodeBuilding2(this NetTool tool, BuildingInfo info, Vector3 position, Vector3 direction)
        {
            //if (!Singleton<ToolManager>.instance.m_properties.IsInsideUI)
            //{
            //    if (direction.sqrMagnitude < 0.5f)
            //    {
            //        return;
            //    }
            //    if (info.m_mesh == null)
            //    {
            //        return;
            //    }
            //    direction.y = 0f;
            //    direction.x += 5;
            //    Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
            //    Building building = default(Building);
            //    BuildingManager instance = Singleton<BuildingManager>.instance;
            //    instance.m_materialBlock.Clear();
            //    instance.m_materialBlock.SetVector(instance.ID_BuildingState, new Vector4(0f, 1000f, 0f, 256f));
            //    instance.m_materialBlock.SetVector(instance.ID_ObjectIndex, RenderManager.DefaultColorLocation);
            //    instance.m_materialBlock.SetColor(instance.ID_Color, info.m_buildingAI.GetColor(0, ref building, Singleton<InfoManager>.instance.CurrentMode));
            //    ToolManager expr_DC_cp_0 = Singleton<ToolManager>.instance;
            //    expr_DC_cp_0.m_drawCallData.m_defaultCalls = expr_DC_cp_0.m_drawCallData.m_defaultCalls + 1;
            //    Graphics.DrawMesh(info.m_mesh, position, rotation, info.m_material, info.m_prefabDataLayer, null, 0, instance.m_materialBlock);
            //}
        }
    }
}
