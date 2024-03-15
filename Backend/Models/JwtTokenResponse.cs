using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class JwtTokenResponse
    {

        public string accessToken {  get; set; }

        public string encryptedAccessToken { get; set; }

        public int? expiresInSeconds { get; set; }

        public bool success {  get; set; }

        public ErrorJwtToken error {  get; set; }

    }


    public class ErrorJwtToken
    {
        public int statusCode { get; set; }

        public string message { get; set; }
    }

    public class JwtTokenRequest
    {
        public string usernameOrEmailAddresss { get; set; }

        public string password { get; set; }
    }
}