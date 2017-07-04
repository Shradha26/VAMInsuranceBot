using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Threading.Tasks;

namespace VAMInsuranceBot.Dialog.IDialog
{
    public interface IDamagePictureDialog : ITheftPictureDialog
    {
        Task GetLicensePicAsync(IDialogContext context, IAwaitable<Message> argument);
        Task GetVehiclePicAsync(IDialogContext context, IAwaitable<Message> argument);
        Task GetAnotherVehiclePicAsync(IDialogContext context, IAwaitable<Message> argument);
        Task GetRCPicAsync(IDialogContext context, IAwaitable<Message> argument);
    }
}
