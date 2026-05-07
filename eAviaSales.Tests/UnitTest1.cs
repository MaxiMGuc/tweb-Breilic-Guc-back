using eAviaSales.BusinessLogic.Functions.Auth;
using eAviaSales.Domains.Models.User;

namespace eAviaSales.Tests;

public class AuthFlowTests
{
    [Fact]
    public void LoginActionFlow_ReturnsNull_WhenPayloadInvalid()
    {
        var flow = new AuthFlow();
        var request = new UserAuthRequest { Login = "", Password = "" };

        var result = flow.LoginActionFlow(request);

        Assert.Null(result);
    }

    [Fact]
    public void LoginActionFlow_ReturnsToken_WhenPayloadValid()
    {
        var flow = new AuthFlow();
        var request = new UserAuthRequest { Login = "demo_user", Password = "password123" };

        var result = flow.LoginActionFlow(request);

        Assert.NotNull(result);
        Assert.False(string.IsNullOrWhiteSpace(result!.Token));
    }
}
