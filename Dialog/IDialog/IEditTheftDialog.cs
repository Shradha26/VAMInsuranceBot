using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Threading.Tasks;

namespace VAMInsuranceBot.Dialog.IDialog
{
    public interface IEditTheftDialog
    {
        Task EditTheftClaimDetailsConfirmationAsync(IDialogContext context, IAwaitable<Message> argument);
        Task EditTheftClaimDetailsAsync(IDialogContext context, IAwaitable<Message> argument);
    }
}