using GeekQuiz.Models;
using GeekQuiz.WorkerServices;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace GeekQuiz.Testing.WorkerServices
{
    [TestFixture]
    [Category("NUnit")]
    public class TriviaServiceTest
    {
        private const string _USER = "Test";

        [Test]
        public void TriviaService_Ctor_ThrowsWithNullDb()
        {
            TriviaContext db = null;

            Assert.That(() => MakeSut(db), Throws.ArgumentNullException.With.Message.Contain("db"));
        }

        [Test]
        public async Task NextQuestionAsync_ContextWithMockData_ReturnsExpected()
        {
            var mockContext = GetMockContext();
            var sut = MakeSut(mockContext.Object);
            var expectedId = 1;

            var actual = await sut.NextQuestionAsync(_USER);

            Assert.That(actual.Id, Is.EqualTo(expectedId));
        }

        [Test]
        public async Task StoreAsync_CorrectAnswer_ReturnsTrue()
        {
            var mockContext = GetMockContext();
            var newAnswer = new TriviaAnswer
            {
                Id = 1,
                OptionId = 11,
                QuestionId = 3,
                UserId = _USER
            };
            var sut = MakeSut(mockContext.Object);

            var actual = await sut.StoreAsync(newAnswer);

            Assert.That(actual, Is.True);
        }

        [Test]
        public async Task StoreAsync_IncorrectAnswer_ReturnsFalse()
        {
            var mockContext = GetMockContext();
            var newAnswer = new TriviaAnswer
            {
                Id = 2,
                OptionId = 12,
                QuestionId = 3,
                UserId = _USER
            };
            var sut = MakeSut(mockContext.Object);

            var actual = await sut.StoreAsync(newAnswer);

            Assert.That(actual, Is.False);
        }

        private TriviaService MakeSut(TriviaContext db)
        {
            return new TriviaService(db);
        }

        private Mock<TriviaContext> GetMockContext()
        {
            var questions = GetQuestions();
            var answers = GetAnswers();
            var options = GetOptions();
            var mockQuestions = GetMockQuestions(questions);
            var mockAnswers = GetMockAnswers(answers);
            var mockOptions = GetMockOptions(options);
            var mockContext = new Mock<TriviaContext>();
            mockContext.Setup(c => c.TriviaQuestions).Returns(mockQuestions.Object);
            mockContext.Setup(c => c.TriviaAnswers).Returns(mockAnswers.Object);
            mockContext.Setup(c => c.TriviaOptions).Returns(mockOptions.Object);
            return mockContext;
        }

        private IQueryable<TriviaOption> GetOptions()
        {
            var data = new List<TriviaOption>
              {
                new TriviaOption { Id = 1, Title = "2000", IsCorrect = false, QuestionId = 1 },
                new TriviaOption { Id = 2, Title = "2001", IsCorrect = false, QuestionId = 1 },
                new TriviaOption { Id = 3, Title = "2002", IsCorrect = true, QuestionId = 1 },
                new TriviaOption { Id = 4, Title = "2003", IsCorrect = false, QuestionId = 1 },
                new TriviaOption { Id = 5, Title = "Contoso Ltd.", IsCorrect = false, QuestionId = 2 },
                new TriviaOption { Id = 6, Title = "Initech", IsCorrect = false, QuestionId = 2 },
                new TriviaOption { Id = 7, Title = "Fabrikam, Inc.", IsCorrect = false, QuestionId = 2 },
                new TriviaOption { Id = 8, Title = "Northwind Traders", IsCorrect = true, QuestionId = 2 },
                new TriviaOption { Id = 9, Title = "Network.com", IsCorrect = false, QuestionId = 3 },
                new TriviaOption { Id = 10, Title = "Alpha4.com", IsCorrect = false, QuestionId = 3 },
                new TriviaOption { Id = 11, Title = "Symbolics.com", IsCorrect = true, QuestionId = 3 },
                new TriviaOption { Id = 12, Title = "InterConnect.com", IsCorrect = false, QuestionId = 3 }
              };

            return data.AsQueryable();
        }

        private IQueryable<TriviaQuestion> GetQuestions()
        {
            var questions = new List<TriviaQuestion>();

            questions.Add(new TriviaQuestion
            {
                Id = 1,
                Title = "When was .NET first released?"
            });

            questions.Add(new TriviaQuestion
            {
                Id = 2,
                Title = "What fictional company did Nancy Davolio work for?"
            });

            questions.Add(new TriviaQuestion
            {
                Id = 3,
                Title = "The first and still the oldest domain name on the internet is:"
            });

            return questions.AsQueryable();
        }

        private IQueryable<TriviaAnswer> GetAnswers()
        {
            var data = new List<TriviaAnswer>();
            return data.AsQueryable();
        }

        private static Mock<DbSet<TriviaAnswer>> GetMockAnswers(IQueryable<TriviaAnswer> answers)
        {
            var mockSet = new Mock<DbSet<TriviaAnswer>>();

            mockSet.As<IDbAsyncEnumerable<TriviaAnswer>>().Setup(m => m.GetAsyncEnumerator())
              .Returns(new TestDbAsyncEnumerator<TriviaAnswer>(answers.GetEnumerator()));

            mockSet.As<IQueryable<TriviaAnswer>>().Setup(m => m.Provider)
              .Returns(new TestDbAsyncQueryProvider<TriviaAnswer>(answers.Provider));

            mockSet.As<IQueryable<TriviaAnswer>>().Setup(m => m.Expression).Returns(answers.Expression);
            mockSet.As<IQueryable<TriviaAnswer>>().Setup(m => m.ElementType).Returns(answers.ElementType);
            mockSet.As<IQueryable<TriviaAnswer>>().Setup(m => m.GetEnumerator()).Returns(answers.GetEnumerator());
            return mockSet;
        }

        private static Mock<DbSet<TriviaQuestion>> GetMockQuestions(IQueryable<TriviaQuestion> questions)
        {
            var mockSet = new Mock<DbSet<TriviaQuestion>>();

            mockSet.As<IDbAsyncEnumerable<TriviaQuestion>>().Setup(m => m.GetAsyncEnumerator())
              .Returns(new TestDbAsyncEnumerator<TriviaQuestion>(questions.GetEnumerator()));

            mockSet.As<IQueryable<TriviaQuestion>>().Setup(m => m.Provider)
              .Returns(new TestDbAsyncQueryProvider<TriviaQuestion>(questions.Provider));

            mockSet.As<IQueryable<TriviaQuestion>>().Setup(m => m.Expression).Returns(questions.Expression);
            mockSet.As<IQueryable<TriviaQuestion>>().Setup(m => m.ElementType).Returns(questions.ElementType);
            mockSet.As<IQueryable<TriviaQuestion>>().Setup(m => m.GetEnumerator()).Returns(questions.GetEnumerator());
            return mockSet;
        }

        private static Mock<DbSet<TriviaOption>> GetMockOptions(IQueryable<TriviaOption> options)
        {
            var mockSet = new Mock<DbSet<TriviaOption>>();

            mockSet.As<IDbAsyncEnumerable<TriviaOption>>().Setup(m => m.GetAsyncEnumerator())
              .Returns(new TestDbAsyncEnumerator<TriviaOption>(options.GetEnumerator()));

            mockSet.As<IQueryable<TriviaOption>>().Setup(m => m.Provider)
              .Returns(new TestDbAsyncQueryProvider<TriviaOption>(options.Provider));

            mockSet.As<IQueryable<TriviaOption>>().Setup(m => m.Expression).Returns(options.Expression);
            mockSet.As<IQueryable<TriviaOption>>().Setup(m => m.ElementType).Returns(options.ElementType);
            mockSet.As<IQueryable<TriviaOption>>().Setup(m => m.GetEnumerator()).Returns(options.GetEnumerator());
            return mockSet;
        }
    }
}
