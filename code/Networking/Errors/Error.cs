using System;
using fRP.Networking.Interfaces;
using fRP.Networking.Packets;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace fRP.Networking.Errors;

public class Error : IError
{
	[JsonPropertyName( "error" )]
	public ushort ErrorCode { get; set; }

	public Error( ushort error )
	{
		this.ErrorCode = error == 0 ? throw new ArgumentNullException( nameof( error ) ) : error;
	}

	public virtual string ReturnErrorMessage()
	{
		return "Unknown error";
	}

	public Tuple<bool, string> GotError( Packet packet )
	{
		try
		{
			var error = JsonSerializer.Deserialize<Error>( packet.Content );
			var pNotFoundError = Mappings.IdToErrorType[error.ErrorCode];
            return true

		}
		catch ( System.Exception e )
		{
			Log.Error( e );
		}
	}
}