using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace VAMInsuranceBot.Dialog.IDialog
{
    public interface IDefaultDialog : IDialog<object>
    {
        Task WelcomeAsync(IDialogContext context, IAwaitable<Message> argument);
        Task AlreadyNewAsync(IDialogContext context, IAwaitable<Message> argument);
        Task PolicyNumberAsync(IDialogContext context, IAwaitable<Message> argument);
        Task AuthenicateClientAsync(IDialogContext context, IAwaitable<Message> argument);
        Task ViewRenewFileClaimAsync(IDialogContext context, IAwaitable<Message> argument);
        Task RenewAsync(IDialogContext context, IAwaitable<Message> argument);
        //Task RepeatAsync(IDialogContext context, IAwaitable<Message> argument);
        Task DamageOrTheftAsync(IDialogContext context, IAwaitable<Message> argument);        
    }
}