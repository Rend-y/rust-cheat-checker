using System.Diagnostics.CodeAnalysis;
using RCC.Modules.SteamInformation;
using Xunit;
using Xunit.Abstractions;

namespace RCC.Test
{
    [ExcludeFromCodeCoverage]
    public class SteamInformation
    {
        private readonly string _privateSteamIdAccount;
        private readonly ISteamInformation<SteamData> _steamInformation;
        private readonly ITestOutputHelper _testOutputHelper;

        public SteamInformation(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _steamInformation = new SteamInformationService();
            _privateSteamIdAccount = "76561199006134220";
        }

        [Theory]
        [InlineData("Some text 76561198012345678 and more text", new[] { "76561198012345678" })]
        [InlineData("76561198012345678 and 76561198098765432 are Steam IDs",
            new[] { "76561198012345678", "76561198098765432" })]
        [InlineData("76561198012345678 and 76561198012345678 are overlapping Steam IDs", new[] { "76561198012345678" })]
        [InlineData("Some text 1234567890123456 and more text", new string[0])]
        [InlineData("Steam ID: 765611980123 and more text", new[] { "765611980123" })]
        [InlineData("Text 76561198012345678 MoreText 76561198098765432",
            new[] { "76561198012345678", "76561198098765432" })]
        [InlineData("Text 76561198012345678 moreText 76561198098765432",
            new[] { "76561198012345678", "76561198098765432" })]
        [InlineData("Steam ID: 76561198012345678 and [SteamID:76561198098765432]",
            new[] { "76561198012345678", "76561198098765432" })]
        [InlineData(null, new string[0])]
        [InlineData("", new string[0])]
        [InlineData("NoSteamIdsHere", new string[0])]
        [InlineData("76561198012345678, 76561198098765432, 76561198012345678",
            new[] { "76561198012345678", "76561198098765432" })]
        public void GetSteamIdFromContent_ShouldReturnCorrectSteamIds(string content, params string[] expectedSteamIds)
        {
            var result = _steamInformation.GetSteamIdFromContent(content);
            Assert.Equal(expectedSteamIds, result);
        }
    }
}