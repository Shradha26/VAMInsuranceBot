using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Threading.Tasks;

namespace VAMInsuranceBot.Dialog.IDialog
{
    public interface IDriverDialog
    {
        Task GetDriverNameAsync(IDialogContext context, IAwaitable<Message> argument);
        Task GetDriverRelationAsync(IDialogContext context, IAwaitable<Message> argument);
        Task GetDriverLicenseAsync(IDialogContext context, IAwaitable<Message> argument);
        Task GetDriverInjuryAsync(IDialogContext context, IAwaitable<Message> argument);
    }
}
