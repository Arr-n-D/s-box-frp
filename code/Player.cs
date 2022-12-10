using Sandbox;

namespace fRP;
partial class Player : Sandbox.Player
{
	private TimeSince timeSinceDropped;
	private TimeSince timeSinceJumpReleased;

	// private BaseInventory inventory;


	public ClothingContainer Clothing = new();
	public override void Respawn()
	{
		SetModel( "models/citizen/citizen.vmdl" );
		EnableHideInFirstPerson = true;
		EnableAllCollisions = true;
		EnableDrawing = true;
		EnableShadowInFirstPerson = true;
		Controller = new WalkController();

		if ( DevController is NoclipController )
		{
			DevController = null;
		}

		base.Respawn();

	}

	public Player( Client cl )
	{
		// Load clothing from client data
		Clothing.LoadFromClient( cl );
	}

	public Player() { }
	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		SimulateActiveChild( cl, ActiveChild );

		if ( LifeState != LifeState.Alive )
			return;

		var controller = GetActiveController();
		if ( controller != null )
		{
			EnableSolidCollisions = !controller.HasTag( "noclip" );

			SimulateAnimation( controller );
		}

		TickPlayerUse();
		SimulateActiveChild( cl, ActiveChild );

		if ( Input.Pressed( InputButton.View ) )
		{
			// if ( CameraMode is ThirdPersonCamera )
			// {
			// 	CameraMode = new FirstPersonCamera();
			// }
			// else
			// {
			// 	CameraMode = new ThirdPersonCamera();
			// }
		}

		if ( Input.Pressed( InputButton.Drop ) )
		{
			var dropped = Inventory.DropActive();
			if ( dropped != null )
			{
				dropped.PhysicsGroup.ApplyImpulse( Velocity + EyeRotation.Forward * 500.0f + Vector3.Up * 100.0f, true );
				dropped.PhysicsGroup.ApplyAngularImpulse( Vector3.Random * 100.0f, true );

				this.timeSinceDropped = 0;
			}
		}

		if ( Input.Released( InputButton.Jump ) )
		{
			if ( timeSinceJumpReleased < 0.3f )
			{
				GameManager.Current?.DoPlayerNoclip( cl );
			}

			timeSinceJumpReleased = 0;
		}

		if ( Input.AnalogMove.y != 0 || Input.AnalogMove.x != 0 )
		{
			timeSinceJumpReleased = 1;
		}
	}

	Entity lastWeapon;

	void SimulateAnimation( PawnController controller )
	{
		if ( controller == null )
			return;

		// where should we be rotated to
		var turnSpeed = 0.02f;

		Rotation rotation;

		// If we're a bot, spin us around 180 degrees.
		if ( Client.IsBot )
			rotation = ViewAngles.WithYaw( ViewAngles.yaw + 180f ).ToRotation();
		else
			rotation = ViewAngles.ToRotation();

		var idealRotation = Rotation.LookAt( rotation.Forward.WithZ( 0 ), Vector3.Up );
		Rotation = Rotation.Slerp( Rotation, idealRotation, controller.WishVelocity.Length * Time.Delta * turnSpeed );
		Rotation = Rotation.Clamp( idealRotation, 45.0f, out var shuffle ); // lock facing to within 45 degrees of look direction

		CitizenAnimationHelper animHelper = new CitizenAnimationHelper( this );

		animHelper.WithWishVelocity( controller.WishVelocity );
		animHelper.WithVelocity( controller.Velocity );
		animHelper.WithLookAt( EyePosition + EyeRotation.Forward * 100.0f, 1.0f, 1.0f, 0.5f );
		animHelper.AimAngle = rotation;
		animHelper.FootShuffle = shuffle;
		animHelper.DuckLevel = MathX.Lerp( animHelper.DuckLevel, controller.HasTag( "ducked" ) ? 1 : 0, Time.Delta * 10.0f );
		animHelper.VoiceLevel = ( Host.IsClient && Client.IsValid() ) ? Client.TimeSinceLastVoice < 0.5f ? Client.VoiceLevel : 0.0f : 0.0f;
		animHelper.IsGrounded = GroundEntity != null;
		animHelper.IsSitting = controller.HasTag( "sitting" );
		animHelper.IsNoclipping = controller.HasTag( "noclip" );
		animHelper.IsClimbing = controller.HasTag( "climbing" );
		animHelper.IsSwimming = this.GetWaterLevel() >= 0.5f;
		animHelper.IsWeaponLowered = false;

		if ( controller.HasEvent( "jump" ) ) animHelper.TriggerJump();
		if ( ActiveChild != lastWeapon ) animHelper.TriggerDeploy();

		if ( ActiveChild is BaseCarriable carry )
		{
			carry.SimulateAnimator( animHelper );
		}
		else
		{
			animHelper.HoldType = CitizenAnimationHelper.HoldTypes.None;
			animHelper.AimBodyWeight = 0.5f;
		}

		lastWeapon = ActiveChild;
	}

}