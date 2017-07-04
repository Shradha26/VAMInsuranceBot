using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Threading.Tasks;

namespace VAMInsuranceBot.Dialog.IDialog
{
    public interface ITheftDialog
    {
        Task GetTheftDateAsync(IDialogContext context, IAwaitable<Message> argument);
        Task GetTheftTypeAsync(IDialogContext context, IAwaitable<Message> argument);
        Task GetTheftLocationAsync(IDialogContext context, IAwaitable<Message> argument);
        Task GetTheftDescriptionAsync(IDialogContext context, IAwaitable<Message> argument);
    }
}
