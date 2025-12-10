using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using MotherTreeCrafts.Areas.Identity.Pages.Account;
using MotherTreeCrafts.Models;
using Xunit;

namespace MotherTreeCrafts.Tests;

/// <summary>
/// Unit tests for the LoginModel class
/// </summary>
public class LoginModelTests
{
    private readonly Mock<SignInManager<UserAccount>> _mockSignInManager;
    private readonly Mock<UserManager<UserAccount>> _mockUserManager;
    private readonly Mock<ILogger<LoginModel>> _mockLogger;
    private readonly LoginModel _loginModel;

    public LoginModelTests()
    {
        // Setup UserManager mock
        var userStoreMock = new Mock<IUserStore<UserAccount>>();
        _mockUserManager = new Mock<UserManager<UserAccount>>(
            userStoreMock.Object,
            null, null, null, null, null, null, null, null);

        // Setup SignInManager mock
        var contextAccessorMock = new Mock<IHttpContextAccessor>();
        var claimsFactoryMock = new Mock<IUserClaimsPrincipalFactory<UserAccount>>();
        _mockSignInManager = new Mock<SignInManager<UserAccount>>(
            _mockUserManager.Object,
            contextAccessorMock.Object,
            claimsFactoryMock.Object,
            null, null, null, null);

        _mockLogger = new Mock<ILogger<LoginModel>>();

        _loginModel = new LoginModel(_mockSignInManager.Object, _mockLogger.Object);

        // Setup PageContext and HttpContext
        var httpContext = new DefaultHttpContext();
        var modelState = new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary();
        var actionContext = new ActionContext(httpContext, new Microsoft.AspNetCore.Routing.RouteData(), new PageActionDescriptor(), modelState);
        var pageContext = new PageContext(actionContext);
        _loginModel.PageContext = pageContext;
        _loginModel.Url = new UrlHelper(actionContext);
    }

    #region OnGetAsync Tests

    [Fact]
    public async Task OnGetAsync_ShouldClearExternalCookie()
    {
        // Arrange
        var authServiceMock = new Mock<IAuthenticationService>();
        authServiceMock
            .Setup(a => a.SignOutAsync(
                It.IsAny<HttpContext>(),
                It.IsAny<string>(),
                It.IsAny<AuthenticationProperties>()))
            .Returns(Task.CompletedTask);

        _loginModel.PageContext.HttpContext.RequestServices = new ServiceProviderMock(authServiceMock.Object);

        _mockSignInManager
            .Setup(s => s.GetExternalAuthenticationSchemesAsync())
            .ReturnsAsync(new List<AuthenticationScheme>());

        // Act
        await _loginModel.OnGetAsync();

        // Assert
        authServiceMock.Verify(a => a.SignOutAsync(
            It.IsAny<HttpContext>(),
            IdentityConstants.ExternalScheme,
            It.IsAny<AuthenticationProperties>()), Times.Once);
    }

    [Fact]
    public async Task OnGetAsync_ShouldSetReturnUrl_WhenProvided()
    {
        // Arrange
        var returnUrl = "/test-return-url";
        var authServiceMock = new Mock<IAuthenticationService>();
        authServiceMock
            .Setup(a => a.SignOutAsync(
                It.IsAny<HttpContext>(),
                It.IsAny<string>(),
                It.IsAny<AuthenticationProperties>()))
            .Returns(Task.CompletedTask);

        _loginModel.PageContext.HttpContext.RequestServices = new ServiceProviderMock(authServiceMock.Object);

        _mockSignInManager
            .Setup(s => s.GetExternalAuthenticationSchemesAsync())
            .ReturnsAsync(new List<AuthenticationScheme>());

        // Act
        await _loginModel.OnGetAsync(returnUrl);

        // Assert
        _loginModel.ReturnUrl.Should().Be(returnUrl);
    }

    [Fact]
    public async Task OnGetAsync_ShouldSetDefaultReturnUrl_WhenNotProvided()
    {
        // Arrange
        var authServiceMock = new Mock<IAuthenticationService>();
        authServiceMock
            .Setup(a => a.SignOutAsync(
                It.IsAny<HttpContext>(),
                It.IsAny<string>(),
                It.IsAny<AuthenticationProperties>()))
            .Returns(Task.CompletedTask);

        _loginModel.PageContext.HttpContext.RequestServices = new ServiceProviderMock(authServiceMock.Object);

        _mockSignInManager
            .Setup(s => s.GetExternalAuthenticationSchemesAsync())
            .ReturnsAsync(new List<AuthenticationScheme>());

        // Act
        await _loginModel.OnGetAsync(null);

        // Assert
        _loginModel.ReturnUrl.Should().Be("/");
    }

