using Kudu.Contracts.Jobs;
using Kudu.Contracts.Tracing;
using Kudu.Core.Infrastructure;
using Kudu.Services.Jobs;
using Kudu.TestHarness.Xunit;
using Moq;
using System;
using System.IO.Abstractions;
using System.Net;
using System.Net.Http;
using Xunit;

namespace Kudu.Services.Test.Jobs
{
    [KuduXunitTestClass]
    public class InvokeTriggeredJobReadonlyFileSystemTests
    {
        [Fact]
        public void InvokeTriggeredJobShouldReturn503()
        {
            var controller = new JobsController(
                Mock.Of<ITriggeredJobsManager>(),
                Mock.Of<IContinuousJobsManager>(),
                Mock.Of<ITracer>());

            var fileSystem = new Mock<IFileSystem>();
            var dirBase = new Mock<DirectoryBase>();
            fileSystem.Setup(f => f.Directory).Returns(dirBase.Object);
            dirBase.Setup(d => d.CreateDirectory(It.IsAny<string>())).Throws<UnauthorizedAccessException>();
            FileSystemHelpers.Instance = fileSystem.Object;
            FileSystemHelpers.TmpFolder = @"D:\";

            controller.Request = new HttpRequestMessage();

            HttpResponseMessage resMsg = controller.InvokeTriggeredJob("foo");
            Assert.Equal(HttpStatusCode.ServiceUnavailable, resMsg.StatusCode);
        }
    }
}
