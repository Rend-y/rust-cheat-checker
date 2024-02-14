using System.Diagnostics.CodeAnalysis;
using RCC.Modules.SteamInformation;
using Xunit;
using Xunit.Abstractions;

namespace RCC.Test
{
    [ExcludeFromCodeCoverage]
    public class SteamDataTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public SteamDataTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ConstructorWithSteamId_PopulatesDataCorrectly()
        {
            var steamId = 76561198012345678;

            var steamData = new SteamData(steamId);

            Assert.Equal(steamId, steamData.SteamId);
            Assert.NotNull(steamData.Username);
            Assert.NotNull(steamData.AvatarUrl);
            Assert.False(steamData.IsDeleted);
            Assert.NotNull(steamData.GetAccountAvatar);
        }

        [Theory]
        [InlineData(12345678901234567, true)]
        [InlineData(1234567890, false)]
        public void IsSteamId_ValidatesCorrectly(long steamId, bool expectedResult)
        {
            var result = SteamData.IsSteamId(steamId);

            Assert.Equal(expectedResult, result);
        }
    }
}