﻿namespace IAlgoTrader.Back.SeedWorks.Infrastructure
{
    public class JwToken
    {
        public string TokenType { get; set; }
        public string AuthToken { get; set; }
        public string RefreshToken { get; set; }
        public int expires_in { get; set; }
    }
}
