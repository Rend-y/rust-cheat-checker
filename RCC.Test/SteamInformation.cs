using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using RCC.Modules.SteamInformation;
using Xunit;
using Xunit.Abstractions;

namespace RCC.Test
{
    [ExcludeFromCodeCoverage]
    public class SteamInformation
    {
        private readonly ISteamInformation<SteamData> _steamInformation;
        private readonly ITestOutputHelper _testOutputHelper;

        public SteamInformation(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _steamInformation = new SteamInformationService();
        }

        [Theory]
        [InlineData(
            "\"SteamID\"	\"76561199169433522\" \"SteamID\"	\"76561199169433532\" \"SteamID\"	\"76561199169433543\" \"SteamID\"	\"76561199169433542\"")]
        public void Test_GetSteamIsFromContent_on_valid_steamId_data(in string content)
        {
            List<string> steamId = _steamInformation.GetSteamIdFromContent(content);
            Assert.Equal(steamId.Count, 4);
            Assert.All(steamId, s => Assert.True(SteamData.IsSteamId(Convert.ToInt64(s))));
        }

        [Theory]
        [InlineData(
            "\"SteamID\"	\"765611991694343522\" \"SteamID\"	\"323\" \"SteamID\"	\"765611994216\" \"SteamID\"	\"7656\"")]
        public void Test_GetSteamIsFromContent_on_not_valid_steamId_data(in string content)
        {
            List<string> steamId = _steamInformation.GetSteamIdFromContent(content);
            Assert.NotEqual(steamId.Count, 4);
            Assert.All(steamId, s => Assert.False(SteamData.IsSteamId(Convert.ToInt64(s))));
        }

        [Theory]
        [InlineData("")]
        public void Test_GetSteamIsFromContent_with_empty_content(in string content)
        {
            var steamId = _steamInformation.GetSteamIdFromContent(content);
            Assert.Equal(steamId.Count, 0);
        }

        [Theory]
        [InlineData(null)]
        public void Test_GetSteamIsFromContent_with_null_content(in string content)
        {
            var steamId = _steamInformation.GetSteamIdFromContent(content);
            Assert.Equal(steamId.Count, 0);
        }
    }
}