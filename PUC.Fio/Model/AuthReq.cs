using System;

namespace PUC.Fio.Model
{
    public class AuthReq
    {
        public string AuthToken { get; set; }
        public DateTime Expiry { get; set; }
    }
}