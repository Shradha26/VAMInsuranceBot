using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using VAMInsuranceBot.Access;
using VAMInsuranceBot.Helper;
using VAMInsuranceBot.Process;
using VAMInsuranceBot.Dialog.IDialog;

#pragma warning disable CS1998

namespace VAMInsuranceBot.Dialog.Dialogs
{
    [Serializable]
    public class DefaultDialog : IDefaultDialog
    {
        public readonly string dt = "DateTime";
        public readonly string lmdt = "LastMessageDateTime";
        public readonly string pn = "PolicyNumber";
        public readonly string pan = "PanNumber";
        private string temp;

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(WelcomeAsync);
        }

        public async Task WelcomeAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            message.SetBotConversationData(dt, DateTime.UtcNow);
            StorageAccess.StoreTemporaryLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), "user", message.Text);

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(lmdt)))
            {
                await context.PostAsync(Strings.WelcomeOptions);
                StorageAccess.StoreTemporaryLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), "bot", Strings.WelcomeOptions);
                context.Wait(AlreadyNewAsync); 
            }
            else
            {
                await context.PostAsync(Strings.SessionExpired);
                StorageAccess.StoreTemporaryLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), "bot", Strings.SessionExpired);
                context.Wait(WelcomeAsync);
            }
        }

        public async Task AlreadyNewAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            StorageAccess.StoreTemporaryLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), "user", message.Text);

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(lmdt)))
            {
                if (message.Text.ToLower().Contains("existing") || message.Text.Contains("1"))
                {
                    await context.PostAsync(Strings.PanNo);
                    StorageAccess.StoreTemporaryLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), "bot", Strings.PanNo);
                    context.Wait(PanNumberAsync);
                }
                else if (message.Text.ToLower().Contains("new") || message.Text.Contains("2"))
                {
                    await context.PostAsync(Strings.New);
                    StorageAccess.StoreTemporaryLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), "bot", Strings.New);
                    context.Wait(WelcomeAsync);
                }
                else
                {
                    await context.PostAsync(Strings.WrongChoice);
                    StorageAccess.StoreTemporaryLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), "bot", Strings.WrongChoice);
                    context.Wait(AlreadyNewAsync);
                }
            }
            else
            {
                await context.PostAsync(Strings.SessionExpired);
                StorageAccess.StoreTemporaryLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), "bot", Strings.SessionExpired);
                context.Wait(WelcomeAsync);
            }
        }

        public async Task PanNumberAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            StorageAccess.StoreTemporaryLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), "user", message.Text);

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(lmdt)))
            {
                if (DBAccess.CheckPanNumber(message.Text))
                {
                    message.SetBotUserData(pan, message.Text);
                    await context.PostAsync(Strings.DOB);
                    StorageAccess.StoreTemporaryLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), "bot", Strings.DOB);
                    context.Wait(DOBAsync);
                }
                else
                {
                    await context.PostAsync(Strings.IncorrectPan);
                    StorageAccess.StoreTemporaryLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), "bot", Strings.IncorrectPan);
                    context.Wait(PanNumberAsync);
                } 
            }
            else
            {
                await context.PostAsync(Strings.SessionExpired);
                StorageAccess.StoreTemporaryLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), "bot", Strings.SessionExpired);
                context.Wait(WelcomeAsync);
            }
        }

        public async Task DOBAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            StorageAccess.StoreTemporaryLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), "user", message.Text);

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(lmdt)))
            {
                if (ClaimDetails.CheckDateFormat(message.Text))
                {
                    if (DBAccess.CheckDOB(message.GetBotUserData<string>(pan), message.Text))
                    {
                        DBAccess.GenerateOtpAndSendEmail(message.GetBotUserData<string>(pan));
                        StorageAccess.StoreTemporaryLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), panNo: message.GetBotUserData<string>(pan));

                        await context.PostAsync(Strings.EnterOtp);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), message.GetBotUserData<string>(pan), "bot", Strings.EnterOtp);
                        context.Wait(AuthenicateClientAsync);
                    }
                    else
                    {
                        await context.PostAsync(Strings.IncorrectDOBPan);
                        StorageAccess.StoreTemporaryLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), "bot", Strings.IncorrectDOBPan);
                        context.Wait(PanNumberAsync);
                    }
                }
                else
                {
                    await context.PostAsync(Strings.InvalidDateFormat);
                    StorageAccess.StoreTemporaryLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), "bot", Strings.InvalidDateFormat);
                    context.Wait(DOBAsync);
                } 
            }
            else
            {
                await context.PostAsync(Strings.SessionExpired);
                StorageAccess.StoreTemporaryLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), "bot", Strings.SessionExpired);
                context.Wait(WelcomeAsync);
            }
        }

        public async Task AuthenicateClientAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), message.GetBotUserData<string>(pan), "user", message.Text);

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(lmdt)))
            {
                if (true/*DBAccess.AuthenticateOTP(message.Text)*/)
                {
                    await context.PostAsync(temp="Hi, " + DBAccess.GetClientName(message.GetBotUserData<string>(pan)) + "\n\n" + "Your policies are: " + "\n\n" + DBAccess.GetPolicies(message.GetBotUserData<string>(pan)) + "\n\n" + "Please enter a policy number: ");
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), message.GetBotUserData<string>(pan), "bot", temp);
                    context.Wait(PolicyNumberAsync);
                }
                else
                {
                    await context.PostAsync(Strings.IncorrectOtp);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), message.GetBotUserData<string>(pan), "bot", Strings.IncorrectOtp);
                    context.Wait(WelcomeAsync);
                }
            }
            else
            {
                await context.PostAsync(Strings.SessionExpired);
                StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), message.GetBotUserData<string>(pan), "bot", Strings.SessionExpired);
                context.Wait(WelcomeAsync);
            }
        }

        public async Task PolicyNumberAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), message.GetBotUserData<string>(pan), "user", message.Text);

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(lmdt)))
            {
                if (DBAccess.CheckPolicyNumber(message.Text))
                {
                    message.SetBotUserData(pn, message.Text);
                    await context.PostAsync(Strings.PolicyOptions);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), message.GetBotUserData<string>(pan), "bot", Strings.PolicyOptions);
                    context.Wait(ViewRenewFileClaimAsync);
                }
                else
                {
                    await context.PostAsync(Strings.IncorrectPolicy);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), message.GetBotUserData<string>(pan), "bot", Strings.IncorrectPolicy);
                    context.Wait(PolicyNumberAsync);
                }
            }
            else
            {
                await context.PostAsync(Strings.SessionExpired);
                StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), message.GetBotUserData<string>(pan), "bot", Strings.SessionExpired);
                context.Wait(WelcomeAsync);
            }
        }

        public async Task ViewRenewFileClaimAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), message.GetBotUserData<string>(pan), "user", message.Text, message.GetBotUserData<string>(pn));

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(lmdt)))
            {
                double days;
                if (DBAccess.CheckExpiration(message.GetBotUserData<string>(pn), out days))
                {
                    await context.PostAsync(temp = "Your policy expires in " + ((long)days).ToString() + " days! Please care to file a renew.");
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), message.GetBotUserData<string>(pan), "bot", temp, message.GetBotUserData<string>(pn));
                }

                if (message.Text.Contains("1") || message.Text.ToLower().Contains("view"))
                {
                    await context.PostAsync(temp = DBAccess.ViewData(message.GetBotUserData<string>(pn)) + "\n\n" + Strings.PolicyOptions);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), message.GetBotUserData<string>(pan), "bot", temp, message.GetBotUserData<string>(pn));
                    context.Wait(ViewRenewFileClaimAsync);
                }
                else if (message.Text.Contains("2") || message.Text.ToLower().Contains("renew"))
                {
                    await context.PostAsync(temp = "Your policy expires on " + DBAccess.GetRenewDate(message.GetBotUserData<string>(pn)).ToString() + Strings.RenewOptions);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), message.GetBotUserData<string>(pan), "bot", temp, message.GetBotUserData<string>(pn));
                    context.Wait(RenewAsync);
                }
                else if (message.Text.Contains("3") || message.Text.ToLower().Contains("file") || message.Text.ToLower().Contains("claim"))
                {
                    if (DBAccess.ValidatePolicy(message.GetBotUserData<string>(pn)))
                    {
                        DateTime? cd;
                        string cn;
                        if (!(DBAccess.DuplicateClaim(message.GetBotUserData<string>(pn), out cd, out cn)))
                        {
                            await context.PostAsync(Strings.ClaimOptions);
                            StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), message.GetBotUserData<string>(pan), "bot", Strings.ClaimOptions, message.GetBotUserData<string>(pn));
                            context.Wait(DamageOrTheftAsync); 
                        }
                        else
                        {
                            await context.PostAsync(temp = Strings.DuplicateClaim + "Your claim filed on " + cd.Value + " is still in \'OPEN\' state with Claim Number: " + cn + "\n\n" + Strings.PolicyOptions);
                            StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), message.GetBotUserData<string>(pan), "bot", temp, message.GetBotUserData<string>(pn));
                            context.Wait(ViewRenewFileClaimAsync);
                        }
                    }
                    else
                    {
                        await context.PostAsync(Strings.PolicyExpired);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), message.GetBotUserData<string>(pan), "bot", Strings.PolicyExpired, message.GetBotUserData<string>(pn));
                        context.Wait(WelcomeAsync);
                    }
                }
                else if (message.Text.Contains("4") || message.Text.ToLower().Contains("another") || message.Text.ToLower().Contains("policy"))
                {
                    await context.PostAsync(temp = "Your policies are: " + "\n\n" + DBAccess.GetPolicies(message.GetBotUserData<string>(pan)) + "\n\n" + "Please enter a policy number: ");
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), message.GetBotUserData<string>(pan), "bot", temp, message.GetBotUserData<string>(pn));
                    context.Wait(PolicyNumberAsync);
                }
                else if (message.Text.Contains("5") || message.Text.ToLower().Contains("leave") || message.Text.ToLower().Contains("exit") || message.Text.ToLower().Contains("quit"))
                {
                    
                    await context.PostAsync(temp = "Thank you!!" + "\n\n" + "Bye.");
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), message.GetBotUserData<string>(pan), "bot", temp, message.GetBotUserData<string>(pn));
                    context.Wait(WelcomeAsync);
                }
                else
                {
                    await context.PostAsync(Strings.WrongChoice);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), message.GetBotUserData<string>(pan), "bot", Strings.WrongChoice, message.GetBotUserData<string>(pn));
                    context.Wait(ViewRenewFileClaimAsync);
                }
            }
            else
            {
                await context.PostAsync(Strings.SessionExpired);
                StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), message.GetBotUserData<string>(pan), "bot", Strings.SessionExpired, message.GetBotUserData<string>(pn));
                context.Wait(WelcomeAsync);
            }
        }

        public async Task RenewAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), message.GetBotUserData<string>(pan), "user", message.Text, message.GetBotUserData<string>(pn));

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(lmdt)))
            {
                if (message.Text.Contains("1") || message.Text.Contains("2") || message.Text.ToLower().Contains("yes") || message.Text.ToLower().Contains("no"))
                {
                    if (message.Text.Contains("1") || message.Text.ToLower().Contains("yes"))
                    {
                        DBAccess.FileRenew(message.GetBotUserData<string>(pn));
                        await context.PostAsync(Strings.RenewFiled);
                        StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), message.GetBotUserData<string>(pan), "bot", Strings.RenewFiled, message.GetBotUserData<string>(pn));
                    }
                    await context.PostAsync(Strings.PolicyOptions);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), message.GetBotUserData<string>(pan), "bot", Strings.PolicyOptions, message.GetBotUserData<string>(pn));
                    context.Wait(ViewRenewFileClaimAsync);
                }
                else
                {
                    await context.PostAsync(Strings.WrongChoice);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), message.GetBotUserData<string>(pan), "bot", Strings.WrongChoice, message.GetBotUserData<string>(pn));
                    context.Wait(RenewAsync);
                }
            }
            else
            {
                await context.PostAsync(Strings.SessionExpired);
                StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), message.GetBotUserData<string>(pan), "bot", Strings.SessionExpired, message.GetBotUserData<string>(pn));
                context.Wait(WelcomeAsync);
            }
        }

        public async Task DamageOrTheftAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), message.GetBotUserData<string>(pan), "user", message.Text, message.GetBotUserData<string>(pn));

            if (Session.CheckSession(message.GetBotConversationData<DateTime>(lmdt)))
            {
                if (message.Text.Contains("1") || message.Text.ToLower().Contains("damage"))
                {
                    DamageDialog dd = new DamageDialog();
                    await context.PostAsync(Strings.AccidentDate);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), message.GetBotUserData<string>(pan), "bot", Strings.AccidentDate, message.GetBotUserData<string>(pn));
                    context.Wait(dd.GetAccidentDateAsync);
                }
                else if (message.Text.Contains("2") || message.Text.ToLower().Contains("theft"))
                {
                    TheftDialog td = new TheftDialog();
                    await context.PostAsync(Strings.TheftDate);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), message.GetBotUserData<string>(pan), "bot", Strings.TheftDate, message.GetBotUserData<string>(pn));
                    context.Wait(td.GetTheftDateAsync);
                }
                else
                {
                    await context.PostAsync(Strings.WrongChoice);
                    StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), message.GetBotUserData<string>(pan), "bot", Strings.WrongChoice, message.GetBotUserData<string>(pn));
                    context.Wait(DamageOrTheftAsync);
                }
            }
            else
            {
                await context.PostAsync(Strings.SessionExpired);
                StorageAccess.StoreStructuredLog(StorageAccess.GetDateTime(message.GetBotConversationData<DateTime>(dt)), message.GetBotUserData<string>(pan), "bot", Strings.SessionExpired, message.GetBotUserData<string>(pn));
                context.Wait(WelcomeAsync);
            }
        }
    }
}