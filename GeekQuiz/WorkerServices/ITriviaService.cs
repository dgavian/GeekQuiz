using GeekQuiz.Models;
using System;
using System.Threading.Tasks;
namespace GeekQuiz.WorkerServices
{
  public interface ITriviaService
  {
    Task<TriviaQuestion> NextQuestionAsync(string userId);
    Task<bool> StoreAsync(TriviaAnswer answer);
  }
}
