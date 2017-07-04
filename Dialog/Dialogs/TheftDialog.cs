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
    public class TheftDialog : IVehicleDialog, ITheftDialog, ITheftPictureDialog, IEditTheftDialog
    {
        private vehicle veh = new vehicle();
        private theft thef = new theft();
        private DefaultDialog dd;
        private bool has_value;
        private string temp;

        public async Task GetTheftDateAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "user", message.Text, message.GetBotUserData<string>(dd.pn));
            dd = new DefaultDialog();

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(dd.lmdt)))
            {
                has_value = thef.Date == DateTime.MinValue ? false : true;

                if (ClaimDetails.CheckDateFormat(message.Text))
                {
                    if (ClaimDetails.ValidateDate(Convert.ToDateTime(message.Text), message.GetBotUserData<string>(dd.pn)))
                    {
                        thef.Date = Convert.ToDateTime(message.Text);

                        if (!has_value)
                        {
                            await context.PostAsync(Strings.TheftType);
                            StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.TheftType, message.GetBotUserData<string>(dd.pn));
                            context.Wait(GetTheftTypeAsync);
                        }
                        else
                        {
                            await context.PostAsync(Strings.ChangeAnotherField);
                            StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.ChangeAnotherField, message.GetBotUserData<string>(dd.pn));
                            context.Wait(EditTheftClaimDetailsConfirmationAsync);
                        }
                    }
                    else
                    {
                        await context.PostAsync(Strings.InvalidDate);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.InvalidDate, message.GetBotUserData<string>(dd.pn));
                        context.Wait(GetTheftDateAsync);
                    }
                }
                else
                {
                    await context.PostAsync(Strings.InvalidDateFormat);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.InvalidDateFormat, message.GetBotUserData<string>(dd.pn));
                    context.Wait(GetTheftDateAsync);
                } 
            }
            else
            {
                await context.PostAsync(Strings.SessionExpired);
                StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.SessionExpired, message.GetBotUserData<string>(dd.pn));
                context.Wait(dd.WelcomeAsync);
            }
        }

        public async Task GetTheftTypeAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "user", message.Text, message.GetBotUserData<string>(dd.pn));

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(dd.lmdt)))
            {
                string msg = message.Text.ToLower();

                if (msg.Contains("partial") || msg.Contains("total"))
                {
                    has_value = thef.Type.Length != 0 ? true : false;
                    thef.Type = msg.Contains("partial") ? "partial" : "total";

                    if (!has_value)
                    {
                        await context.PostAsync(Strings.TheftLocation);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.TheftLocation, message.GetBotUserData<string>(dd.pn));
                        context.Wait(GetTheftLocationAsync);
                    }
                    else
                    {
                        await context.PostAsync(Strings.ChangeAnotherField);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.ChangeAnotherField, message.GetBotUserData<string>(dd.pn));
                        context.Wait(EditTheftClaimDetailsConfirmationAsync);
                    }
                }
                else
                {
                    await context.PostAsync(Strings.InvalidTheftType);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.InvalidTheftType, message.GetBotUserData<string>(dd.pn));
                    context.Wait(GetTheftTypeAsync);
                } 
            }
            else
            {
                await context.PostAsync(Strings.SessionExpired);
                StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.SessionExpired, message.GetBotUserData<string>(dd.pn));
                context.Wait(dd.WelcomeAsync);
            }
        }

        public async Task GetTheftLocationAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "user", message.Text, message.GetBotUserData<string>(dd.pn));

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(dd.lmdt)))
            {
                has_value = thef.Location.Length != 0 ? true : false;
                thef.Location = message.Text;

                if (!has_value)
                {
                    if (thef.Type == "partial")
                    {
                        await context.PostAsync(Strings.TheftDescription);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.TheftDescription, message.GetBotUserData<string>(dd.pn));
                        context.Wait(GetTheftDescriptionAsync);
                    }
                    else
                    {
                        await context.PostAsync(Strings.VehicleRegistration);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.VehicleRegistration, message.GetBotUserData<string>(dd.pn));
                        context.Wait(GetVehicleRegNoAsync);
                    }

                }
                else
                {
                    await context.PostAsync(Strings.ChangeAnotherField);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.ChangeAnotherField, message.GetBotUserData<string>(dd.pn));
                    context.Wait(EditTheftClaimDetailsConfirmationAsync);
                } 
            }
            else
            {
                await context.PostAsync(Strings.SessionExpired);
                StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.SessionExpired, message.GetBotUserData<string>(dd.pn));
                context.Wait(dd.WelcomeAsync);
            }
        }

        public async Task GetTheftDescriptionAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "user", message.Text, message.GetBotUserData<string>(dd.pn));

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(dd.lmdt)))
            {
                has_value = thef.Description.Length != 0 ? true : false;
                thef.Description = message.Text;

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
                    context.Wait(EditTheftClaimDetailsConfirmationAsync);
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
                        context.Wait(EditTheftClaimDetailsConfirmationAsync);
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
                    context.Wait(EditTheftClaimDetailsConfirmationAsync);
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

            has_value = veh.Model.Length != 0 ? true : false;
            veh.Model = message.Text;

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(dd.lmdt)))
            {
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
                    context.Wait(EditTheftClaimDetailsConfirmationAsync);
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
                    veh.Class = message.Text;

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
                        context.Wait(EditTheftClaimDetailsConfirmationAsync);
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
                    await context.PostAsync(Strings.FIRPic);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.FIRPic, message.GetBotUserData<string>(dd.pn));
                    context.Wait(GetFIRPicAsync);
                }
                else
                {
                    await context.PostAsync(Strings.ChangeAnotherField);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.ChangeAnotherField, message.GetBotUserData<string>(dd.pn));
                    context.Wait(EditTheftClaimDetailsConfirmationAsync);
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
                        StorageAccess.StorePicture(message.GetBotUserData<string>(dd.pn), message.GetBotUserData<string>(dd.pan), "theft-fir", file.ContentUrl, file.ContentType, StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)));
                    }

                    if (thef.Type == "partial")
                    {
                        await context.PostAsync(Strings.VehicleTheftPic);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.VehicleTheftPic, message.GetBotUserData<string>(dd.pn));
                        context.Wait(GetVehiclePicAsync);
                    }
                    else
                    {
                        await context.PostAsync(temp = ClaimDetails.ViewClaimDetails(veh, thef) + Strings.ChangeAnyFields);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", temp, message.GetBotUserData<string>(dd.pn));
                        context.Wait(EditTheftClaimDetailsConfirmationAsync);
                    }
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
                        StorageAccess.StorePicture(message.GetBotUserData<string>(dd.pn), message.GetBotUserData<string>(dd.pan), "theft-vehicle", file.ContentUrl, file.ContentType, StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)));
                    }

                    await context.PostAsync(temp = ClaimDetails.ViewClaimDetails(veh, thef) + Strings.ChangeAnyFields);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", temp, message.GetBotUserData<string>(dd.pn));
                    context.Wait(EditTheftClaimDetailsConfirmationAsync);
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

        public async Task EditTheftClaimDetailsConfirmationAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "user", message.Text, message.GetBotUserData<string>(dd.pn));

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(dd.lmdt)))
            {
                if (message.Text.ToLower() == "yes")
                {
                    await context.PostAsync(Strings.FieldEdit);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.FieldEdit, message.GetBotUserData<string>(dd.pn));
                    context.Wait(EditTheftClaimDetailsAsync);
                }
                else if (message.Text.ToLower() == "no")
                {
                    string cn;
                    await context.PostAsync(temp = "Your Claim Number is " + ClaimDetails.GenerateClaim(message.GetBotUserData<string>(dd.pn), out cn) + "\n\n" + Strings.PolicyOptions);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", temp, message.GetBotUserData<string>(dd.pn));
                    ClaimDetails.SaveTheftDetails(veh, thef, message.GetBotUserData<string>(dd.pn));
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

        public async Task EditTheftClaimDetailsAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "user", message.Text, message.GetBotUserData<string>(dd.pn));

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(dd.lmdt)))
            {
                string msg = message.Text.ToLower();

                if (msg.Contains("theft"))
                {
                    if (msg.Contains("type"))
                    {
                        await context.PostAsync(Strings.TheftType);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.TheftType, message.GetBotUserData<string>(dd.pn));
                        context.Wait(GetTheftTypeAsync);
                    }
                    else if (msg.Contains("date"))
                    {
                        await context.PostAsync(Strings.TheftDate);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.TheftDate, message.GetBotUserData<string>(dd.pn));
                        context.Wait(GetTheftDateAsync);
                    }
                    else if (msg.Contains("location"))
                    {
                        await context.PostAsync(Strings.TheftLocation);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.TheftLocation, message.GetBotUserData<string>(dd.pn));
                        context.Wait(GetTheftLocationAsync);
                    }
                    else if (msg.Contains("description"))
                    {
                        await context.PostAsync(Strings.TheftDescription);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.TheftDescription, message.GetBotUserData<string>(dd.pn));
                        context.Wait(GetTheftDescriptionAsync);
                    }
                    else
                    {
                        await context.PostAsync(Strings.WrongChoice);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.WrongChoice, message.GetBotUserData<string>(dd.pn));
                        context.Wait(EditTheftClaimDetailsAsync);
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
                        context.Wait(EditTheftClaimDetailsAsync);
                    }
                }
                else
                {
                    await context.PostAsync(Strings.WrongChoice);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dd.dt)), message.GetBotUserData<string>(dd.pan), "bot", Strings.WrongChoice, message.GetBotUserData<string>(dd.pn));
                    context.Wait(EditTheftClaimDetailsAsync);
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
