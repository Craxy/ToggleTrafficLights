namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils
{
  public static class CitiesHelper
  {
    public static bool HasTrafficLights(NetNode.Flags flags) 
      => (flags & NetNode.Flags.TrafficLights) == NetNode.Flags.TrafficLights;

    public static bool IsCustomTrafficLights(NetNode.Flags flags) 
      => (flags & NetNode.Flags.CustomTrafficLights) == NetNode.Flags.CustomTrafficLights;

    public static void ClickOnRoadsButton()
    {
      //open/close road panel
      //Source: KeyShortcuts.SelectUIButton
      //I want all to enjoy the developers comment/log at the end of the SelectUIButton:
      //"SelectUIButton() was terminated to prevent an infinite loop. This might be some kind of bug... :D"
      //...
      var tutorialUiTag = (TutorialUITag) MonoTutorialTag.Find("Roads");
      tutorialUiTag.target.SimulateClick();
    }
  }
}
