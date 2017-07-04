using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Threading.Tasks;

namespace VAMInsuranceBot.Dialog.IDialog
{
    public interface IThirdPartyDialog
    {
        Task GetThirdPartyRegistrationAsync(IDialogContext context, IAwaitable<Message> argument);
        Task GetThirdPartyLicenseAsync(IDialogContext context, IAwaitable<Message> argument);
        Task GetThirdPartyInjuryAsync(IDialogContext context, IAwaitable<Message> argument);
        Task GetThirdPartyFIRFiledAsync(IDialogContext context, IAwaitable<Message> argument);
    }
}
