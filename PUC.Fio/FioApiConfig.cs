using System;

namespace PUC.Fio
{
    public class FioApiConfig
    {
        /// <summary>
        /// In Min
        /// </summary>
        public int CacheTimeOut { get; set; } = 60;

        public string Username { get; set; }
        public string Password { get; set; }

        /// <summary>
        /// Create api by reading Environment Variable if vars are empty it will enter anonymous mode
        /// </summary>
        public FioApiConfig()
        {
            Username = Environment.GetEnvironmentVariable("USERNAME");
            Password = Environment.GetEnvironmentVariable("PASSWORD");
        }

        /// <summary>
        /// Create new api with authentication
        /// </summary>
        /// <param name="username">Your FIO Username</param>
        /// <param name="password">Your FIO Password</param>
        public FioApiConfig(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}