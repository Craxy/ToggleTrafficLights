Toggle Traffic Lights
=====================
Mod for Cities:Skylines
-> [on Steam Workshop](http://steamcommunity.com/sharedfiles/filedetails/?id=411833858)

Tool to **remove or add traffic lights at intersection**. Traffic lights can be added and removed at any kind of road intersection including highways and their off- and on-ramps.
![Same intersection with and without traffic lights](./docs/files/img/TrafficLightsVsNoTrafficLights.png)

In game the tool can be activate via **Ctrl+T**. When you hover over an intersection a ToolTip will show the current Traffic Light status (has traffic light or not). Via a left mouse click you can toggle between traffic lights and no traffic lights. To disable this tool simply select another tool.


**Note**: The existence of traffic lights will be added to new savegames (the savegame size increases by a couple of kB (~33kB) -- I think that's negligible). The savegame can be loaded without the mod enabled but in this case the traffic lights will be reset to it's original statuses (see [this table](https://www.reddit.com/r/CitiesSkylines/comments/2zp61z/i_made_a_table_chart_of_which_intersections/) for the default statuses).

Note: If you append or remove a road to/from an intersection the traffic lights will be recalculated by the game and placed according to the used road types. You must then retoggle the traffic lights via this tool.

Note: The tool is now disabled in the editors. It does not have to be disable in the editors anymore.

## Known Issues & Further Work
This mod is currently a very early version! Toggling the traffic lights works already but there are some issues and further work:
See: https://github.com/Craxy/ToggleTrafficLights/issues
* There is currently no button to activate this tool. You need to use Ctrl+T. A button below the road options is planned.
* On some intersections it's quite hard to find the right point to click (especially on on- and off-ramps). I'm using the integrated detection of NetNode, so the detection will most likely remain the way it is. Just fiddle around a bit until the ToolTip shows up.
* The current ToolTip to indicate an intersection is more or less an interim solution. For later version it's planned to use the same highlighting as the usual selection or bulldozer tools.

* You can toggle traffic lights on every NetNode with an intersection -- i.e. not only intersections between car roads but also on pedestrian paths and railway crossings between trains (both without any visual effect). I have not checked the effect on these kind of intersection. It's planned for further version to disable the toggling on pedestrian and train roads.
* Traffic lights at railway crossings between railway and roads with cars can be added and removed too. I have not inspected the resulting effect. If nobody identifies some use it will most likely be removed in the future.
* It's currently possible to add traffic lights at intersections even if it doesn't make sense -- like just one ingoing road and the remaining ones are just outgoing. The idea for further version is to only allow traffic lights if there are at least two incoming roads present. However this is of very low priority and just might or might not be implemented. At those junctions traffic lights will be most likely always be green, so there is no harm in keeping this.


## Release Notes
### 0.1.0 (2015-03-22)
* Initial Release

### 0.2.0 (2015-03-23)
* Ctrl+T now toggle the tool on/of (returns to the previous selected tool)
* Disabled tool inside the editors
* Traffic lights outside the buildable area cannot be toggled anymore
