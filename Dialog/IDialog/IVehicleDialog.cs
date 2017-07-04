using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Threading.Tasks;

namespace VAMInsuranceBot.Dialog.IDialog
{
    public interface IVehicleDialog
    {
        Task GetVehicleRegNoAsync(IDialogContext context, IAwaitable<Message> argument);
        Task GetVehicleMakeAsync(IDialogContext context, IAwaitable<Message> argument);
        Task GetVehicleModelAsync(IDialogContext context, IAwaitable<Message> argument);
        Task GetVehicleClassAsync(IDialogContext context, IAwaitable<Message> argument);
        Task GetVehicleTypeAsync(IDialogContext context, IAwaitable<Message> argument);
    }
}
