using System.Security.Claims;
using FluentAssertions;
using Luna.Auth.Models.Blank.Models;
using Luna.Auth.Models.Database.Models;
using Luna.Auth.Models.Domain.Models;
using Luna.Auth.Repositories.Repositories.AuthRepository;
using Luna.Auth.Repositories.Repositories.SessionArchiveRepository;
using Luna.Auth.Repositories.Repositories.SessionRepository;
using Luna.Auth.Services.Services.AuthService;
using Luna.Auth.Services.Services.TokensService;
using Luna.Tools.Crypto;
using Luna.Tools.Web;
using Luna.Users.gRPC.Client.Services;
using Moq;

namespace Luna.Auth.UnitTests;

public class AuthServiceTests
{
	private readonly Mock<IAuthRepository> _mockAuthRepository;
	private readonly Mock<ISessionRepository> _mockSessionRepository;
	private readonly Mock<ITokensService> _mockTokensService;
	private readonly Mock<IUserServiceClient> _userServiceClient;
	private readonly AuthService _authService;

	public AuthServiceTests()
	{
		_mockAuthRepository = new Mock<IAuthRepository>();
		_mockSessionRepository = new Mock<ISessionRepository>();
		Mock<ISessionArchiveRepository> mockSessionArchiveRepository = new();
		_mockTokensService = new Mock<ITokensService>();
		_userServiceClient = new Mock<IUserServiceClient>();
		JwtOptions jwtOptions = new()
		{
			ValidInDays = 1,
			RefreshValidInDays = 7
		};

		// _authService = new AuthService
		// (
		// 	_mockAuthRepository.Object,
		// 	_mockSessionRepository.Object,
		// 	mockSessionArchiveRepository.Object,
		// 	_mockTokensService.Object,
		// 	jwtOptions,
		// 	_userServiceClient.Object
		// );
	}

	[Fact]
	public async Task LoginAsync_WithValidCredentials_ReturnsAuthDomain()
	{
		string email = "test@example.com";
		string password = "password";
		Guid userId = Guid.NewGuid();
		byte[] passwordHash = Crypto.HashSha512(password);

		AuthUserDatabase user = new AuthUserDatabase
		{
			Id = userId,
			Email = email,
		};

		_mockAuthRepository.Setup(repo => repo.GetAuthUserAsync(email))
			.ReturnsAsync(user);

		_mockTokensService.Setup(service => service.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>()))
			.Returns("access_token");

		_mockTokensService.Setup(service => service.GenerateRefreshToken())
			.Returns("refresh_token");

		_mockSessionRepository.Setup(repo =>
				repo.SetSessionAsync(
					It.IsAny<Guid>(),
					It.IsAny<Guid>(),
					It.IsAny<SessionDatabase>(),
					It.IsAny<TimeSpan>()))
			.ReturnsAsync(true);

		AuthDomain result = await _authService.SignInAsync(new SignInCodeBlank() {Email = email});

		result.Should().NotBeNull();
		result.Email.Should().Be(email);
		result.UserId.Should().Be(userId);
		result.Token.Should().Be("access_token");
		result.RefreshToken.Should().Be("refresh_token");
	}

	[Fact]
	public async Task LoginAsync_WithInvalidPassword_ThrowsUnauthorizedAccessException()
	{
		string email = "test@example.com";
		string password = "password";
		Guid userId = Guid.NewGuid();
		byte[] passwordHash = Crypto.HashSha512("different_password");

		AuthUserDatabase user = new AuthUserDatabase
		{
			Id = userId,
			Email = email,
		};

		_mockAuthRepository.Setup(repo => repo.GetAuthUserAsync(email))
			.ReturnsAsync(user);

		await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
			_authService.SignInAsync(new  SignInCodeBlank(){Email = email}));
	}
}