﻿using IAlgorTrader.Back.Service.SeedWorks.Interfaces;
using IAlgoTrader.Back.Domain.Entities;
using IAlgoTrader.Back.SeedWorks.Infrastructure;
using IAlgoTrader.Back.Service.SeedWorks.Interfaces;
using IAlgoTrader.Back.Service.SeedWorks.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using static IAlgoTrader.Back.Domain.Entities.User;

namespace IAlgoTrader.Back.Service.SeedWorks.Helpers
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly IJwtFactory _jwtFactory;
        private readonly ITokenFactory _tokenFactory;

        public TokenGenerator(IJwtFactory jwtFactory, ITokenFactory tokenFactory)
        {
            _jwtFactory = jwtFactory;
            _tokenFactory = tokenFactory;
        }

        public JwToken TokenGeneration(User user, JwtIssuerOptions _jwtOptions, IList<IdentityRole> userRoles)
        {
            var refreshToken = _tokenFactory.GenerateToken();
            if (user.RefreshTokens == null)
                user.RefreshTokens = new List<RefreshToken>();
            user.AddRefreshToken(refreshToken, user.Id, _jwtOptions.ExpireTimeTokenInMinute);

            var identity = _jwtFactory.GenerateClaimsIdentity(user.Email, user.Id);
            if (identity == null)
            {
                throw new SystemException("در فراخوانی و تطابق اطلاعات حساب کاربری خطایی رخ داده است!");
            }

            var userRoleNames = userRoles != null ? userRoles.Select(c => c.Name).ToList() : null;
            var userRoleIds = userRoles != null ? userRoles.Select(c => c.Id.ToString()).ToList() : null;

            var generatedToken = GenerateJwt(user, userRoleNames, userRoleIds, identity, _jwtFactory,
                refreshToken, _jwtOptions.ExpireTimeTokenInMinute);

            return generatedToken;
        }

        public static JwToken GenerateJwt(User user, IList<string> userRoles, IReadOnlyCollection<string> userRoleIds, ClaimsIdentity identity,
            IJwtFactory jwtFactory, string refreshToken, int refreshTime)
        {
            var result = new JwToken
            {
                TokenType = "Bearer",
                AuthToken = jwtFactory.GenerateEncodedToken(user, userRoles, userRoleIds, identity),
                RefreshToken = refreshToken,
                expires_in = refreshTime,
            };

            return result;
        }

    }
}
