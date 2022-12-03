using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace kogerohu
{
    /// <summary>
    /// Copy company phone from related Account main phone column if exists when Account is changed.
    /// This operation should commit before data will be saved => PreOperation
    /// </summary>
    public class ContactCopyCompanyPhone : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext executionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Obtain the Organization Service factory service from the service provider
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService orgService = factory.CreateOrganizationService(executionContext.UserId);
            
            // The InputParameters collection contains all the data passed in the message request.
            var isTarget = executionContext.InputParameters.Contains("Target");
            var isTargetTypeEntity = executionContext.InputParameters["Target"] is Entity;

            if (!isTarget || !isTargetTypeEntity) return;

            // Obtain the target entity from the input parameters.
            var target = (Entity)executionContext.InputParameters["Target"];

            // If there is no related account do nothing
            if (!target.Attributes.ContainsKey("parentcustomerid")) return;

            var accountRef = (EntityReference)target.Attributes["parentcustomerid"];

            // Getting account with phone number
            var columns = new ColumnSet("telephone1");
            var account = orgService.Retrieve("account", accountRef.Id, columns);

            // If no main phone, do nothing
            if (account.Attributes["telephone1"] == null) return;

            var mainPhone = (string)account.Attributes["telephone1"];

            // Copy account's main phone to contact's company phone
            target.Attributes["company"] = mainPhone;

            // Since target goes further on the execution pipeline
            // And main stage is next, our data will be saved and
            // we don't have to save our data manually.
        }
    }
}
