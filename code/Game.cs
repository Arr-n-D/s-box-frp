using Sandbox;
using System.Threading.Tasks;

//
// You don't need to put things in a namespace, but it doesn't hurt.
//
namespace fRP;

/// <summary>
/// This is your game class. This is an entity that is created serverside when
/// the game starts, and is replicated to the client. 
/// 
/// You can use this to create things like HUDs and declare which player class
/// to use for spawned players.
/// </summary>
public partial class frpGame : Game
{
	public frpGame()
	{
		if ( IsServer )
		{
			DownloadAsset("gvar.citizen_zombie");
			_ = new fRPHud();
			
		}
		
	}

	/// <summary>
	/// A client has joined the server. Make them a pawn to play with
	/// </summary>
	public override void ClientJoined( Client cl )
	{
		base.ClientJoined( cl );
		var player = new Player( cl );
		player.Respawn();

		cl.Pawn = player;
	}

	static async Task DownloadAsset( string packageName)
	{
        var package = await Package.Fetch( packageName, false );
        if ( package == null || package.PackageType != Package.Type.Model || package.Revision == null )
        {
            // spawn error particles
            return;
        }

        var model = package.GetMeta( "PrimaryAsset", "models/dev/error.vmdl" );
        // downloads if not downloads, mounts if not mounted
        await package.MountAsync();

		Precache.Add( model );
    }
}
