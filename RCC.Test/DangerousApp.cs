using System;
using RCC.Modules.DangerousApp;
using Xunit;
using Xunit.Abstractions;

namespace RCC.Test
{
    public class DangerousApp
    {
        private readonly IDangerousApp<SDangerousApplication> _dangerousApp;
        private readonly ITestOutputHelper _testOutputHelper;

        public DangerousApp(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _dangerousApp = new DangerousAppService();
        }

        [Theory]
        [InlineData(data: @"")]
        public void Test_FindAllApplicationInRegistry_With_Empty_String(string registryKey)
        {
            Assert.Throws<ArgumentException>(() => _dangerousApp.FindAllApplicationInRegistry(registryKey));
        }

        [Theory]
        [InlineData(data: @"test\Microsoft\Windows\CurrentVersion\Uninstall")]
        public void Test_FindAllApplicationInRegistry_With_Incorrect_Registry_Key(string registryKey)
        {
            Assert.Throws<InvalidOperationException>(() => _dangerousApp.FindAllApplicationInRegistry(registryKey));
        }

        [Theory]
        [InlineData(data: @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall")]
        public void Test_FindAllApplicationInRegistry_WithRegistryKey(in string registryKey)
        {
            _testOutputHelper.WriteLine(_dangerousApp.FindAllApplicationInRegistry(registryKey).Count.ToString());
            Assert.NotEmpty(_dangerousApp.FindAllApplicationInRegistry(registryKey));
        }
    }
}