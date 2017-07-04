using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Threading.Tasks;
using VAMInsuranceBot.Access;
using VAMInsuranceBot.Dialog.IDialog;
using VAMInsuranceBot.Helper;
using VAMInsuranceBot.Models;
using VAMInsuranceBot.Process;

namespace VAMInsuranceBot.Dialog.Dialogs
{
    [Serializable]
    public class DamageDialog : IAccidentDialog, IVehicleDialog, IDriverDialog, IDamagePictureDialog, IThirdPartyDialog, IEditDamageDialog
    {
        private accident acc = new accident();
        private vehicle veh = new vehicle();
        private driver dri = new driver();
        private third_party tp = new third_party();
        private DefaultDialog dd;
        private bool has_value;
        private string temp;

        public async Task GetAccidentDateAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "user", message.Text, message.GetBotUserData<string>(dd.pn));
            dd = new DefaultDialog();

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(dd.lmdt)))
            {
                has_value = acc.Date == DateTime.MinValue ? false : true;


                if (ClaimDetails.CheckDateFormat(message.Text))
                {
                    if (ClaimDetails.ValidateDate(Convert.ToDateTime(message.Text), message.GetBotUserData<string>(dd.pn)))
                    {
                        acc.Date = Convert.ToDateTime(message.Text);

                        if (!has_value)
                        {
                            await context.PostAsync(Strings.AccidentLocation);
                            StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.AccidentLocation, message.GetBotUserData<string>(dd.pn));
                            context.Wait(GetAccidentLocationAsync);
                        }
                        else
                        {
                            await context.PostAsync(Strings.ChangeAnotherField);
                            StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.ChangeAnotherField, message.GetBotUserData<string>(dd.pn));
                            context.Wait(EditDamageClaimDetailsConfirmationAsync);
                        }
                    }
                    else
                    {
                        await context.PostAsync(Strings.InvalidDate);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.InvalidDate, message.GetBotUserData<string>(dd.pn));
                        context.Wait(GetAccidentDateAsync);
                    }
                }
                else
                {
                    await context.PostAsync(Strings.InvalidDateFormat);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.InvalidDateFormat, message.GetBotUserData<string>(dd.pn));
                    context.Wait(GetAccidentDateAsync);
                } 
            }
            else
            {
                await context.PostAsync(Strings.SessionExpired);
                StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.SessionExpired, message.GetBotUserData<string>(dd.pn));
                context.Wait(dd.WelcomeAsync);
            }
        }

        public async Task GetAccidentLocationAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "user", message.Text, message.GetBotUserData<string>(dd.pn));

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(dd.lmdt)))
            {
                has_value = acc.Location.Length != 0 ? true : false;
                acc.Location = message.Text;

                if (!has_value)
                {
                    await context.PostAsync(Strings.AccidentThirdParty);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.AccidentThirdParty, message.GetBotUserData<string>(dd.pn));
                    context.Wait(GetAccidentThirdPartyAsync);
                }
                else
                {
                    await context.PostAsync(Strings.ChangeAnotherField);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.ChangeAnotherField, message.GetBotUserData<string>(dd.pn));
                    context.Wait(EditDamageClaimDetailsConfirmationAsync);
                } 
            }
            else
            {
                await context.PostAsync(Strings.SessionExpired);
                StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.SessionExpired, message.GetBotUserData<string>(dd.pn));
                context.Wait(dd.WelcomeAsync);
            }
        }

        public async Task GetAccidentThirdPartyAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "user", message.Text, message.GetBotUserData<string>(dd.pn));

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(dd.lmdt)))
            {
                has_value = acc.Third_Party;
                acc.Third_Party = message.Text.ToLower().Contains("yes") ? true : false;

                if (!has_value)
                {
                    await context.PostAsync(Strings.VehicleRegistration);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.VehicleRegistration, message.GetBotUserData<string>(dd.pn));
                    context.Wait(GetVehicleRegNoAsync);
                }
                else
                {
                    await context.PostAsync(Strings.ChangeAnotherField);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.ChangeAnotherField, message.GetBotUserData<string>(dd.pn));
                    context.Wait(EditDamageClaimDetailsConfirmationAsync);
                } 
            }
            else
            {
                await context.PostAsync(Strings.SessionExpired);
                StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.SessionExpired, message.GetBotUserData<string>(dd.pn));
                context.Wait(dd.WelcomeAsync);
            }
        }

        public async Task GetVehicleRegNoAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "user", message.Text, message.GetBotUserData<string>(dd.pn));

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(dd.lmdt)))
            {
                has_value = veh.Registration_Number.Length != 0 ? true : false;

                if (message.Text.Length >= 12 && message.Text.Length <= 14)
                {
                    veh.Registration_Number = message.Text;

                    if (!has_value)
                    {
                        await context.PostAsync(Strings.VehicleMake);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.VehicleMake, message.GetBotUserData<string>(dd.pn));
                        context.Wait(GetVehicleMakeAsync);
                    }
                    else
                    {
                        await context.PostAsync(Strings.ChangeAnotherField);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.ChangeAnotherField, message.GetBotUserData<string>(dd.pn));
                        context.Wait(EditDamageClaimDetailsConfirmationAsync);
                    }
                }
                else
                {
                    await context.PostAsync(Strings.InvalidVehicleRegNo);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.InvalidVehicleRegNo, message.GetBotUserData<string>(dd.pn));
                    context.Wait(GetVehicleRegNoAsync);
                } 
            }
            else
            {
                await context.PostAsync(Strings.SessionExpired);
                StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.SessionExpired, message.GetBotUserData<string>(dd.pn));
                context.Wait(dd.WelcomeAsync);
            }
        }

        public async Task GetVehicleMakeAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "user", message.Text, message.GetBotUserData<string>(dd.pn));

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(dd.lmdt)))
            {
                has_value = veh.Make.Length != 0 ? true : false;
                veh.Make = message.Text;

                if (!has_value)
                {
                    await context.PostAsync(Strings.VehicleModel);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.VehicleModel, message.GetBotUserData<string>(dd.pn));
                    context.Wait(GetVehicleModelAsync);
                }
                else
                {
                    await context.PostAsync(Strings.ChangeAnotherField);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.ChangeAnotherField, message.GetBotUserData<string>(dd.pn));
                    context.Wait(EditDamageClaimDetailsConfirmationAsync);
                } 
            }
            else
            {
                await context.PostAsync(Strings.SessionExpired);
                StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.SessionExpired, message.GetBotUserData<string>(dd.pn));
                context.Wait(dd.WelcomeAsync);
            }
        }

        public async Task GetVehicleModelAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "user", message.Text, message.GetBotUserData<string>(dd.pn));

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(dd.lmdt)))
            {
                has_value = veh.Model.Length != 0 ? true : false;
                veh.Model = message.Text;

                if (!has_value)
                {
                    await context.PostAsync(Strings.VehicleClass);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.VehicleClass, message.GetBotUserData<string>(dd.pn));
                    context.Wait(GetVehicleClassAsync);
                }
                else
                {
                    await context.PostAsync(Strings.ChangeAnotherField);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.ChangeAnotherField, message.GetBotUserData<string>(dd.pn));
                    context.Wait(EditDamageClaimDetailsConfirmationAsync);
                } 
            }
            else
            {
                await context.PostAsync(Strings.SessionExpired);
                StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.SessionExpired, message.GetBotUserData<string>(dd.pn));
                context.Wait(dd.WelcomeAsync);
            }
        }

        public async Task GetVehicleClassAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "user", message.Text, message.GetBotUserData<string>(dd.pn));

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(dd.lmdt)))
            {
                string msg = message.Text.ToLower();

                if (msg.Contains("private") || msg.Contains("commercial"))
                {
                    has_value = veh.Class.Length != 0 ? true : false;
                    veh.Class = msg.Contains("private") ? "private" : "commercial";

                    if (!has_value)
                    {
                        await context.PostAsync(Strings.VehicleType);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.VehicleType, message.GetBotUserData<string>(dd.pn));
                        context.Wait(GetVehicleTypeAsync);
                    }
                    else
                    {
                        await context.PostAsync(Strings.ChangeAnotherField);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.ChangeAnotherField, message.GetBotUserData<string>(dd.pn));
                        context.Wait(EditDamageClaimDetailsConfirmationAsync);
                    }
                }
                else
                {
                    await context.PostAsync(Strings.InvalidVehicleClass);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.InvalidVehicleClass, message.GetBotUserData<string>(dd.pn));
                    context.Wait(GetVehicleClassAsync);
                } 
            }
            else
            {
                await context.PostAsync(Strings.SessionExpired);
                StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.SessionExpired, message.GetBotUserData<string>(dd.pn));
                context.Wait(dd.WelcomeAsync);
            }
        }

        public async Task GetVehicleTypeAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "user", message.Text, message.GetBotUserData<string>(dd.pn));

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(dd.lmdt)))
            {
                has_value = veh.Type.Length != 0 ? true : false;
                veh.Type = message.Text;

                if (!has_value)
                {
                    await context.PostAsync(Strings.DriverName);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.DriverName, message.GetBotUserData<string>(dd.pn));
                    context.Wait(GetDriverNameAsync);
                }
                else
                {
                    await context.PostAsync(Strings.ChangeAnotherField);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.ChangeAnotherField, message.GetBotUserData<string>(dd.pn));
                    context.Wait(EditDamageClaimDetailsConfirmationAsync);
                } 
            }
            else
            {
                await context.PostAsync(Strings.SessionExpired);
                StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.SessionExpired, message.GetBotUserData<string>(dd.pn));
                context.Wait(dd.WelcomeAsync);
            }
        }

        public async Task GetDriverNameAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "user", message.Text, message.GetBotUserData<string>(dd.pn));

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(dd.lmdt)))
            {
                has_value = dri.Name.Length != 0 ? true : false;
                dri.Name = message.Text;

                if (!has_value)
                {
                    await context.PostAsync(Strings.DriverRelation);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.DriverRelation, message.GetBotUserData<string>(dd.pn));
                    context.Wait(GetDriverRelationAsync);
                }
                else
                {
                    await context.PostAsync(Strings.ChangeAnotherField);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.ChangeAnotherField, message.GetBotUserData<string>(dd.pn));
                    context.Wait(EditDamageClaimDetailsConfirmationAsync);
                } 
            }
            else
            {
                await context.PostAsync(Strings.SessionExpired);
                StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.SessionExpired, message.GetBotUserData<string>(dd.pn));
                context.Wait(dd.WelcomeAsync);
            }
        }

        public async Task GetDriverRelationAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "user", message.Text, message.GetBotUserData<string>(dd.pn));

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(dd.lmdt)))
            {
                has_value = dri.Relation.Length != 0 ? true : false;
                dri.Relation = message.Text;

                if (!has_value)
                {
                    await context.PostAsync(Strings.DriverLicense);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.DriverLicense, message.GetBotUserData<string>(dd.pn));
                    context.Wait(GetDriverLicenseAsync);
                }
                else
                {
                    await context.PostAsync(Strings.ChangeAnotherField);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.ChangeAnotherField, message.GetBotUserData<string>(dd.pn));
                    context.Wait(EditDamageClaimDetailsConfirmationAsync);
                } 
            }
            else
            {
                await context.PostAsync(Strings.SessionExpired);
                StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.SessionExpired, message.GetBotUserData<string>(dd.pn));
                context.Wait(dd.WelcomeAsync);
            }
        }

        public async Task GetDriverLicenseAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "user", message.Text, message.GetBotUserData<string>(dd.pn));

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(dd.lmdt)))
            {
                has_value = dri.License_Number.Length != 0 ? true : false;

                if (message.Text.Length == 16)
                {
                    dri.License_Number = message.Text;

                    if (!has_value)
                    {
                        await context.PostAsync(Strings.DriverInjured);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.DriverInjured, message.GetBotUserData<string>(dd.pn));
                        context.Wait(GetDriverInjuryAsync);
                    }
                    else
                    {
                        await context.PostAsync(Strings.ChangeAnotherField);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.ChangeAnotherField, message.GetBotUserData<string>(dd.pn));
                        context.Wait(EditDamageClaimDetailsConfirmationAsync);
                    }
                }
                else
                {
                    await context.PostAsync(Strings.InvalidDriverLicense);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.InvalidDriverLicense, message.GetBotUserData<string>(dd.pn));
                    context.Wait(GetDriverLicenseAsync);
                } 
            }
            else
            {
                await context.PostAsync(Strings.SessionExpired);
                StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.SessionExpired, message.GetBotUserData<string>(dd.pn));
                context.Wait(dd.WelcomeAsync);
            }
        }

        public async Task GetDriverInjuryAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "user", message.Text, message.GetBotUserData<string>(dd.pn));

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(dd.lmdt)))
            {
                has_value = dri.Injured;
                dri.Injured = message.Text.ToLower().Contains("yes") ? true : false;

                if (!has_value)
                {
                    await context.PostAsync(Strings.DriverLicensePic);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.DriverLicensePic, message.GetBotUserData<string>(dd.pn));
                    context.Wait(GetLicensePicAsync);
                }
                else
                {
                    await context.PostAsync(Strings.ChangeAnotherField);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.ChangeAnotherField, message.GetBotUserData<string>(dd.pn));
                    context.Wait(EditDamageClaimDetailsConfirmationAsync);
                } 
            }
            else
            {
                await context.PostAsync(Strings.SessionExpired);
                StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.SessionExpired, message.GetBotUserData<string>(dd.pn));
                context.Wait(dd.WelcomeAsync);
            }
        }

        public async Task GetLicensePicAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(dd.lmdt)))
            {
                if (message.Attachments.Count > 0)
                {
                    foreach (var file in message.Attachments)
                    {
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "user", file.ContentUrl, message.GetBotUserData<string>(dd.pn));
                        StorageAccess.StorePicture(message.GetBotUserData<string>(dd.pn), message.GetBotUserData<string>(dd.pan), "damage-driver-license", file.ContentUrl, file.ContentType, StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)));
                    }

                    await context.PostAsync(Strings.VehicleRCPic);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.VehicleRCPic, message.GetBotUserData<string>(dd.pn));
                    context.Wait(GetRCPicAsync);
                }
                else
                {
                    await context.PostAsync(Strings.NoPic);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.NoPic, message.GetBotUserData<string>(dd.pn));
                    context.Wait(GetLicensePicAsync);
                } 
            }
            else
            {
                await context.PostAsync(Strings.SessionExpired);
                StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.SessionExpired, message.GetBotUserData<string>(dd.pn));
                context.Wait(dd.WelcomeAsync);
            }
        }

        public async Task GetRCPicAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(dd.lmdt)))
            {
                if (message.Attachments.Count > 0)
                {
                    foreach (var file in message.Attachments)
                    {
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "user", file.ContentUrl, message.GetBotUserData<string>(dd.pn));
                        StorageAccess.StorePicture(message.GetBotUserData<string>(dd.pn), message.GetBotUserData<string>(dd.pan), "damage-vehicle-rc", file.ContentUrl, file.ContentType, StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)));
                    }

                    await context.PostAsync(Strings.VehicleDamagePic);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.VehicleDamagePic, message.GetBotUserData<string>(dd.pn));
                    context.Wait(GetVehiclePicAsync);
                }
                else
                {
                    await context.PostAsync(Strings.NoPic);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.NoPic, message.GetBotUserData<string>(dd.pn));
                    context.Wait(GetRCPicAsync);
                } 
            }
            else
            {
                await context.PostAsync(Strings.SessionExpired);
                StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.SessionExpired, message.GetBotUserData<string>(dd.pn));
                context.Wait(dd.WelcomeAsync);
            }
        }

        public async Task GetVehiclePicAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(dd.lmdt)))
            {
                if (message.Attachments.Count > 0)
                {
                    foreach (var file in message.Attachments)
                    {
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "user", file.ContentUrl, message.GetBotUserData<string>(dd.pn));
                        StorageAccess.StorePicture(message.GetBotUserData<string>(dd.pn), message.GetBotUserData<string>(dd.pan), "damage-vehicle", file.ContentUrl, file.ContentType, StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)));
                    }

                    await context.PostAsync(Strings.AnotherVehicleDamage);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.AnotherVehicleDamage, message.GetBotUserData<string>(dd.pn));
                    context.Wait(GetAnotherVehiclePicAsync);
                }
                else
                {
                    await context.PostAsync(Strings.NoPic);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.NoPic, message.GetBotUserData<string>(dd.pn));
                    context.Wait(GetVehiclePicAsync);
                } 
            }
            else
            {
                await context.PostAsync(Strings.SessionExpired);
                StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.SessionExpired, message.GetBotUserData<string>(dd.pn));
                context.Wait(dd.WelcomeAsync);
            }
        }

        public async Task GetAnotherVehiclePicAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "user", message.Text, message.GetBotUserData<string>(dd.pn));

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(dd.lmdt)))
            {
                if (message.Text.ToLower().Contains("yes") ? true : false)
                {
                    await context.PostAsync(Strings.VehicleDamagePic);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.VehicleDamagePic, message.GetBotUserData<string>(dd.pn));
                    context.Wait(GetVehiclePicAsync);
                }
                else
                {
                    if (acc.Third_Party)
                    {
                        await context.PostAsync(Strings.ThirdPartyRegistration);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.ThirdPartyRegistration, message.GetBotUserData<string>(dd.pn));
                        context.Wait(GetThirdPartyRegistrationAsync);
                    }
                    else
                    {
                        await context.PostAsync(Strings.AccidentDescription);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.AccidentDescription, message.GetBotUserData<string>(dd.pn));
                        context.Wait(GetAccidentDescriptionAsync);
                    }
                } 
            }
            else
            {
                await context.PostAsync(Strings.SessionExpired);
                StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.SessionExpired, message.GetBotUserData<string>(dd.pn));
                context.Wait(dd.WelcomeAsync);
            }
        }

        public async Task GetThirdPartyRegistrationAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "user", message.Text, message.GetBotUserData<string>(dd.pn));

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(dd.lmdt)))
            {
                has_value = tp.Registration_Number.Length != 0 ? true : false;

                if (message.Text.Length >= 12 && message.Text.Length <= 14)
                {
                    tp.Registration_Number = message.Text;

                    if (!has_value)
                    {
                        await context.PostAsync(Strings.ThirdPartyLicense);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.ThirdPartyLicense, message.GetBotUserData<string>(dd.pn));
                        context.Wait(GetThirdPartyLicenseAsync);
                    }
                    else
                    {
                        await context.PostAsync(Strings.ChangeAnotherField);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.ChangeAnotherField, message.GetBotUserData<string>(dd.pn));
                        context.Wait(EditDamageClaimDetailsConfirmationAsync);
                    }
                }
                else
                {
                    await context.PostAsync(Strings.InvalidVehicleRegNo);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.InvalidVehicleRegNo, message.GetBotUserData<string>(dd.pn));
                    context.Wait(GetThirdPartyRegistrationAsync);
                } 
            }
            else
            {
                await context.PostAsync(Strings.SessionExpired);
                StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.SessionExpired, message.GetBotUserData<string>(dd.pn));
                context.Wait(dd.WelcomeAsync);
            }
        }

        public async Task GetThirdPartyLicenseAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "user", message.Text, message.GetBotUserData<string>(dd.pn));

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(dd.lmdt)))
            {
                has_value = tp.License_Number.Length != 0 ? true : false;

                if (message.Text.Length == 16)
                {
                    tp.License_Number = message.Text;

                    if (!has_value)
                    {
                        await context.PostAsync(Strings.ThirdPartyInjury);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.ThirdPartyInjury, message.GetBotUserData<string>(dd.pn));
                        context.Wait(GetThirdPartyInjuryAsync);
                    }
                    else
                    {
                        await context.PostAsync(Strings.ChangeAnotherField);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.ChangeAnotherField, message.GetBotUserData<string>(dd.pn));
                        context.Wait(EditDamageClaimDetailsConfirmationAsync);
                    }
                }
                else
                {
                    await context.PostAsync(Strings.InvalidDriverLicense);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.InvalidDriverLicense, message.GetBotUserData<string>(dd.pn));
                    context.Wait(GetThirdPartyLicenseAsync);
                } 
            }
            else
            {
                await context.PostAsync(Strings.SessionExpired);
                StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.SessionExpired, message.GetBotUserData<string>(dd.pn));
                context.Wait(dd.WelcomeAsync);
            }
        }

        public async Task GetThirdPartyInjuryAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "user", message.Text, message.GetBotUserData<string>(dd.pn));

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(dd.lmdt)))
            {
                has_value = tp.Injured;
                tp.Injured = message.Text.ToLower().Contains("yes") ? true : false;

                if (!has_value)
                {
                    await context.PostAsync(Strings.FIRFiled);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.FIRFiled, message.GetBotUserData<string>(dd.pn));
                    context.Wait(GetThirdPartyFIRFiledAsync);
                }
                else
                {
                    await context.PostAsync(Strings.ChangeAnotherField);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.ChangeAnotherField, message.GetBotUserData<string>(dd.pn));
                    context.Wait(EditDamageClaimDetailsConfirmationAsync);
                } 
            }
            else
            {
                await context.PostAsync(Strings.SessionExpired);
                StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.SessionExpired, message.GetBotUserData<string>(dd.pn));
                context.Wait(dd.WelcomeAsync);
            }
        }

        public async Task GetThirdPartyFIRFiledAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "user", message.Text, message.GetBotUserData<string>(dd.pn));

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(dd.lmdt)))
            {
                has_value = tp.FIR_Filed;
                tp.FIR_Filed = message.Text.ToLower().Contains("yes") ? true : false;

                if (!has_value)
                {
                    if (tp.FIR_Filed)
                    {
                        await context.PostAsync(Strings.FIRPic);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.FIRPic, message.GetBotUserData<string>(dd.pn));
                        context.Wait(GetFIRPicAsync);
                    }
                    else
                    {
                        await context.PostAsync(Strings.AccidentDescription);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.AccidentDescription, message.GetBotUserData<string>(dd.pn));
                        context.Wait(GetAccidentDescriptionAsync);
                    }
                }
                else
                {
                    await context.PostAsync(Strings.ChangeAnotherField);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.ChangeAnotherField, message.GetBotUserData<string>(dd.pn));
                    context.Wait(EditDamageClaimDetailsConfirmationAsync);
                } 
            }
            else
            {
                await context.PostAsync(Strings.SessionExpired);
                StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.SessionExpired, message.GetBotUserData<string>(dd.pn));
                context.Wait(dd.WelcomeAsync);
            }
        }

        public async Task GetFIRPicAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(dd.lmdt)))
            {
                if (message.Attachments.Count > 0)
                {
                    foreach (var file in message.Attachments)
                    {
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "user", file.ContentUrl, message.GetBotUserData<string>(dd.pn));
                        StorageAccess.StorePicture(message.GetBotUserData<string>(dd.pn), message.GetBotUserData<string>(dd.pan), "damage-thirdParty-fir", file.ContentUrl, file.ContentType, StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)));
                    }

                    await context.PostAsync(Strings.AccidentDescription);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.AccidentDescription, message.GetBotUserData<string>(dd.pn));
                    context.Wait(GetAccidentDescriptionAsync);
                }
                else
                {
                    await context.PostAsync(Strings.NoPic);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.NoPic, message.GetBotUserData<string>(dd.pn));
                    context.Wait(GetFIRPicAsync);
                } 
            }
            else
            {
                await context.PostAsync(Strings.SessionExpired);
                StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.SessionExpired, message.GetBotUserData<string>(dd.pn));
                context.Wait(dd.WelcomeAsync);
            }
        }

        public async Task GetAccidentDescriptionAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "user", message.Text, message.GetBotUserData<string>(dd.pn));

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(dd.lmdt)))
            {
                acc.Description = message.Text;
                await context.PostAsync(temp = ClaimDetails.ViewClaimDetails(veh, acc: acc, dri: dri, tp: tp) + Strings.ChangeAnyFields);
                StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", temp, message.GetBotUserData<string>(dd.pn));
                context.Wait(EditDamageClaimDetailsConfirmationAsync); 
            }
            else
            {
                await context.PostAsync(Strings.SessionExpired);
                StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.SessionExpired, message.GetBotUserData<string>(dd.pn));
                context.Wait(dd.WelcomeAsync);
            }
        }

        public async Task EditDamageClaimDetailsConfirmationAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "user", message.Text, message.GetBotUserData<string>(dd.pn));

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(dd.lmdt)))
            {
                if (message.Text.ToLower() == "yes")
                {
                    await context.PostAsync(Strings.FieldEdit);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.FieldEdit, message.GetBotUserData<string>(dd.pn));
                    context.Wait(EditDamageClaimDetailsAsync);
                }
                else if (message.Text.ToLower() == "no")
                {
                    string cn;
                    await context.PostAsync(temp = "Your Claim Number is " + ClaimDetails.GenerateClaim(message.GetBotUserData<string>(dd.pn), out cn) + "\n\n" + Strings.PolicyOptions);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", temp, message.GetBotUserData<string>(dd.pn));
                    ClaimDetails.SaveDamageDetails(message.GetBotUserData<string>(dd.pn), veh, dri, acc);
                    context.Wait(dd.ViewRenewFileClaimAsync);
                } 
            }
            else
            {
                await context.PostAsync(Strings.SessionExpired);
                StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.SessionExpired, message.GetBotUserData<string>(dd.pn));
                context.Wait(dd.WelcomeAsync);
            }
        }

        public async Task EditDamageClaimDetailsAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "user", message.Text, message.GetBotUserData<string>(dd.pn));

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(dd.lmdt)))
            {
                string msg = message.Text.ToLower();

                if (msg.Contains("accident"))
                {
                    if (msg.Contains("date"))
                    {
                        await context.PostAsync(Strings.AccidentDate);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.AccidentDate, message.GetBotUserData<string>(dd.pn));
                        context.Wait(GetAccidentDateAsync);
                    }
                    else if (msg.Contains("location"))
                    {
                        await context.PostAsync(Strings.AccidentLocation);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.AccidentLocation, message.GetBotUserData<string>(dd.pn));
                        context.Wait(GetAccidentLocationAsync);
                    }
                    else if (msg.Contains("description"))
                    {
                        await context.PostAsync(Strings.AccidentDescription);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.AccidentDescription, message.GetBotUserData<string>(dd.pn));
                        context.Wait(GetAccidentDescriptionAsync);
                    }
                    else if (msg.Contains("party"))
                    {
                        await context.PostAsync(Strings.AccidentThirdParty);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.AccidentThirdParty, message.GetBotUserData<string>(dd.pn));
                        context.Wait(GetAccidentThirdPartyAsync);
                    }
                    else
                    {
                        await context.PostAsync(Strings.WrongChoice);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.WrongChoice, message.GetBotUserData<string>(dd.pn));
                        context.Wait(EditDamageClaimDetailsAsync);
                    }
                }
                else if (msg.Contains("driver"))
                {
                    if (msg.Contains("name"))
                    {
                        await context.PostAsync(Strings.DriverName);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.DriverName, message.GetBotUserData<string>(dd.pn));
                        context.Wait(GetDriverNameAsync);
                    }
                    else if (msg.Contains("relation"))
                    {
                        await context.PostAsync(Strings.DriverRelation);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.DriverRelation, message.GetBotUserData<string>(dd.pn));
                        context.Wait(GetDriverRelationAsync);
                    }
                    else if (msg.Contains("license"))
                    {
                        await context.PostAsync(Strings.DriverLicense);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.DriverLicense, message.GetBotUserData<string>(dd.pn));
                        context.Wait(GetDriverLicenseAsync);
                    }
                    else if (msg.Contains("injur"))
                    {
                        await context.PostAsync(Strings.DriverInjured);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.DriverInjured, message.GetBotUserData<string>(dd.pn));
                        context.Wait(GetDriverInjuryAsync);
                    }
                    else
                    {
                        await context.PostAsync(Strings.WrongChoice);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.WrongChoice, message.GetBotUserData<string>(dd.pn));
                        context.Wait(EditDamageClaimDetailsAsync);
                    }
                }
                else if (msg.Contains("vehicle"))
                {
                    if (msg.Contains("registration"))
                    {
                        await context.PostAsync(Strings.VehicleRegistration);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.VehicleRegistration, message.GetBotUserData<string>(dd.pn));
                        context.Wait(GetVehicleRegNoAsync);
                    }
                    else if (msg.Contains("make"))
                    {
                        await context.PostAsync(Strings.VehicleMake);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.VehicleMake, message.GetBotUserData<string>(dd.pn));
                        context.Wait(GetVehicleMakeAsync);
                    }
                    else if (msg.Contains("model"))
                    {
                        await context.PostAsync(Strings.VehicleModel);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.VehicleModel, message.GetBotUserData<string>(dd.pn));
                        context.Wait(GetVehicleModelAsync);
                    }
                    else if (msg.Contains("class"))
                    {
                        await context.PostAsync(Strings.VehicleClass);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.VehicleClass, message.GetBotUserData<string>(dd.pn));
                        context.Wait(GetVehicleClassAsync);
                    }
                    else if (msg.Contains("type"))
                    {
                        await context.PostAsync(Strings.VehicleType);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.VehicleType, message.GetBotUserData<string>(dd.pn));
                        context.Wait(GetVehicleTypeAsync);
                    }
                    else
                    {
                        await context.PostAsync(Strings.WrongChoice);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.WrongChoice, message.GetBotUserData<string>(dd.pn));
                        context.Wait(EditDamageClaimDetailsAsync);
                    }
                }
                else if (msg.Contains("third") || msg.Contains("party"))
                {
                    if (msg.Contains("registration"))
                    {
                        await context.PostAsync(Strings.ThirdPartyRegistration);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.ThirdPartyRegistration, message.GetBotUserData<string>(dd.pn));
                        context.Wait(GetThirdPartyRegistrationAsync);
                    }
                    else if (msg.Contains("license"))
                    {
                        await context.PostAsync(Strings.ThirdPartyLicense);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.ThirdPartyLicense, message.GetBotUserData<string>(dd.pn));
                        context.Wait(GetThirdPartyLicenseAsync);
                    }
                    else if (msg.Contains("injur"))
                    {
                        await context.PostAsync(Strings.ThirdPartyInjury);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.ThirdPartyInjury, message.GetBotUserData<string>(dd.pn));
                        context.Wait(GetThirdPartyInjuryAsync);
                    }
                    else if (msg.Contains("fir"))
                    {
                        await context.PostAsync(Strings.FIRFiled);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.FIRFiled, message.GetBotUserData<string>(dd.pn));
                        context.Wait(GetThirdPartyFIRFiledAsync);
                    }
                    else
                    {
                        await context.PostAsync(Strings.WrongChoice);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.WrongChoice, message.GetBotUserData<string>(dd.pn));
                        context.Wait(EditDamageClaimDetailsAsync);
                    }
                }
                else
                {
                    await context.PostAsync(Strings.WrongChoice);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.WrongChoice, message.GetBotUserData<string>(dd.pn));
                    context.Wait(EditDamageClaimDetailsAsync);
                } 
            }
            else
            {
                await context.PostAsync(Strings.SessionExpired);
                StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.SessionExpired, message.GetBotUserData<string>(dd.pn));
                context.Wait(dd.WelcomeAsync);
            }
        }
    }
}