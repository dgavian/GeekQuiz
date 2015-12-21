using GeekQuiz.Models;
using GeekQuiz.WorkerServices;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Security.Principal;
using System;

namespace GeekQuiz.Controllers
{
  [Authorize]
  public class TriviaController : ApiController
  {
    private readonly ITriviaService _service;
    public TriviaController(ITriviaService service)
    {
      if (service == null)
      {
        throw new ArgumentNullException("service");
      }
      _service = service;
    }

    private IPrincipal _currentUser;
    public IPrincipal CurrentUser
    { 
      get
      {
        if (_currentUser == null)
        {
          // Lazy initialization of Local Default.
          CurrentUser = User;
        }
        return _currentUser;
      }
      set
      {
         if (value == null)
         {
           throw new ArgumentNullException("value");
         }
        if (_currentUser != null)
        {
          // Only allow Dependency to be defined once.
          throw new InvalidOperationException();
        }
        _currentUser = value;
      }
    }

    [ResponseType(typeof(TriviaQuestion))]
    public async Task<IHttpActionResult> Get()
    {
      var userId = CurrentUser.Identity.Name;

      TriviaQuestion nextQuestion = await _service.NextQuestionAsync(userId);

      if (nextQuestion == null)
      {
        return NotFound();
      }

      return Ok(nextQuestion);
    }

    [ResponseType(typeof(TriviaAnswer))]
    public async Task<IHttpActionResult> Post(TriviaAnswer answer)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      answer.UserId = CurrentUser.Identity.Name;

      var isCorrect = await _service.StoreAsync(answer);
      // Should return 201.
      return Ok<bool>(isCorrect);
    }
  }
}
