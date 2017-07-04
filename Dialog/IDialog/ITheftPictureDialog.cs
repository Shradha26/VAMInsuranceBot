using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Threading.Tasks;

namespace VAMInsuranceBot.Dialog.IDialog
{
    public interface ITheftPictureDialog
    {
        Task GetFIRPicAsync(IDialogContext context, IAwaitable<Message> argument);
    }
}
