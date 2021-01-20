using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TroydonFitnessWebsite.Models.Products;
using TroydonFitnessWebsite.Models.UserAccounts;

namespace TroydonFitnessWebsite.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);

        Task SendWelcomeEmailAsync(WelcomeRequest request, string confirmAccountLink, string toEmail, string firstName);
        // TODO: Password reset email
        Task SendPasswordResetAsync(WelcomeRequest request, string confirmAccountLink, string toEmail, string firstName);

        Task UpdateEmailAsync(WelcomeRequest request, string confirmAccountLink, string toEmail, string firstName);

        Task SendRoutineOrderConfirmationEmail(WelcomeRequest request, int numberOfItemsInOrder, string toEmail, string firstName,
            DateTime orderDate, Guid? orderNumber,
            List<string> productNames, List<decimal?> prices, List<int> quantities, List<PersonalTraining.SessionType?> sessionTypes, List<int?> lengthsOfRoutine, List<PersonalTraining.Difficulty?> experienceLevels);
    }
}