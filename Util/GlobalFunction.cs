using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Serilog;
using TodoApi.Models;

namespace TodoApi.Util
{
    public class Globalfunction
    {
        private static readonly IConfiguration _configuration = Startup.StaticConfiguration!;
        public static dynamic SendEmailAsync(string ToEmail, string Subject, string Message, Boolean IsHTML, string ReplyToEmail = "", string ReplyToName = "")
        {
            string SMTPServer = _configuration.GetSection("SMTP:SMTPServer").Get<string>();;
            int SMTPPort = Convert.ToInt32(_configuration.GetSection("SMTP:SMTPPort").Get<string>());
            string SMTPUser = _configuration.GetSection("SMTP:SMTPUser").Get<string>();
            string SMTPPassword = _configuration.GetSection("SMTP:SMTPPassword").Get<string>();
            string SMTPSenderMail = _configuration.GetSection("SMTP:SMTPSenderMail").Get<string>();


            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(SMTPSenderMail, SMTPSenderMail));

            emailMessage.To.Add(new MailboxAddress("", ToEmail));

            if (ReplyToEmail != "")
                emailMessage.ReplyTo.Add(new MailboxAddress(ReplyToName, ReplyToEmail));

            emailMessage.Subject = Subject;

            if (IsHTML)
                emailMessage.Body = new TextPart("html") { Text = Message };
            else
                emailMessage.Body = new TextPart("plain ") { Text = Message };

            using (var client = new SmtpClient())
            {
                try
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    client.Connect(SMTPServer, SMTPPort, SecureSocketOptions.Auto);
                    client.Authenticate(SMTPUser, SMTPPassword);
                    client.Send(emailMessage);
                    client.Disconnect(true);
                }
                catch (Exception ex)
                {
                    Log.Error("Send Mail Fail: " + ex.Message);
                    return false;
                }
            }
            return true;
        }

        
        public static string GenerateRandomChar(string[] AllowedCharacters)
        {
            string sOTP;
            Random rand = new();
            sOTP = AllowedCharacters[rand.Next(0, AllowedCharacters.Length)];
            return sOTP;
        }

        public static string GenerateRandomOTP(int iOTPLength, string[] AllowedCharacters)
        {

            string sOTP = String.Empty;

            string sTempChars;

            Random rand = new();

            for (int i = 0; i < iOTPLength; i++)
            {
                sTempChars = AllowedCharacters[rand.Next(0, AllowedCharacters.Length)];
                sOTP += sTempChars;
            }
            return sOTP;
        }
  
        public static bool CheckPassword(string str)
        {

            bool upppercase = Convert.ToBoolean(_configuration.GetSection("PasswordPolicy:UppperCase").Value);
            bool lowercase = Convert.ToBoolean(_configuration.GetSection("PasswordPolicy:LowerCase").Value);
            bool numericvalue = Convert.ToBoolean(_configuration.GetSection("PasswordPolicy:NumericValue").Value);
            bool specialcharacter = Convert.ToBoolean(_configuration.GetSection("PasswordPolicy:SpecialCharacter").Value);
            int minPasswordLength = Convert.ToInt16(_configuration.GetSection("PasswordPolicy:MinPasswordLength").Value);

            if(str.Length < minPasswordLength){
                throw new ValidationException("Password must be longer than " + minPasswordLength + " characters.");
            }

            if (upppercase && !Regex.IsMatch(str, "(?=.*?[A-Z])"))
            {
                throw new ValidationException("Password must include at least one upper case letter.");
            }
            if (lowercase && !Regex.IsMatch(str, "(?=.*?[a-z])"))
            {
                throw new ValidationException("Password must include at least one lower case letter.");
            }
            if (numericvalue && !Regex.IsMatch(str, "(?=.*?[0-9]).{8,}"))
            {
                throw new ValidationException("Password must include at least one numeric character.");
            }

            string pattern = @"[!""#$@%&'()*+,\-./:;<=>?@[\\\]^_`{|}~\s]";
            if (specialcharacter && !Regex.IsMatch(str, pattern))
            {
                throw new ValidationException("Password must include at least one special character.");
            }
            return true;
        }

        public static string GetClientIP(HttpContext context) {
            string clientip = "127.0.0.1";
            if (context.Connection.RemoteIpAddress != null) 
                clientip = context.Connection.RemoteIpAddress.ToString();
            else if(context.Request.Headers["CF-Connecting-IP"].FirstOrDefault() != null)
                clientip = context.Request.Headers["CF-Connecting-IP"].FirstOrDefault() ?? "";
            else if(context.Request.Headers["X-Forwarded-For"].FirstOrDefault() != null)
                clientip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? "";
                
            return clientip;
        }

        public static Claim[] GetClaims(TokenData obj)
        {
            var claims = new Claim[]
            {
                new Claim("UserID",obj.UserID),
                new Claim("LoginType",obj.LoginType),
                new Claim("UserLevelID", obj.UserLevelID),
                new Claim("isAdmin",obj.isAdmin.ToString()),
                new Claim("TicketExpireDate", obj.TicketExpireDate.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, obj.Sub),
                new Claim(JwtRegisteredClaimNames.Jti, obj.Jti),
                new Claim(JwtRegisteredClaimNames.Iat, obj.Iat, ClaimValueTypes.Integer64)
            };
            return claims;
        }

        public static TokenData GetTokenData(JwtSecurityToken tokenS)
        {
            var obj = new TokenData();
            try
            {
                obj.UserID = tokenS.Claims.First(claim => claim.Type == "UserID").Value;
                obj.LoginType = tokenS.Claims.First(claim => claim.Type == "LoginType").Value;
                obj.UserLevelID = tokenS.Claims.First(claim => claim.Type == "UserLevelID").Value;
                obj.isAdmin = Convert.ToBoolean(tokenS.Claims.First(claim => claim.Type == "isAdmin").Value);
                obj.Sub = tokenS.Claims.First(claim => claim.Type == "sub").Value;
                string TicketExpire = tokenS.Claims.First(claim => claim.Type == "TicketExpireDate").Value;
                DateTime TicketExpireDate = DateTime.Parse(TicketExpire);
                obj.TicketExpireDate = TicketExpireDate;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
            return obj;
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
    }
}