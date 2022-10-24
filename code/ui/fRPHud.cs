using Sandbox;
using Sandbox.UI;
using fRP.UI;

[Library]
public partial class fRPHud : HudEntity<RootPanel>
{
	public fRPHud()
	{
		if ( !IsClient )
			return;

		RootPanel.StyleSheet.Load( "/ui/SandboxHud.scss" );

		RootPanel.AddChild<ChatBox>();
		RootPanel.AddChild<VoiceList>();
		RootPanel.AddChild<VoiceSpeaker>();
		RootPanel.AddChild<Scoreboard<ScoreboardEntry>>();
		RootPanel.AddChild<Crosshair>();
		RootPanel.AddChild<UserPanel>();
	}
}