    [Fact]
    public async Task OnGetAsync_ShouldLoadExternalLogins()
    {
        // Arrange
        var authServiceMock = new Mock<IAuthenticationService>();
        authServiceMock
            .Setup(a => a.SignOutAsync(
                It.IsAny<HttpContext>(),
                It.IsAny<string>(),
                It.IsAny<AuthenticationProperties>()))
            .Returns(Task.CompletedTask);

        _loginModel.PageContext.HttpContext.RequestServices = new ServiceProviderMock(authServiceMock.Object);

        var externalLogins = new List<AuthenticationScheme>
        {
            new AuthenticationScheme("Google", "Google", typeof(IAuthenticationHandler)),
            new AuthenticationScheme("Facebook", "Facebook", typeof(IAuthenticationHandler))
        };

        _mockSignInManager
            .Setup(s => s.GetExternalAuthenticationSchemesAsync())
            .ReturnsAsync(externalLogins);

        // Act
        await _loginModel.OnGetAsync();

        // Assert
        _loginModel.ExternalLogins.Should().HaveCount(2);
        _loginModel.ExternalLogins.Should().Contain(s => s.Name == "Google");
        _loginModel.ExternalLogins.Should().Contain(s => s.Name == "Facebook");
    }

    [Fact]
    public async Task OnGetAsync_ShouldAddErrorToModelState_WhenErrorMessageExists()
    {
        // Arrange
        var errorMessage = "Test error message";
        _loginModel.ErrorMessage = errorMessage;

        var authServiceMock = new Mock<IAuthenticationService>();
        authServiceMock
            .Setup(a => a.SignOutAsync(
                It.IsAny<HttpContext>(),
                It.IsAny<string>(),
                It.IsAny<AuthenticationProperties>()))
            .Returns(Task.CompletedTask);

        _loginModel.PageContext.HttpContext.RequestServices = new ServiceProviderMock(authServiceMock.Object);

        _mockSignInManager
            .Setup(s => s.GetExternalAuthenticationSchemesAsync())
            .ReturnsAsync(new List<AuthenticationScheme>());

        // Act
        await _loginModel.OnGetAsync();

        // Assert
        _loginModel.ModelState.IsValid.Should().BeFalse();
        _loginModel.ModelState.Should().ContainKey(string.Empty);
    }

    #endregion

    #region OnPostAsync - Successful Login Tests

    [Fact]
    public async Task OnPostAsync_ShouldRedirectToReturnUrl_WhenLoginSucceeds()
    {
        // Arrange
        var returnUrl = "/dashboard";
        _loginModel.Input = new LoginModel.InputModel
        {
            Username = "testuser",
            Password = "Test@123",
            RememberMe = false
        };

        _mockSignInManager
            .Setup(s => s.PasswordSignInAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<bool>()))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        _mockSignInManager
            .Setup(s => s.GetExternalAuthenticationSchemesAsync())
            .ReturnsAsync(new List<AuthenticationScheme>());

        // Act
        var result = await _loginModel.OnPostAsync(returnUrl);

