using Xunit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using PotatoDistroHR.Web.Pages;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tests.PotatoDistroHR.Web.Pages
{
    /// <summary>
    /// Mock HTTP Session for testing
    /// </summary>
    public class MockHttpSession : ISession
    {
        private readonly Dictionary<string, byte[]> _sessionStorage;

        public MockHttpSession(Dictionary<string, byte[]> sessionStorage = null)
        {
            _sessionStorage = sessionStorage ?? new Dictionary<string, byte[]>();
        }

        public string Id => Guid.NewGuid().ToString();
        public bool IsAvailable => true;
        public IEnumerable<string> Keys => _sessionStorage.Keys;

        public void Clear() => _sessionStorage.Clear();
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(string key) => _sessionStorage.Remove(key);
        public void Set(string key, byte[] value) => _sessionStorage[key] = value;
        public bool TryGetValue(string key, out byte[] value) => _sessionStorage.TryGetValue(key, out value);
    }

    /// <summary>
    /// Unit tests for IndexModel
    /// </summary>
    public class IndexModelTests
    {
        [Fact]
        public void Constructor_ShouldInitializeModel()
        {
            // Arrange & Act
            var model = new IndexModel();

            // Assert
            Assert.NotNull(model);
            Assert.IsAssignableFrom<PageModel>(model);
        }

        [Fact]
        public void IsAuthenticated_ShouldDefaultToFalse()
        {
            // Arrange & Act
            var model = new IndexModel();

            // Assert
            Assert.False(model.IsAuthenticated);
        }

        [Fact]
        public void OnGet_WithAuthenticatedUser_ShouldSetIsAuthenticatedToTrue()
        {
            // Arrange
            var model = new IndexModel();
            var sessionData = new Dictionary<string, byte[]>
            {
                { "UserID", BitConverter.GetBytes(123) }
            };
            var mockSession = new MockHttpSession(sessionData);

            var httpContext = new DefaultHttpContext();
            httpContext.Session = mockSession;

            model.PageContext = new PageContext
            {
                HttpContext = httpContext
            };

            // Act
            model.OnGet();

            // Assert
            Assert.True(model.IsAuthenticated);
        }

        [Fact]
        public void OnGet_WithUnauthenticatedUser_ShouldSetIsAuthenticatedToFalse()
        {
            // Arrange
            var model = new IndexModel();
            var mockSession = new MockHttpSession();

            var httpContext = new DefaultHttpContext();
            httpContext.Session = mockSession;

            model.PageContext = new PageContext
            {
                HttpContext = httpContext
            };

            // Act
            model.OnGet();

            // Assert
            Assert.False(model.IsAuthenticated);
        }

        [Fact]
        public void OnGet_ShouldCheckUserIDInSession()
        {
            // Arrange
            var model = new IndexModel();
            var sessionData = new Dictionary<string, byte[]>
            {
                { "UserID", BitConverter.GetBytes(456) }
            };
            var mockSession = new MockHttpSession(sessionData);

            var httpContext = new DefaultHttpContext();
            httpContext.Session = mockSession;

            model.PageContext = new PageContext
            {
                HttpContext = httpContext
            };

            // Act
            model.OnGet();

            // Assert
            Assert.True(model.IsAuthenticated);
        }

        [Fact]
        public void IsAuthenticated_ShouldBeSettable()
        {
            // Arrange
            var model = new IndexModel();

            // Act
            model.IsAuthenticated = true;

            // Assert
            Assert.True(model.IsAuthenticated);
        }

        [Fact]
        public void OnGet_WithZeroUserId_ShouldSetIsAuthenticatedToTrue()
        {
            // Arrange
            var model = new IndexModel();
            var sessionData = new Dictionary<string, byte[]>
            {
                { "UserID", BitConverter.GetBytes(0) }
            };
            var mockSession = new MockHttpSession(sessionData);

            var httpContext = new DefaultHttpContext();
            httpContext.Session = mockSession;

            model.PageContext = new PageContext
            {
                HttpContext = httpContext
            };

            // Act
            model.OnGet();

            // Assert
            Assert.True(model.IsAuthenticated);
        }
    }
}
