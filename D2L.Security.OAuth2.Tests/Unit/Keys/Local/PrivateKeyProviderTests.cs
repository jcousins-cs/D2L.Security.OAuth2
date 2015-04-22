﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Threading.Tasks;

using D2L.Security.OAuth2.Keys;
using D2L.Security.OAuth2.Keys.Local;
using D2L.Security.OAuth2.Keys.Local.Data;
using D2L.Security.OAuth2.Keys.Local.Default;
using D2L.Security.OAuth2.Utilities;

using Moq;

using NUnit.Framework;

namespace D2L.Security.OAuth2.Tests.Unit.Keys.Local {
	[TestFixture]
	[Category( "Unit" )]
	sealed class PrivateKeyProviderTests {
		private const long ROTATION_PERIOD_SECONDS = 10*60;
		private const long KEY_LIFETIME_SECONDS = 60*60;
		private static readonly TimeSpan KEY_LIFETIME = TimeSpan.FromSeconds( KEY_LIFETIME_SECONDS );
		private static readonly TimeSpan ROTATION_PERIOD = TimeSpan.FromSeconds( ROTATION_PERIOD_SECONDS );

		private Mock<IPublicKeyDataProvider> m_mockPublicKeyDataProvider;
		private Mock<IDateTimeProvider> m_mockDateTimeProvider;
		private IPrivateKeyProvider m_privateKeyProvider;

		[SetUp]
		public void SetUp() {
			m_mockPublicKeyDataProvider = new Mock<IPublicKeyDataProvider>( MockBehavior.Strict );
			m_mockPublicKeyDataProvider.Setup( pkdp => pkdp.SaveAsync( It.IsAny<JsonWebKey>() ) ).Returns( Task.Delay( 0 ) );

			m_mockDateTimeProvider = new Mock<IDateTimeProvider>();
			m_mockDateTimeProvider.Setup( dp => dp.UtcNow ).Returns( () => DateTime.UtcNow );

			m_privateKeyProvider = new PrivateKeyProvider(
				m_mockPublicKeyDataProvider.Object,
				m_mockDateTimeProvider.Object,
				KEY_LIFETIME,
				ROTATION_PERIOD );
		}

		[Test]
		public async Task GetSigningCredentialsAsync_FirstCall_CreatesAndReturnsKey() {
			D2LSecurityToken key = await m_privateKeyProvider.GetSigningCredentialsAsync();

			m_mockPublicKeyDataProvider.Verify( pkdp => pkdp.SaveAsync( It.IsAny<JsonWebKey>() ), Times.Once() );

			Assert.NotNull( key );
		}

		[TestCase(0)]
		[TestCase(1)]
		[TestCase((KEY_LIFETIME_SECONDS - ROTATION_PERIOD_SECONDS) / 2)]
		[TestCase(KEY_LIFETIME_SECONDS - ROTATION_PERIOD_SECONDS - 1)]
		public async Task GetSigningCredentialsAsync_SecondCallShortlyAfter_ReturnsSameKey( long offsetSeconds ) {
			DateTime now = DateTime.UtcNow;

			m_mockDateTimeProvider.Setup( dtp => dtp.UtcNow ).Returns( now );
			D2LSecurityToken key1 = await m_privateKeyProvider.GetSigningCredentialsAsync();

			m_mockDateTimeProvider.Setup( dtp => dtp.UtcNow ).Returns( now + TimeSpan.FromSeconds( offsetSeconds ) );
			D2LSecurityToken key2 = await m_privateKeyProvider.GetSigningCredentialsAsync();

			m_mockPublicKeyDataProvider.Verify( pkdp => pkdp.SaveAsync( It.IsAny<JsonWebKey>() ), Times.Once() );

			Assert.AreEqual( key1.KeyId, key2.KeyId );
		}

		[TestCase(0)]
		[TestCase(1)]
		[TestCase(ROTATION_PERIOD_SECONDS / 2)]
		[TestCase(ROTATION_PERIOD_SECONDS - 1)]
		[TestCase(ROTATION_PERIOD_SECONDS)]
		[TestCase(ROTATION_PERIOD_SECONDS + 1)]
		public async Task GetSigningCredentialsAsync_KeyDuringOrAfterRotationPeriod_ReturnsNewKey( long offsetSeconds ) {
			DateTime now = DateTime.UtcNow;

			m_mockDateTimeProvider.Setup( dtp => dtp.UtcNow ).Returns( now );
			D2LSecurityToken key1 = await m_privateKeyProvider.GetSigningCredentialsAsync();

			m_mockDateTimeProvider
				.Setup( dtp => dtp.UtcNow )
				.Returns( now + KEY_LIFETIME - ROTATION_PERIOD + TimeSpan.FromSeconds( offsetSeconds ) );

			D2LSecurityToken key2 = await m_privateKeyProvider.GetSigningCredentialsAsync();

			m_mockPublicKeyDataProvider.Verify( pkdp => pkdp.SaveAsync( It.IsAny<JsonWebKey>() ), Times.Exactly( 2 ) );

			Assert.AreNotEqual( key1.KeyId, key2.KeyId );
		}

		[Test]
		public async Task GetSigningCredentialsAsync_RaceyFirstCall_CreatesOnlyOneKey() {
			var tasks = Enumerable
				.Range( 0, 20 )
				.Select( _ => m_privateKeyProvider.GetSigningCredentialsAsync() );

			IEnumerable<D2LSecurityToken> keys = await Task.WhenAll( tasks );

			m_mockPublicKeyDataProvider.Verify( pkdp => pkdp.SaveAsync( It.IsAny<JsonWebKey>() ), Times.Once() );
			var ids = keys.Select( k => k.KeyId ).ToList();
			foreach( Guid id in ids ) {
				Assert.AreEqual( ids[ 0 ], id );
			}
		}
	}

}