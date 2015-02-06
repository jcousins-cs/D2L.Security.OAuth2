﻿namespace D2L.Security.RequestAuthentication {
	public enum AuthenticationResult {
		
		/// <summary>
		/// Authentication was successful
		/// </summary>
		Success,
		
		/// <summary>
		/// Security token is expired
		/// </summary>
		Expired,
		
		/// <summary>
		/// Security token was supplied in more than one location
		/// </summary>
		LocationConflict,
		
		/// <summary>
		/// Security token was not supplied
		/// </summary>
		Anonymous,

		/// <summary>
		/// Xsrf token did not match
		/// </summary>
		XsrfMismatch
	}
}
