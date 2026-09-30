using TheBand.AuthDomain.Interfaces;
using TheBand.AuthInfrastructure.Repositories;

namespace TheBand.AuthTests;

public sealed class UserRepositoryContractTests
{
    [Fact]
    public void IUserRepository_UserRepository_ImplementsContract()
    {
        Assert.True(typeof(IUserRepository).IsAssignableFrom(typeof(UserRepository)));
    }
}
