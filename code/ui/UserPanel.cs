// using Sandbox.UI.Construct;
// using Sandbox.UI;
// namespace fRP.UI
// {
//     public partial class UserPanel : Panel
//     {
//          static UserPanel Current;

//         public Panel HealthPanel;
//         public Panel HealthIcon;
//         public Panel HungerPanel;
//         public Panel HungerIcon;
//         public Panel ThirstPanel;
//         public Panel ThirstIcon;
//         public Panel IllnessPanel;
//         public Panel IllnessIcon;
//         public Panel BleedingPanel;
//         public Panel BleedingIcon;

//         public UserPanel()
//         {
//             Current = this;
//             StyleSheet.Load( "/ui/UserPanel.scss" );

//             IllnessPanel = Add.Panel( "illness_panel" );
//             HealthPanel = Add.Panel( "health_panel" );
//             // HungerPanel = Add.Panel( "hunger_panel" );
//             // ThirstPanel = Add.Panel( "thirst_panel" );
//             BleedingPanel = Add.Panel( "bleeding_panel" );

//             IllnessIcon = IllnessPanel.Add.Panel( "illness_icon" );
//             HealthIcon = HealthPanel.Add.Panel( "health_icon" );
//             // HungerIcon = HungerPanel.Add.Panel( "hunger_icon" );
//             // ThirstIcon = ThirstPanel.Add.Panel( "thirst_icon" );
//             BleedingIcon = BleedingPanel.Add.Panel( "bleeding_icon" );


            
//         }
//     }
// }