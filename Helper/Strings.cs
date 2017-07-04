namespace VAMInsuranceBot.Helper
{
    public static class Strings
    {
        public static readonly string WelcomeOptions = "Welcome to VAM Insurance Bot" + "\n\n" + "Are you an existing customer or do you want to buy a new policy?" + "\n\n" + "1. Existing" + "\n\n" + "2. New" + "\n\n" + "Please enter the correct choice:";
        public static readonly string PanNo = "Please enter your PAN Number:";
        public static readonly string DOB = "Please enter your date of birth (Format: MM/DD/YYYY): ";
        public static readonly string IncorrectPan = "The PAN number you entered does not correspond to any customer! Please try again.";
        public static readonly string IncorrectDOBPan = "The PAN number and the Date of Birth do not correspond. Please enter you PAN number: ";
        public static readonly string PolicyNumber = "Enter your 10 digit Policy Number: ";
        public static readonly string New = "Welcome to VAM Insurance" + "\n\n" + "Please visit out website www.abcde.com or call on XXXXX for more details." + "\n\n" + "Thank You!!" + "\n\n" + "Have a nice day!";
        public static readonly string WrongChoice = "I cannot understand that." + "\n\n" + "Enter again:";
        public static readonly string EnterOtp = "You will need to authenticate for yourself for the next steps with the OTP which has been sent to your registered email ID." + "\n\n" + "Please enter the OTP (valid for 10 minutes): ";
        public static readonly string IncorrectPolicy = "The policy you have entered is incorrect." + "\n\n" + "Please enter again: ";
        public static readonly string PolicyOptions ="What would you like to do?" + "\n\n" + "1. View your policy" + "\n\n" + "2. Renew your policy" + "\n\n" + "3. File an insurance claim" + "\n\n" + "4. Navigate to another policy" + "\n\n" + "5. Leave the conversation" + "\n\n" + "Please enter the suitable options number:";
        public static readonly string IncorrectOtp = "The OTP that you have entered is either incorrect or expired." + "\n\n" + "Your session has also expired." + "\n\b" + "Please try again.";
        public static readonly string RenewOptions = "." + "\n\n" + "Do you want to file a renew for the policy?" + "\n\n" + "1. Yes" + "\n\n" + "2. No" + "\n\n" + "Please enter your choice: ";
        public static readonly string ClaimOptions = "Claim should be registered under which criteria?" + "\n\n" + "1. Damage" + "\n\n" + "2. Theft" + "\n\n" + "Enter your choice:";
        public static readonly string DuplicateClaim = "You are trying to file a duplicate claim." + "\n\n";
        public static readonly string PolicyExpired = "Sorry your Policy has expired. Cannot file the claim. Please contact the office or your agent." + "\n\n" + "redirecting to the main menu." + "\n\n" + "Thank You!";
        public static readonly string RenewFiled = "Your renew has been filed." + "\n\n" + "Thank you!";
        public static readonly string ThankYou = "Thank You!!" + "\n\n" + "Have a nice day!";
        public static readonly string NoPic = "The message you sent did not contain any image. Please upload the picture again.";
        public static readonly string ClaimFiled = "Your claim has been filed for review." + "\n\n" + "Do you want to return to the main menu?" + "\n\n" + "If yes, please enter \"yes\" else enter \"no\"";
        public static readonly string SessionExpired = "Sorry, your session has expired. Start again.";
        public static readonly string AccidentDate = "On what date did the accident take place? (Format: MM/DD/YYYY)";
        public static readonly string AccidentLocation = "Where did the accident take place?";
        public static readonly string AccidentDescription = "Please describe your accident: ";
        public static readonly string AccidentThirdParty = "Is there a third party involved?";
        public static readonly string VehicleRegistration= "What was the registration number of your vehicle involved in the accident?";
        public static readonly string VehicleMake = "Which is the manufacturing company of the vehicle?";
        public static readonly string VehicleModel = "What is the model name of your vehicle";
        public static readonly string VehicleClass = "What class is your vehicle, \'Private\' or \'Commercial\'?";
        public static readonly string InvalidVehicleClass = "Your vehicle class has to be one of \'Private\' or \'Commercial\'! Please enter again: ";
        public static readonly string InvalidVehicleRegNo = "That is not a valid registration number. Please enter again:";
        public static readonly string VehicleType = "What type is your vehicle? (Example: \'Two wheeler\', \'Sedan\', etc.)";
        public static readonly string DriverName = "What is the name of the person who was driving the vehicle at the time of the accident?";
        public static readonly string DriverRelation = "How is the driver related to you? (If you were the driver, type \'Self\')";
        public static readonly string DriverLicense = "Please enter the 16-digit license number of the driver:";
        public static readonly string DriverInjured = "Was the driver injured?" + "\n\n" + "\'Yes\' or \'No\'";
        public static readonly string DriverLicensePic = "Please upload a picture of the license of the driver:";
        public static readonly string InvalidDriverLicense = "That is not a valid license number. Please enter again:";
        public static readonly string VehicleRCPic = "Please upload a picture of your vehicle's RC:";
        public static readonly string FIRPic = "Please upload a picture of your FIR.";
        public static readonly string VehicleTheftPic = "Please upload a picture of your vehicle showing the loss:";
        public static readonly string VehicleDamagePic = "Please upload a picture of the damaged vehicle showing the damage:";
        public static readonly string AnotherVehicleDamage = "Do you want to upload another picture of your damaged vehicle?" + "\n\n" + "\'Yes\' or \'No\'";
        public static readonly string ChangeAnyFields = "Would you like to change any of the fields?" + "\n\n" + "\'Yes\' or \'No\'";
        public static readonly string FieldEdit = "Which field would you like to edit? (Example: Accident Date or Vehicle Registration Number)";
        public static readonly string ChangeAnotherField = "Would you like to change any other field?" + "\n\n" + "\'Yes\' or \'No\'";
        public static readonly string TheftDate = "On what date did the theft take place? (Format: MM/DD/YYYY)";
        public static readonly string TheftType = "Is it a \'Partial\' or a \'Total\' loss?";
        public static readonly string InvalidTheftType = "I don't recognise that category! Please enter again:";
        public static readonly string TheftLocation = "Where did the theft take place?";
        public static readonly string TheftDescription = "Please mention the parts stolen:";
        public static readonly string InvalidDate = "That is an invalid date! Please enter again:";
        public static readonly string InvalidDateFormat = "Invalid date! Please enter the date in the given format:";
        public static readonly string ThirdPartyRegistration = "Please enter the vehicle registration number of the third party: ";
        public static readonly string ThirdPartyLicense = "Please enter the driver license number of the third party: ";
        public static readonly string ThirdPartyInjury = "Is the third party driver injured?" + "\n\n" + "\'Yes\' or \'No\'";
        public static readonly string FIRFiled = "Have you filed an FIR against the third party?" + "\n\n" + "\'Yes\' or \'No\'";
    }
}