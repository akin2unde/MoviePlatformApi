using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MoviePlatformApi.Models;

namespace MoviePlatformApi
{
    public class JWT
    {
        private static readonly string Secret = "MOVIEApiaW9uZU51bQJlciI6IjA4MCIsLk5hbW==";
        public static string GenerateJWT(User user)
        {
            // generate token that is valid for 7 days normally but will do 30 days for this test 
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Secret);
            var Expires = DateTime.UtcNow.AddDays(30);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(nameof(User.MailAddress), user.MailAddress),
                    new Claim(nameof(User.Name), user.Name),
                    new Claim(nameof(User.Id), user.Id),
                    new Claim(nameof(User.ExpireOn), Expires.ToString()),
                    }),
                Expires = Expires,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static User GetUserFromJWT(string token)
        {
            if (token == null)
                return null;
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Secret);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;
                var user = new User
                {
                    MailAddress = jwtToken.Claims.First(x => x.Type == nameof(User.MailAddress)).Value,
                    Name = jwtToken.Claims.First(x => x.Type == nameof(User.Name)).Value,
                    Id = jwtToken.Claims.First(x => x.Type == nameof(User.Id)).Value,
                    ExpireOn = DateTime.Parse(jwtToken.Claims.First(x => x.Type == nameof(User.ExpireOn)).Value),
                };

                // return user id from JWT token if validation successful
                return user;
            }
            catch (Exception ex)
            {
                // return null if validation fails
                return null;
            }
        }

    }
}