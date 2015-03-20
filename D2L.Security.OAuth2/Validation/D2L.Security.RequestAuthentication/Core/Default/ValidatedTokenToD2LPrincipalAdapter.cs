﻿using System;
using System.Collections.Generic;
using D2L.Security.AuthTokenValidation;

namespace D2L.Security.RequestAuthentication.Core.Default {
	internal sealed class ValidatedTokenToD2LPrincipalAdapter : ID2LPrincipal {

		private readonly DateTime m_expiry;
		private readonly string m_userId;
		private readonly string m_tenantId;
		private readonly string m_tenantUrl;
		private readonly IEnumerable<string> m_scopes;
		private readonly PrincipalType m_type;
		
		private readonly string m_xsrf;
		private readonly string m_accessToken;

		internal ValidatedTokenToD2LPrincipalAdapter( IValidatedToken validatedToken, string accessToken ) {
			m_expiry = validatedToken.Expiry;
			m_tenantId = validatedToken.GetTenantId();
			m_tenantUrl = validatedToken.GetTenantUrl();
			m_scopes = validatedToken.GetScopes();

			m_userId = validatedToken.GetUserId();
			m_type = string.IsNullOrEmpty( m_userId ) ? PrincipalType.Service : PrincipalType.User;

			m_xsrf = validatedToken.GetXsrfToken();
			m_accessToken = accessToken;
		}

		DateTime ID2LPrincipal.AccessTokenExpiry {
			get { return m_expiry; }
		}

		string ID2LPrincipal.UserId {
			get { return m_userId; }
		}
		
		string ID2LPrincipal.TenantId {
			get { return m_tenantId; }
		}

		string ID2LPrincipal.TenantUrl {
			get { return m_tenantUrl; }
		}
		
		IEnumerable<string> ID2LPrincipal.Scopes {
			get { return m_scopes; }
		}

		PrincipalType ID2LPrincipal.Type {
			get { return m_type; }
		}

		string ID2LPrincipal.Xsrf {
			get { return m_xsrf;  }
		}

		string ID2LPrincipal.AccessToken {
			get { return m_accessToken; }
		}
	}
}