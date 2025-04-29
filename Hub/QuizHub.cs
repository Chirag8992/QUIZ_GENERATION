using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Quizgeneration_Project.Hubs
{
    public class QuizHub : Hub
    {
        public async Task JoinQuiz(string quizId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, quizId);
        }

        public async Task SubmitQuiz(string quizId, object answers)
        {
            // Here you can broadcast or just acknowledge
            await Clients.Group(quizId).SendAsync("QuizSubmitted", answers);
        }

        public async Task ForceSubmit(string quizId)
        {
            await Clients.Group(quizId).SendAsync("ForceSubmitQuiz");
        }
    }
}
