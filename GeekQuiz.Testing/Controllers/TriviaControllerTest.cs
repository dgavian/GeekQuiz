using GeekQuiz.Controllers;
using GeekQuiz.Models;
using GeekQuiz.WorkerServices;
using Moq;
using NUnit.Framework;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Http.Results;

namespace GeekQuiz.Testing.Controllers
{
    [TestFixture]
    [Category("NUnit")]
    public class TriviaControllerTest
    {
        private const string _USER = "Test";

        [Test]
        public void TriviaController_Ctor_ThrowsWithNullService()
        {
            ITriviaService service = null;
            var user = MakeUser();

            Assert.That(() => MakeSut(service, user), Throws.ArgumentNullException);
        }

        [Test]
        public void CurrentUser_SetterWithNullValue_Throws()
        {
            var service = new Mock<ITriviaService>().Object;
            IPrincipal user = null;

            Assert.That(() => MakeSut(service, user), Throws.ArgumentNullException);
        }

        [Test]
        public void CurrentUser_SetterSetMoreThanOnce_Throws()
        {
            var service = new Mock<ITriviaService>().Object;
            var sut = MakeSut(service);
            // Getting lazy loaded value invokes the setter.
            var currentUser = sut.CurrentUser;

            Assert.That(() => sut.CurrentUser = MakeUser(), Throws.InvalidOperationException);
        }

        [Test]
        public void CurrentUser_SetOnce_ReturnsExpectedUserName()
        {
            var userName = "TestSetCurrentUser";
            var service = new Mock<ITriviaService>().Object;
            var sut = MakeSut(service);

            sut.CurrentUser = MakeUser(userName);

            Assert.That(sut.CurrentUser.Identity.Name, Is.EqualTo(userName));
        }

        [Test]
        public async Task Get_ServiceReturnsValidQuestion_Returns200()
        {
            var service = new Mock<ITriviaService>();
            var response = new TriviaQuestion();
            service.Setup(s => s.NextQuestionAsync(It.IsAny<string>())).ReturnsAsync(response);
            var user = MakeUser();
            var sut = MakeSut(service.Object, user);

            var actual = await sut.Get() as OkNegotiatedContentResult<TriviaQuestion>;

            Assert.That(actual, Is.Not.Null);
        }

        [Test]
        public async Task Get_ServiceReturnsNullQuestion_Returns404()
        {
            var service = new Mock<ITriviaService>();
            TriviaQuestion response = null;
            service.Setup(s => s.NextQuestionAsync(It.IsAny<string>())).ReturnsAsync(response);
            var user = MakeUser();
            var sut = MakeSut(service.Object, user);

            var actual = await sut.Get() as NotFoundResult;

            Assert.That(actual, Is.Not.Null);
        }

        [Test]
        public async Task Post_ServiceWithMockData_ReturnsExpectedValue()
        {
            var service = new Mock<ITriviaService>();
            service.Setup(s => s.StoreAsync(It.IsAny<TriviaAnswer>())).ReturnsAsync(true);
            var user = MakeUser();
            var sut = MakeSut(service.Object, user);
            var answer = new TriviaAnswer();

            var actual = await sut.Post(answer) as OkNegotiatedContentResult<bool>;

            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.Content, Is.True);
        }

        private TriviaController MakeSut(ITriviaService service)
        {
            var result = new TriviaController(service);
            return result;
        }

        private TriviaController MakeSut(ITriviaService service, IPrincipal user)
        {
            var result = new TriviaController(service);
            result.CurrentUser = user;
            return result;
        }

        private IPrincipal MakeUser(string userName = _USER)
        {
            var mockIdentity = new Mock<IIdentity>();
            mockIdentity.Setup(i => i.Name).Returns(userName);
            var mockPrincipal = new Mock<IPrincipal>();
            mockPrincipal.Setup(p => p.Identity).Returns(mockIdentity.Object);
            return mockPrincipal.Object;
        }
    }
}
