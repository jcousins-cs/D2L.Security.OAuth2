﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using D2L.Security.OAuth2.Validation.Token.PublicKeys.OpenIdConfigurations;
using Microsoft.IdentityModel.Protocols;

namespace D2L.Security.OAuth2.Validation.Token.PublicKeys.Default {
	internal sealed class PublicKeyProvider : IPublicKeyProvider {

		private readonly IOpenIdConfigurationFetcher m_configurationFetcher;

		internal PublicKeyProvider( IOpenIdConfigurationFetcher configurationFetcher ) {
			m_configurationFetcher = configurationFetcher;
		}

		IPublicKey IPublicKeyProvider.Get() {
			OpenIdConnectConfiguration openIdKey = m_configurationFetcher.Fetch();
			IPublicKey key = ParseOpenIdKey( openIdKey );
			return key;
		}

		private IPublicKey ParseOpenIdKey( OpenIdConnectConfiguration configuration ) {
			IList<JsonWebKey> jsonWebKeys = configuration.JsonWebKeySet.Keys;
			if( jsonWebKeys.Count != 1 ) {
				throw new Exception( string.Format( "Expected one json web key but got {0}", jsonWebKeys.Count ) );
			}

			SecurityToken securityToken = JsonWebKeyToSecurityToken( jsonWebKeys[0] );
			string issuer = configuration.Issuer;

			return new PublicKey( securityToken, issuer );
		}

		private X509SecurityToken JsonWebKeyToSecurityToken( JsonWebKey jsonWebKey ) {
			
			if( jsonWebKey.Kty != Constants.ALLOWED_KEY_TYPE ) {
				throw new Exception( 
					string.Format( "Expected key type to be {0} but was {1}", Constants.ALLOWED_KEY_TYPE, jsonWebKey.Kty ) 
					);
			}

			IList<string> x5cEntries = jsonWebKey.X5c;
			if( x5cEntries.Count != 1 ) {
				throw new Exception( string.Format( "Expected one x5c entry but got {0}", x5cEntries.Count ) );
			}

			byte[] payload = Convert.FromBase64String( x5cEntries.First() );
			X509Certificate2 certificate = new X509Certificate2( payload );
			X509SecurityToken token = new X509SecurityToken( certificate );
			return token;
		}
	}
}
