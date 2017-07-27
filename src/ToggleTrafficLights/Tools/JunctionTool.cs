using System;
using System.Diagnostics;
using System.Text;
using ColossalFramework;
using ColossalFramework.Math;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Tools
{
  /// <summary>
  /// Isolated functionality to toggle traffic lights and stop signs.
  /// From RoadBaseAI.RenderNode, RoadBaseAI.ClickNodeButton and similar functions and variables
  /// RoadBaseAi always checks for <see cref="InfoManager.CurrentMode"/> and <see cref="InfoManager.CurrentSubMode"/>,
  /// But <see cref="JunctionTool"/> can be used without setting these modes.
  /// </summary>
  public class JunctionTool : ToolBase
  {
    protected override void OnEnable()
    {
      DebugLog.Info($"Tunnels: {TransportManager.instance.TunnelsVisible}");

      base.OnEnable();

      ForceElevation(CurrentElevation);

      OnEnabled();
      DebugLog.Info($"{nameof(JunctionTool)} enabled");
    }

    protected override void OnDisable()
    {
      base.OnDisable();

      if (CurrentElevation >= Elevation.Overground)
      {
        TransportManager.instance.TunnelsVisible = false;
      }

      OnDisabled();
    }

    protected override void OnToolGUI(Event e)
    {
      base.OnToolGUI(e);

      ShowTooltip();
      HandleClick(e);
      HandleElevation(e);
    }

    private Ray _mouseRay;
    private float _mouseRayLength;
    private Vector3 _rayRight;
    private bool _mouseRayValid;
    private Camera _camera = null;
    protected override void OnToolLateUpdate()
    {
      base.OnToolLateUpdate();

      if (_camera == null)
      {
        _camera = Camera.main;  // Camera.main searches every call for the main camera -> expensive
      }
      this._mouseRay = _camera.ScreenPointToRay(Input.mousePosition);
      this._mouseRayLength = _camera.farClipPlane;
      this._rayRight = _camera.transform.TransformDirection(Vector3.right);
      this._mouseRayValid = !m_toolController.IsInsideUI && Cursor.visible;
    }

    public override void SimulationStep()
    {
      base.SimulationStep();

      DetectHoveredElements();
    }

    public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
    {
      base.RenderOverlay(cameraInfo);

      RenderIntersections(cameraInfo);
    }

    private Vector3 _mousePosition = Vector3.zero;

    public Vector3 MousePosition => _mousePosition;

    // Node is enough get junctions and its connections
    private ushort _hoveredNodeId = 0;

    public ushort HoveredNodeId => _hoveredNodeId;
    private int _hoveredButtonIndex;


    private void DetectHoveredElements()
    {
      _hoveredNodeId = 0;
      if (!m_toolController.IsInsideUI && Cursor.visible)
      {
        var netService = new RaycastService
                              {
                                m_service = ItemClass.Service.None,
                                m_subService = ItemClass.SubService.None,
                                m_itemLayers = ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels,
                              };
        var input = new RaycastInput(_mouseRay, _mouseRayLength)
        {
          m_rayRight = _rayRight,
          m_netService = netService,
          m_buildingService = netService,
          m_propService = netService,
          m_treeService = netService,
          m_districtNameOnly = true,
          m_ignoreTerrain = true,
          m_ignoreNodeFlags = IsUndergroundVisible() ? NetNode.Flags.None : (NetNode.Flags.None | NetNode.Flags.Underground),
        };

        if (RayCast(input, out var output))
        {
          _mousePosition = output.m_hitPos;

          var id = output.m_netNode;
          if (id != 0)
          {
            var node = Singleton<NetManager>.instance.m_nodes.m_buffer[id];
            if (!GameAreaManager.instance.PointOutOfArea(node.m_position))
            {
              _hoveredNodeId = id;
            }
            _hoveredButtonIndex = DetectHoveredButtons(input, id, ref node);
          }
          else
          {
            _hoveredButtonIndex = 0;
          }
        }
      }
    }

    /// <summary>
    /// -1: Traffic Light
    ///  0: Nothing
    /// >0: Stop Sign
    /// </summary>
    private int DetectHoveredButtons(RaycastInput input, ushort nodeId, ref NetNode node)
    {
      //Source: RoadBaseAI.RayCastNodeButton
      Vector3 origin = input.m_ray.origin;
      Vector3 normalized = input.m_ray.direction.normalized;
      Vector3 b = input.m_ray.origin + normalized * input.m_length;
      Segment3 ray = new Segment3(origin, b);

      var f = Vector3.Distance(ray.a, node.m_position);
      if (f < 1000.0f)
      {
        bool flag1 = (node.m_flags & (NetNode.Flags.TrafficLights | NetNode.Flags.OneWayIn)) == NetNode.Flags.None;
        bool flag2 = Node.CanHaveTrafficLights(nodeId, ref node);
        float num1 = 0.0675f * Mathf.Pow(f, 0.65f);
        if (flag2 && (double) ray.DistanceSqr(node.m_position) < (double) num1 * (double) num1)
        {
          return -1;
        }
        if (flag1)
        {
          var instance = Singleton<NetManager>.instance;
          var num2 = node.Info.m_halfWidth * 0.75f;
          for (ushort index = 0; index < 8; ++index)
          {
            var segment = node.GetSegment(index);
            if (segment != 0)
            {
              var info = instance.m_segments.m_buffer[segment].Info;
              if ((info.m_vehicleTypes & VehicleInfo.VehicleType.Car) != VehicleInfo.VehicleType.None)
              {
                bool flag3 = instance.m_segments.m_buffer[segment].m_startNode == nodeId;
                bool flag4 = (instance.m_segments.m_buffer[segment].m_flags & NetSegment.Flags.Invert) !=
                             NetSegment.Flags.None;
                if ((flag3 != flag4
                      ? (info.m_hasBackwardVehicleLanes ? 1 : 0)
                      : (info.m_hasForwardVehicleLanes ? 1 : 0)) != 0)
                {
                  Vector3 vector3 = !flag3
                    ? instance.m_segments.m_buffer[segment].m_endDirection
                    : instance.m_segments.m_buffer[segment].m_startDirection;
                  Vector3 p = node.m_position + vector3 * num2;
                  if ((double) ray.DistanceSqr(p) < (double) num1 * (double) num1)
                  {
                    return index + 1;
                  }
                }
              }
            }
          }
        }
      }
      return 0;
    }

    private void RenderIntersections(RenderManager.CameraInfo cameraInfo)
    {
      var renderManager = Singleton<RenderManager>.instance;
      var netManager = Singleton<NetManager>.instance;
      FastList<RenderGroup> renderedGroups = renderManager.m_renderedGroups;


      // This iterates over all visible nodes
      // source: NetManager.EndRenderImpl
      //
      // Iterates over all visible RenderGroups that contain nodes.
      // Because of the groups not all detected nodes are visible (in fact quit often most aren't visible)
      // Therefore further tests with CameraInfo.Intersect.
      // But CameraInfo.Intersect still is quite imprecise and detects a lot of nodes outside the visible area.
      for (var i = 0; i < renderedGroups.m_size; i++)
      {
        var renderGroup = renderedGroups.m_buffer[i];
        if (renderGroup.m_instanceMask == 0)
        {
          continue;
        }

        int num1 = renderGroup.m_x * 270 / 45;
        int num2 = renderGroup.m_z * 270 / 45;
        int num3 = (renderGroup.m_x + 1) * 270 / 45 - 1;
        int num4 = (renderGroup.m_z + 1) * 270 / 45 - 1;
        for (int index2 = num2; index2 <= num4; ++index2)
        {
          for (int index3 = num1; index3 <= num3; ++index3)
          {
            int count = 0;
            var nodeId = netManager.m_nodeGrid[index2 * 270 + index3];
            while (nodeId != 0)
            {
              var node = netManager.m_nodes.m_buffer[nodeId];

              if (cameraInfo.Intersect(node.m_position, 0f))
              {
                RenderIntersection(nodeId, ref node, cameraInfo);
              }

              if (++count >= 32768)
              {
                CODebugBase<LogChannel>.Error(LogChannel.Core,
                  "Invalid list detected!\n" + System.Environment.StackTrace);
                break;
              }
              nodeId = netManager.m_nodes.m_buffer[nodeId].m_nextGridNode;
            }
          }
        }
      }
    }

    private void RenderIntersection(ushort nodeId, ref NetNode node, RenderManager.CameraInfo cameraInfo)
    {
      if (Node.CanHaveTrafficLights(nodeId, ref node))
      {
        // don't render if underground but underground mode not active
        if (CurrentElevation == Elevation.Overground && node.Info.m_netAI.IsUnderground())
        {
          return;
        }

        RenderTrafficLights(nodeId, ref node, cameraInfo);
        RenderStopSigns(nodeId, ref node, cameraInfo);
      }
    }

    private void RenderTrafficLights(ushort nodeId, ref NetNode node, RenderManager.CameraInfo cameraInfo)
    {
      var pos = node.m_position;
      var alpha = MathUtils.SmoothStep(1000f, 500f, Vector3.Distance(cameraInfo.m_position, pos));
      var scale = _hoveredNodeId == nodeId && _hoveredButtonIndex == -1 ? 1.0f : 0.75f;
      var lights = (node.m_flags & NetNode.Flags.TrafficLights) != NetNode.Flags.None
        ? NotificationEvent.Type.TrafficLightsOn
        : NotificationEvent.Type.TrafficLightsOff;
      NotificationEvent.RenderInstance(cameraInfo, lights, pos, scale, alpha);
    }

    private void RenderStopSigns(ushort nodeId, ref NetNode node, RenderManager.CameraInfo cameraInfo)
    {
      //only display stop signs if no traffic lights
      if ((node.m_flags & NetNode.Flags.TrafficLights) != NetNode.Flags.None)
      {
        return;
      }

      var nm = Singleton<NetManager>.instance;
      var num = node.Info.m_halfWidth * 0.75f;
      var alpha = MathUtils.SmoothStep(1000f, 500f, Vector3.Distance(cameraInfo.m_position, node.m_position));
      for (var index = 0; index < 8; ++index)
      {
        var segment = node.GetSegment(index);
        if (segment != 0)
        {
          var info = nm.m_segments.m_buffer[segment].Info;
          if ((info.m_vehicleTypes & VehicleInfo.VehicleType.Car) != VehicleInfo.VehicleType.None)
          {
            var flag3 = nm.m_segments.m_buffer[segment].m_startNode == nodeId;
            var flag4 = (nm.m_segments.m_buffer[segment].m_flags & NetSegment.Flags.Invert) != NetSegment.Flags.None;
            if ((flag3 != flag4 ? (info.m_hasBackwardVehicleLanes ? 1 : 0) : (info.m_hasForwardVehicleLanes ? 1 : 0)) !=
                0)
            {
              var scale = _hoveredNodeId == nodeId && _hoveredButtonIndex == index + 1 ? 1.0f : 0.75f;
              var vector3 = !flag3
                ? nm.m_segments.m_buffer[segment].m_endDirection
                : nm.m_segments.m_buffer[segment].m_startDirection;
              var position = node.m_position + vector3 * num;
              var flags = !flag3 ? NetSegment.Flags.YieldEnd : NetSegment.Flags.YieldStart;
              var sign = (nm.m_segments.m_buffer[segment].m_flags & flags) != NetSegment.Flags.None
                ? NotificationEvent.Type.YieldOn
                : NotificationEvent.Type.YieldOff;
              NotificationEvent.RenderInstance(cameraInfo, sign, position, scale, alpha);
            }
          }
        }
      }
    }

    private readonly StringBuilder _tooltip = new StringBuilder();

    [Conditional("DEBUG")]
    private void ShowTooltip()
    {
      if (_hoveredNodeId == 0 || m_toolController.IsInsideUI || !Cursor.visible)
      {
        ShowToolInfo(false, null, Vector3.zero);
      }
      else
      {
        var tooltip = _tooltip.Clear();

        var nodeId = _hoveredNodeId;
        var node = Singleton<NetManager>.instance.m_nodes.m_buffer[nodeId];

        tooltip.AppendNameValueLine("Id", nodeId);
        tooltip.AppendNameValueLine("Position", node.m_position);
        tooltip.AppendNameValueLine("Flags", node.m_flags);
        tooltip.AppendNameValueLine("Service", node.Info.GetService());
        tooltip.AppendNameValueLine("ValidTrafficIntersection", Node.IsValidIntersection(nodeId, ref node));
        tooltip.AppendNameValueLine("CanHaveTrafficLights", Node.CanHaveTrafficLights(nodeId, ref node));
        tooltip.AppendNameValueLine("WantLights", node.Info.m_netAI.WantTrafficLights());
        tooltip.AppendNameValueLine("OverlayButtonIndex", _hoveredButtonIndex);

        ShowToolInfo(true, tooltip.ToString(), _mousePosition);
        tooltip.Clear();
      }
    }

    private void HandleClick(Event e)
    {
      if (_hoveredNodeId != 0
          && _hoveredButtonIndex != 0
          && e.type == EventType.MouseDown
          && e.button == 0 // left click
          && !m_toolController.IsInsideUI
          && Cursor.visible)
      {
        var node = Singleton<NetManager>.instance.m_nodes.m_buffer[_hoveredNodeId];
        if (_hoveredButtonIndex == -1)
        {
          Node.TrafficLights.Toggle(_hoveredNodeId, ref node);
        }
        else
        {
          Debug.Assert(_hoveredButtonIndex > 0);
          Node.StopSign.Toggle(_hoveredNodeId, ref node, (ushort) (_hoveredButtonIndex - 1));
        }
      }
    }

    #region Elevation

    private readonly SavedInputKey _buildElevationUp = new SavedInputKey(Settings.buildElevationUp,
      Settings.gameSettingsFile, DefaultSettings.buildElevationUp, true);

    private readonly SavedInputKey _buildElevationDown = new SavedInputKey(Settings.buildElevationDown,
      Settings.gameSettingsFile, DefaultSettings.buildElevationDown, true);

    private void HandleElevation(Event e)
    {
      var current = Event.current;
      if (current == null || !current.isKey || current.keyCode == KeyCode.None || UIView.HasModalInput() ||
          UIView.HasInputFocus())
      {
        return;
      }

      if (_buildElevationUp.IsPressed(e))
      {
        ChangeElevation(true);
      }
      if (_buildElevationDown.IsPressed(e))
      {
        ChangeElevation(false);
      }
    }

    private Elevation _currentElevation = Elevation.OvergroundWithTunnels;

    public Elevation CurrentElevation
    {
      get => _currentElevation;
      set
      {
        if (value != _currentElevation)
        {
          var prev = _currentElevation;
          _currentElevation = value;
          OnElevationChanged(prev, value);
        }
      }
    }

    private void OnElevationChanged(Elevation oldElevation, Elevation newElevation)
    {
      DebugLog.Info($"Change elevation from {oldElevation} to {newElevation}");
      ForceElevation(newElevation);
    }

    private void ForceElevation(Elevation elevation)
    {
      switch (elevation)
      {
        case Elevation.Overground:
          InfoManager.instance.SetCurrentMode(InfoManager.InfoMode.None, InfoManager.SubInfoMode.Default);
          TransportManager.instance.TunnelsVisible = false;
          break;
        case Elevation.OvergroundWithTunnels:
          InfoManager.instance.SetCurrentMode(InfoManager.InfoMode.None, InfoManager.SubInfoMode.Default);
          TransportManager.instance.TunnelsVisible = true;
          break;
        case Elevation.Underground:
          InfoManager.instance.SetCurrentMode(InfoManager.InfoMode.None, InfoManager.SubInfoMode.Default);
          //todo: extract into delegate
          InfoManager.instance.CallNonPublicMethod("SetMode", InfoManager.InfoMode.TrafficRoutes,
            InfoManager.SubInfoMode.JunctionSettings);
          TransportManager.instance.TunnelsVisible = true;
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(elevation), elevation, null);
      }
    }

    private void ChangeElevation(bool increase)
    {
      var ele = (int) _currentElevation + (increase ? -1 : 1);
      ele = Math.Max(0, Math.Min(2, ele));
      CurrentElevation = (Elevation) ele;
    }

    public enum Elevation
    {
      Overground = 0,
      OvergroundWithTunnels = 1,
      Underground = 2,
    }

    public bool IsUndergroundVisible() => CurrentElevation != Elevation.Overground;

    #endregion Elevation

    #region Events

    public event Action Disabled;

    protected virtual void OnDisabled()
    {
      Disabled?.Invoke();
    }

    public event Action Enabled;

    protected virtual void OnEnabled()
    {
      Enabled?.Invoke();
    }

    #endregion Events
  }
}
