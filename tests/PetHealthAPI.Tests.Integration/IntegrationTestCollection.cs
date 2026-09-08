using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace PetHealthAPI.Tests.Integration
{
    [CollectionDefinition("Integration Tests")]
    public class IntegrationTestCollection : ICollectionFixture<WebApplicationFactory<Program>>
    {
        // Esta classe não precisa de implementação.
        // Ela existe apenas para associar o [CollectionDefinition] à interface ICollectionFixture<T>,
        // permitindo que múltiplas classes de teste compartilhem a mesma instância de WebApplicationFactory.
    }
}