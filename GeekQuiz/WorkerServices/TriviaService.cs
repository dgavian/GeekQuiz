using GeekQuiz.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GeekQuiz.WorkerServices
{
  public class TriviaService : ITriviaService
  {
    private readonly TriviaContext _db;

    public TriviaService(TriviaContext db)
    {
      if (db == null)
      {
        throw new ArgumentNullException("db");
      }
      _db = db;
    }

    public async Task<TriviaQuestion> NextQuestionAsync(string userId)
    {
      var lastQuestionId = await _db.TriviaAnswers
        .Where(a => a.UserId == userId)
        .GroupBy(a => a.QuestionId)
        .Select(g => new { QuestionId = g.Key, Count = g.Count() })
        .OrderByDescending(q => new { q.Count, QuestionId = q.QuestionId })
        .Select(q => q.QuestionId)
        .FirstOrDefaultAsync();

      var questionsCount = await _db.TriviaQuestions.CountAsync();

      var nextQuestionId = (lastQuestionId % questionsCount) + 1;
      return await _db.TriviaQuestions.FirstOrDefaultAsync(q => q.Id == nextQuestionId);
    }

    public async Task<bool> StoreAsync(TriviaAnswer answer)
    {
      _db.TriviaAnswers.Add(answer);

      await _db.SaveChangesAsync();
      var selectedOption = await _db.TriviaOptions
        .FirstOrDefaultAsync(o => o.Id == answer.OptionId
        && o.QuestionId == answer.QuestionId);

      return selectedOption.IsCorrect;
    }
  }
}