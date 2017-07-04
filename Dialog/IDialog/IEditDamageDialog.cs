using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Threading.Tasks;

namespace VAMInsuranceBot.Dialog.IDialog
{
    public interface IEditDamageDialog
    {
        Task EditDamageClaimDetailsConfirmationAsync(IDialogContext context, IAwaitable<Message> argument);
        Task EditDamageClaimDetailsAsync(IDialogContext context, IAwaitable<Message> argument);
    }
}
