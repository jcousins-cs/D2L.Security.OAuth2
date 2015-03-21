﻿namespace D2L.Security.OAuth2.Validation.Request {
	internal static class Constants {

		internal static class Headers {
			internal const string COOKIE = "Cookie";
			internal const string XSRF = "X-Csrf-Token";
			internal const string AUTHORIZATION = "Authorization";
		}

		internal static class BearerTokens {
			internal const string SCHEME = "Bearer";
			internal const string SCHEME_PREFIX = SCHEME + " ";
		}

		internal static class Claims {
			internal const string XSRF = "xt";
			internal const string USER_ID = "sub";
			internal const string TENANT_ID = "tenantid";
			internal const string TENANT_URL = "tenanturl";
			internal const string SCOPE = "scope";
		}

		internal const string D2L_AUTH_COOKIE_NAME = "d2lApi";
	}
}