        // Assert
        result.Should().BeOfType<LocalRedirectResult>();
        var redirectResult = result as LocalRedirectResult;
        redirectResult!.Url.Should().Be(returnUrl);
    }

    [Fact]
    public async Task OnPostAsync_ShouldUseDefaultReturnUrl_WhenNotProvided()
    {
        // Arrange
        _loginModel.Input = new LoginModel.InputModel
        {
            Username = "testuser",
            Password = "Test@123",
            RememberMe = false
        };

        _mockSignInManager
            .Setup(s => s.PasswordSignInAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<bool>()))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        _mockSignInManager
            .Setup(s => s.GetExternalAuthenticationSchemesAsync())
            .ReturnsAsync(new List<AuthenticationScheme>());

        // Act
        var result = await _loginModel.OnPostAsync(null);

        // Assert
        result.Should().BeOfType<LocalRedirectResult>();
        var redirectResult = result as LocalRedirectResult;
        redirectResult!.Url.Should().Be("/");
    }

    [Fact]
    public async Task OnPostAsync_ShouldCallPasswordSignInAsync_WithCorrectParameters()
    {
        // Arrange
        var username = "testuser";
        var password = "Test@123";
        var rememberMe = true;

        _loginModel.Input = new LoginModel.InputModel
        {
            Username = username,
            Password = password,
            RememberMe = rememberMe
        };

        _mockSignInManager
            .Setup(s => s.PasswordSignInAsync(username, password, rememberMe, false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        _mockSignInManager
            .Setup(s => s.GetExternalAuthenticationSchemesAsync())
            .ReturnsAsync(new List<AuthenticationScheme>());

        // Act
        await _loginModel.OnPostAsync(null);

        // Assert
        _mockSignInManager.Verify(s => s.PasswordSignInAsync(
            username,
            password,
            rememberMe,
            false), Times.Once);
    }

    [Fact]
    public async Task OnPostAsync_ShouldLogInformation_WhenLoginSucceeds()
    {
        // Arrange
        _loginModel.Input = new LoginModel.InputModel
        {
            Username = "testuser",
            Password = "Test@123",
            RememberMe = false
        };

        _mockSignInManager
            .Setup(s => s.PasswordSignInAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<bool>()))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        _mockSignInManager
            .Setup(s => s.GetExternalAuthenticationSchemesAsync())
            .ReturnsAsync(new List<AuthenticationScheme>());

        // Act
        await _loginModel.OnPostAsync(null);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("User logged in")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    #endregion

    #region OnPostAsync - Failed Login Tests

    [Fact]
    public async Task OnPostAsync_ShouldReturnPage_WhenLoginFails()
    {
        // Arrange
        _loginModel.Input = new LoginModel.InputModel
        {
            Username = "testuser",
            Password = "WrongPassword",
            RememberMe = false
        };

        _mockSignInManager
            .Setup(s => s.PasswordSignInAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<bool>()))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

        _mockSignInManager
            .Setup(s => s.GetExternalAuthenticationSchemesAsync())
            .ReturnsAsync(new List<AuthenticationScheme>());

        // Act
        var result = await _loginModel.OnPostAsync(null);

        // Assert
        result.Should().BeOfType<PageResult>();
    }

    [Fact]
    public async Task OnPostAsync_ShouldAddModelError_WhenLoginFails()
    {
        // Arrange
        _loginModel.Input = new LoginModel.InputModel
        {
            Username = "testuser",
            Password = "WrongPassword",
            RememberMe = false
        };

        _mockSignInManager
            .Setup(s => s.PasswordSignInAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<bool>()))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

        _mockSignInManager
            .Setup(s => s.GetExternalAuthenticationSchemesAsync())
            .ReturnsAsync(new List<AuthenticationScheme>());

        // Act
        await _loginModel.OnPostAsync(null);

        // Assert
        _loginModel.ModelState.IsValid.Should().BeFalse();
        _loginModel.ModelState.Should().ContainKey(string.Empty);
        _loginModel.ModelState[string.Empty]!.Errors.Should().Contain(e => e.ErrorMessage == "Invalid login attempt.");
    }

    [Fact]
    public async Task OnPostAsync_ShouldReturnPage_WhenModelStateIsInvalid()
    {
        // Arrange
        _loginModel.Input = new LoginModel.InputModel
        {
            Username = "testuser",
            Password = "Test@123",
            RememberMe = false
        };

        _loginModel.ModelState.AddModelError("Test", "Test error");

        _mockSignInManager
            .Setup(s => s.GetExternalAuthenticationSchemesAsync())
            .ReturnsAsync(new List<AuthenticationScheme>());

        // Act
        var result = await _loginModel.OnPostAsync(null);

        // Assert
        result.Should().BeOfType<PageResult>();
        _mockSignInManager.Verify(s => s.PasswordSignInAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<bool>(),
            It.IsAny<bool>()), Times.Never);
    }

    #endregion

    #region OnPostAsync - Lockout Tests

    [Fact]
    public async Task OnPostAsync_ShouldRedirectToLockout_WhenAccountIsLockedOut()
    {
        // Arrange
        _loginModel.Input = new LoginModel.InputModel
        {
            Username = "lockeduser",
            Password = "Test@123",
            RememberMe = false
        };

        _mockSignInManager
            .Setup(s => s.PasswordSignInAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<bool>()))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.LockedOut);

        _mockSignInManager
            .Setup(s => s.GetExternalAuthenticationSchemesAsync())
            .ReturnsAsync(new List<AuthenticationScheme>());

        // Act
        var result = await _loginModel.OnPostAsync(null);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        var redirectResult = result as RedirectToPageResult;
        redirectResult!.PageName.Should().Be("./Lockout");
    }

    [Fact]
    public async Task OnPostAsync_ShouldLogWarning_WhenAccountIsLockedOut()
    {
        // Arrange
        _loginModel.Input = new LoginModel.InputModel
        {
            Username = "lockeduser",
            Password = "Test@123",
            RememberMe = false
        };

        _mockSignInManager
            .Setup(s => s.PasswordSignInAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<bool>()))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.LockedOut);

        _mockSignInManager
            .Setup(s => s.GetExternalAuthenticationSchemesAsync())
            .ReturnsAsync(new List<AuthenticationScheme>());

        // Act
        await _loginModel.OnPostAsync(null);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("User account locked out")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    #endregion

    #region OnPostAsync - Two-Factor Authentication Tests

    [Fact]
    public async Task OnPostAsync_ShouldRedirectToTwoFactor_WhenTwoFactorRequired()
    {
        // Arrange
        var returnUrl = "/dashboard";
        var rememberMe = true;

        _loginModel.Input = new LoginModel.InputModel
        {
            Username = "2fauser",
            Password = "Test@123",
            RememberMe = rememberMe
        };

        _mockSignInManager
            .Setup(s => s.PasswordSignInAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<bool>()))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.TwoFactorRequired);

        _mockSignInManager
            .Setup(s => s.GetExternalAuthenticationSchemesAsync())
            .ReturnsAsync(new List<AuthenticationScheme>());

        // Act
        var result = await _loginModel.OnPostAsync(returnUrl);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        var redirectResult = result as RedirectToPageResult;
        redirectResult!.PageName.Should().Be("./LoginWith2fa");
    }

    [Fact]
    public async Task OnPostAsync_ShouldPassReturnUrlAndRememberMe_WhenRedirectingToTwoFactor()
    {
        // Arrange
        var returnUrl = "/dashboard";

        _loginModel.Input = new LoginModel.InputModel
        {
            Username = "2fauser",
            Password = "Test@123",
            RememberMe = true
        };

        _mockSignInManager
            .Setup(s => s.PasswordSignInAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<bool>()))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.TwoFactorRequired);

        _mockSignInManager
            .Setup(s => s.GetExternalAuthenticationSchemesAsync())
            .ReturnsAsync(new List<AuthenticationScheme>());

        // Act
        var result = await _loginModel.OnPostAsync(returnUrl);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        var redirectResult = result as RedirectToPageResult;
        redirectResult!.RouteValues.Should().ContainKey("ReturnUrl");
        redirectResult.RouteValues.Should().ContainKey("RememberMe");
        redirectResult.RouteValues!["ReturnUrl"].Should().Be(returnUrl);
        redirectResult.RouteValues["RememberMe"].Should().Be(true);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public async Task CompleteLoginWorkflow_WithValidCredentials_ShouldSucceed()
    {
        // Arrange
        var username = "validuser";
        var password = "ValidPass@123";
        var returnUrl = "/home";

        _loginModel.Input = new LoginModel.InputModel
        {
            Username = username,
            Password = password,
            RememberMe = false
        };

        _mockSignInManager
            .Setup(s => s.PasswordSignInAsync(username, password, false, false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        _mockSignInManager
            .Setup(s => s.GetExternalAuthenticationSchemesAsync())
            .ReturnsAsync(new List<AuthenticationScheme>());

        // Act
        var result = await _loginModel.OnPostAsync(returnUrl);

        // Assert
        result.Should().BeOfType<LocalRedirectResult>();
        var redirectResult = result as LocalRedirectResult;
        // UrlHelper converts returnUrl to a local path
        redirectResult!.Url.Should().NotBeNullOrEmpty();
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("User logged in")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task CompleteLoginWorkflow_WithInvalidCredentials_ShouldFail()
    {
        // Arrange
        var username = "invaliduser";
        var password = "WrongPassword";

        _loginModel.Input = new LoginModel.InputModel
        {
            Username = username,
            Password = password,
            RememberMe = false
        };

        _mockSignInManager
            .Setup(s => s.PasswordSignInAsync(username, password, false, false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

        _mockSignInManager
            .Setup(s => s.GetExternalAuthenticationSchemesAsync())
            .ReturnsAsync(new List<AuthenticationScheme>());

        // Act
        var result = await _loginModel.OnPostAsync(null);

        // Assert
        result.Should().BeOfType<PageResult>();
        _loginModel.ModelState.IsValid.Should().BeFalse();
        _loginModel.ModelState[string.Empty]!.Errors.Should().Contain(e => e.ErrorMessage == "Invalid login attempt.");
    }

    #endregion

    #region Helper Classes

    /// <summary>
    /// Mock service provider for HttpContext
    /// </summary>
    private class ServiceProviderMock : IServiceProvider
    {
        private readonly IAuthenticationService _authenticationService;

        public ServiceProviderMock(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public object? GetService(Type serviceType)
        {
            if (serviceType == typeof(IAuthenticationService))
            {
                return _authenticationService;
            }
            return null;
        }
    }

    #endregion
}
