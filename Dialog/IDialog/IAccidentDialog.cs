using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Threading.Tasks;

namespace VAMInsuranceBot.Dialog.IDialog
{
    public interface IAccidentDialog
    {
        Task GetAccidentDateAsync(IDialogContext context, IAwaitable<Message> argument);
        Task GetAccidentLocationAsync(IDialogContext context, IAwaitable<Message> argument);
        Task GetAccidentThirdPartyAsync(IDialogContext context, IAwaitable<Message> argument);
        Task GetAccidentDescriptionAsync(IDialogContext context, IAwaitable<Message> argument);
    }
}
