using Microsoft.IdentityModel.Tokens;
using PersonsAPIBusinessLayer.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PersonAPIServerSide.Authentication
{
	public static class Authenticator
	{
		public static bool Authenticate(string userName, string password)
		{
			if (!PersonsAPIBusinessLayer.Users.User.CheckUserCredentials(userName, password))
				return false;

			return true;
		}


		public static string CreateToken(string userName, DateTime expiresAt, string strSecretKey)
		{
			//algo
			//payload
			//signing key for signature

			var secretKey = Encoding.UTF8.GetBytes(strSecretKey);

			var user = User.Find(userName);

			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier,userName??string.Empty),
				new Claim(ClaimTypes.Name, user.Name??string.Empty)
			};

			var jwt = new JwtSecurityToken(signingCredentials: new SigningCredentials(
					new SymmetricSecurityKey(secretKey),
					SecurityAlgorithms.HmacSha256Signature
				),
				claims: claims,
				expires: expiresAt,
				notBefore: DateTime.UtcNow);
			return new JwtSecurityTokenHandler().WriteToken(jwt);
		}

	}
}
