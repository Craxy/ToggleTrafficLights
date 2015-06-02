﻿using ColossalFramework;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Tools
{
    public class DefaultToolWithNetNodeDetection : DefaultTool
    {
        public override void SimulationStep()
        {
            base.SimulationStep();

            if (m_mouseRayValid && GetNodeIgnoreFlags() != NetNode.Flags.All)
            {
//                var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
//                var mouseRayLength = Camera.main.farClipPlane;
//                var rayRight = Camera.main.transform.TransformDirection(Vector3.right);
//
//                var defaultService = new ToolBase.RaycastService(ItemClass.Service.Road, ItemClass.SubService.None, ItemClass.Layer.Default);
//                var input = new ToolBase.RaycastInput(mouseRay, mouseRayLength)
//                {
//                    m_rayRight = rayRight,
//                    m_netService = defaultService,
//                    m_ignoreNodeFlags = NetNode.Flags.None,
//                    m_ignoreSegmentFlags = NetSegment.Flags.Untouchable,
//                };
//                RaycastOutput output;
//                if (!RayCast(input, out output))
//                {
//                    var id = m_hoverInstance;
//                    id.NetNode = 0;
//                    m_hoverInstance = id;
//                    return;
//                }
//
//                {
//                    var id = m_hoverInstance;
//                    id.NetNode = output.m_netNode;
//                    m_hoverInstance = id;
//                }



                var defaultService = new ToolBase.RaycastService(ItemClass.Service.Road, ItemClass.SubService.None, GetNodeLayerIncludeFlags());
                var input = new ToolBase.RaycastInput(m_mouseRay, m_mouseRayLength)
                {
                    m_rayRight = m_rayRight,
                    m_netService = defaultService,
                    m_ignoreNodeFlags = GetNodeIgnoreFlags(),
                    m_ignoreSegmentFlags = NetSegment.Flags.Untouchable,    //provides a MUCH better node recognition -- far mor generous
                    //other flags and services unnecessary -- I'm only interested in NetNodes
                };
                RaycastOutput output;
                if (RayCast(input, out output))
                {
                    if (output.m_netNode != 0)
                    {
                        if (CheckNode(output.m_netNode, ref m_selectErrors))
                        {
                            //check IncludeFlags
                            var node = Singleton<NetManager>.instance.m_nodes.m_buffer[output.m_netNode];

                            if ((node.m_flags & GetNodeIncludeFlags()) != 0)
                            {
                                output.m_hitPos = node.m_position;
                            }
                            else
                            {
                                output.m_netNode = 0;
                            }
                        }
                        else
                        {
                            output.m_netNode = 0;
                        }

                        var id = m_hoverInstance;
                        id.NetNode = output.m_netNode;
                        m_hoverInstance = id;
                    }
                }
                else
                {
                    var id = m_hoverInstance;
                    id.NetNode = 0;
                    m_hoverInstance = id;
                }
            }
        }

        protected override bool CheckNode(ushort node, ref ToolErrors errors)
        {
//            //only works in editors
//            return base.CheckNode(node, ref errors);

            return !Singleton<GameAreaManager>.instance.PointOutOfArea(Singleton<NetManager>.instance.m_nodes.m_buffer[node].m_position);
        }

        /// <summary>
        /// Will be applied AFTER <see cref="DefaultTool.GetNodeIgnoreFlags"/>
        /// Ignore flags ignores node if node contains this flag. Most nodes have mutliple flags, but get ignored when they have just one ignore flag
        /// Example: most junctions are created and therefore have flags=Created|Junction. Having IgnoreFlags=~Junctions will therefore find no junctions.
        /// Therefore best use IgnoreFlags=None and IncludeFlags the ones you want
        /// 
        /// A node must satisfy at least one of the flags
        /// </summary>
        public virtual NetNode.Flags GetNodeIncludeFlags()
        {
            return NetNode.Flags.All;
        }

        public virtual ItemClass.Layer GetNodeLayerIncludeFlags()
        {
            return ItemClass.Layer.Default;
        }
    }
}