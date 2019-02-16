using System;
using W8lessLabs.GraphAPI;

namespace BlazorGraphExample.Services
{
    // This is a temporary object since current JsonSerializer appears to not handle DateTimeOffset on GraphAuthResponse.
    // Probably can change GraphAuthResponse to just use DateTime and then will remove this go-between object.
    public class GetTokenResult
    {
        public string IdToken { get; set; }
        public DateTime Expires { get; set; }
        public GraphAccount Account { get; set; }

        public GraphAuthResponse ToGraphAuthResponse() =>
            new GraphAuthResponse(IdToken, Expires, Account);
            
    }
}
