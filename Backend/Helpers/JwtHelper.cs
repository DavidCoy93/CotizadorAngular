using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace MSSPAPI.Helpers
{
    /// <summary>
    /// Class that contains a method for generate a JWT Token
    /// </summary>
    public class JwtHelper
    {
        /// <summary>
        /// Method for generate a JWT Token
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public static string GenerateJwtToken(string userName, string pwd)
        {
            string issuer = "https://www.mssptest.com";
            string audience = "https://www.mssptest.com/api";

            try
			{   
                string encPwd = EncDec.Encript(pwd);

                byte[] ks = Encoding.UTF8.GetBytes(encPwd);

                SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(ks);

                SigningCredentials signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

                JwtHeader header = new JwtHeader(signingCredentials);

                Claim[] claims = new Claim[]
                {
                    new Claim("UserName", userName)
                };

                JwtPayload payload = new JwtPayload(issuer, audience, claims, DateTime.UtcNow, DateTime.UtcNow.AddHours(4));

                JwtSecurityToken token = new JwtSecurityToken(header, payload);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
			catch (Exception ex)
			{

				throw ex;
			}

        }
    }
}