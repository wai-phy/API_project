using System;

namespace TodoApi.Models
{
    /* 
     * This class don't use for Table. It will use only for AuthToken Data as Object
     */
    public class TokenData
    {
        public string Sub = "";  //Required Field, Used for core JWT
        public string Jti = ""; //Required Field, Used for core JWT
        public string Iat = ""; //Required Field, Used for core JWT
        public string UserID = "";
        public string UserName = "";
        public string LoginType = "";
        public string UserLevelID = "";
        public string IPAddress = "";

        public bool isAdmin = false;

        public DateTime TicketExpireDate = DateTime.UtcNow;
    }
}