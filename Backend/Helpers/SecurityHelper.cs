using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace MSSP.Helpers
{
    internal static class SecurityHelper
    {

        /// <summary>
        /// Genera el token
        /// </summary>
        /// <returns>Token generated</returns>
        /// <exception cref="Exception"></exception>
        internal static string CreateJWT()
        {
            try
            {
                // Header creation //
                var _symmetricSecurityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes("")
                    );
                var _signingCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(
                        _symmetricSecurityKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha512
                    );
                var _Header = new JwtHeader(_signingCredentials);

                // Payload creation //
                var _Payload = new JwtPayload(
                        issuer: "",
                        audience: "",
                        claims: null,
                        notBefore: DateTime.UtcNow,
                        // Time expiration
                        expires: DateTime.Now
                    );

                // Token generation //
                var _Token = new JwtSecurityToken(
                        _Header,
                        _Payload
                    );

                string JWT = new JwtSecurityTokenHandler().WriteToken(_Token);
                return JWT;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}