﻿using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using D2L.Security.OAuth2.Keys;
using D2L.Security.OAuth2.Keys.Default;
using D2L.Security.OAuth2.Keys.Development;
using D2L.Security.OAuth2.Provisioning;
using D2L.Security.OAuth2.Provisioning.Default;

namespace D2L.Security.OAuth2.TestFramework
{
	public static class TestAccessTokenProviderFactory
    {
		private static readonly Guid Guid = new Guid( "FA7C07A8-42C8-4C57-9AF2-CCE10C271033" );
		private static readonly RSAParameters ExpandoPrivateRsaKey = new RSAParameters
		{
			D = Convert.FromBase64String( "beXnLc1J4DdtieYmFArjVXIluXvHusYzVbTlXTXVFxRl7rsEFNn2M0mJCdCW2SLjBERaMuGB03MOAW1rBQOw7uizwlC4hgIgs8ONr38kiTe2yIU6/LsbadILf/GAGeDFt/X0ZWL2AWJ5OmY56UcMvOQchKApdWEMgJtvMLM/xN7/8R54zvM53G20fl6GqnRrJT1zCpADjSUpeuY4/+R0HOiFOqXpUEnyTuqAeRuQ41n+8sPioT+Biu2kp7Bwfbeoco3g9vncsjOEIf75cK84dO6r9EdOj6tGLYEugqEJ7RA68eDdNqNDqyeuX5wU3BNQ+KHP0ZSZz6MJ1HH47RGA2Q==" ),
			DP = Convert.FromBase64String( "t2N01VAlELr0EW5w1+8C4ZuaewkWoMwmRnH1x81ML5mBPub4I6CohUdIFRuqiXoox2POB2O2I/PnxC8EKSpO9q4ckmls1VCEa+jrl0bYLhPRtePjBCAQ98OS6C1/ZiGxejqv8QzXxKlgV4sgKGm7tBzmZ1ETLE6y5DLQBgsUhB0=" ),
			DQ = Convert.FromBase64String( "VYcSUmJcVg4SCd+OdSEEzSX6TamvsDnNoFSJ79waoC1kCnGk7OGI0axWaRuerqJslQoydjDGVb4iQ/Iw9fsKea8rQfRKcd8Y9Rz3WXSyOBBa/CryymSjr6dQ+EjgA6TW+KaXP+oIkga5qP1aPQ9dYX/DFnWoWsEBgCdx+mWU0/8=" ),
			Exponent = Convert.FromBase64String( "AQAB" ),
			InverseQ = Convert.FromBase64String( "o93/fc9KZJ86Wy97L1vIW1D7K1uN9374079/F4U7DBNC+XYS2a6gGAnonsHwqkUaZo3XVPl0GfojKgF01CGVIUpNI6u0HiEat7DyByGu4onX5bXhCb7b9Iz7SS2f68xoax0S0zfotWObWxWq4tseg4a/F+2wacNUKUiIEjJvQVQ=" ),
			Modulus = Convert.FromBase64String( "xWrs0hAtygx13cTf+AkW8NjDJz4PPqds86ilcmDAhh5j/FRreI8OJeAv9qMCQCIagRDPFE/i2B1eg9ES241FsJC8lMNYHf697zsywtbC8qmZn8T4RigDAoBSTxaWgnOYPP07FdNFA6jBXZwJ2SzC5yF84tlqDuhYUClQgbDPT2Ur522901rDMBnVJ41dUVJdOhbgOGhwu4hIpp8XJpeLNusvxC3kasxMRs1CopDYgNVUbBkVm3z5KhsudvAlB7PHoDlBLkxnUMNJhGabkgRSSDLO0JpP9trBZSUWoUkd6RQDWm6rLtSH9KvOIaOEBd2y0ipZC2AKFMKHMPEGiOxPCw==" ),
			P = Convert.FromBase64String( "41SWtgV7r/0AcUemlieS9Sasu4ZkduregnpX0PL6uLeUd6QcT2VXm9z89MeNoj/hBIe+Ada5IibmiRhN8LN4Z697Ij0nHRpiNfA9v4YhRSQi0E7B2dla7hdrf+DkVcANdfhjkqYFrDAT6lfiOmpOJtPMZyyViFIUkuZhl81YFG0=" ),
			Q = Convert.FromBase64String( "3lCYzfXalBUpK/2wOIeB/zIU5Gmw9t3pyRdyduCkCuziGKZEZjWwSXt/bkw1wVLVmo3hPSWQJ4WruhE/9WZXeDQeBm7HVcML/lBRnjjqzAzn1HDbC3UfEAB+YvmD0/uuTMGGSPncr8+LWpG8QpUjk5KCvRX9lnATqz+NUymsFlc=" ),
		};

		public static IAccessTokenProvider Create(HttpClient httpClient, String tokenProvisioningEndpoint)
		{
			IAuthServiceClient authServiceClient = new AuthServiceClient( httpClient, new Uri( tokenProvisioningEndpoint ) );

			IPrivateKeyProvider priv = new StaticPrivateKeyProvider( Guid, ExpandoPrivateRsaKey );
			ITokenSigner km = new TokenSigner( priv );

			INonCachingAccessTokenProvider noCacheTokenProvider = new AccessTokenProvider( km, authServiceClient );

			return new CachedAccessTokenProvider( noCacheTokenProvider, Timeout.InfiniteTimeSpan );
		}
    }
}
