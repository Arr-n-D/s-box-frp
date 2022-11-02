using System.Text.Json.Serialization;
using fRP.Networking.Interfaces;
namespace fRP.Networking.Errors
{
	public class PlayerNotFoundError : IError
	{
		public string Message { get; set; } = "Player not found.";

        public string ReturnErrorMessage()
        {
            return Message;
        }
	}
}