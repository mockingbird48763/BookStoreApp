using Microsoft.EntityFrameworkCore;

namespace BookStore.Services.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        // Arrange
        int x = 100;

        // Act
        x += 100;

        // Assert
        Assert.Equal(200, x);
    }
}