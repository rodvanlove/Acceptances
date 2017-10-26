using System;
using System.Collections;
using System.Text;
using Microsoft.Xrm.Sdk;
using System.Web;
using System.Xml;
using System.Diagnostics;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Crm.Sdk.Messages;
namespace Termination
{
    public class Delegates : IPlugin
    {
        private ITracingService _tracingService;

        public void Execute(IServiceProvider serviceProvider)
        {
            _tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            EntityReference targetEntity = null;
            string relationshipName = string.Empty;
            EntityReferenceCollection relatedEntities = null;
            EntityReference relatedEntity = null;
            Entity building = null;
            //Entity opportunity = null;
            //Entity account = null;
            //Guid opportunityOppID = Guid.Empty;
            //EntityReferenceCollection eqEntites = null;
            //Relationship relationshipEventContact = null;
            Guid opportunityId = Guid.Empty;
            Guid buildingId = Guid.Empty;
            Guid accountId = Guid.Empty;

            try
            {

                #region Associate
                if (context.MessageName.ToLower() == "associate")
                //if (context.MessageName.ToLower() == "associate" || context.MessageName.ToLower() == "disassociate")
                {
                    // Get the “Relationship” Key from context
                    if (context.InputParameters.Contains("Relationship"))
                    {
                        // Get the Relationship name for which this plugin fired
                        relationshipName = ((Relationship)context.InputParameters["Relationship"]).SchemaName;
                    }

                    // Check the "Relationship Name" with your intended one
                    if (relationshipName != "new_opportunity_new_building")
                    {
                        return;
                    }

                    _tracingService.Trace("Association from opportunity to building");

                    // Get Entity EventBooking reference from Target Key from context
                    if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is EntityReference)
                    {
                        targetEntity = (EntityReference)context.InputParameters["Target"];
                        
                        //opportunity = service.Retrieve("opportunity", targetEntity.Id, new ColumnSet("name"));
                        //if (opportunity.Attributes.Contains("name"))
                        //{
                            //var opportunityOpp = (EntityReference)opportunity["name"];
                            //opportunityOppID = opportunityOpp.Id;
                            opportunityId = targetEntity.Id;
                        //}

                        _tracingService.Trace("Target Entity = {0}", opportunityId.ToString());
                    }

                    // Get Entity Contact reference from RelatedEntities Key from context
                    if (context.InputParameters.Contains("RelatedEntities") && context.InputParameters["RelatedEntities"] is EntityReferenceCollection)
                    {
                        relatedEntities = context.InputParameters["RelatedEntities"] as EntityReferenceCollection;
                        relatedEntity = relatedEntities[0];
                        _tracingService.Trace("Related Entity = {0}", relatedEntity.Id.ToString());
                        //eqEntites = new EntityReferenceCollection();
                        //eqEntites.Add(new EntityReference("new_building", relatedEntities[0].Id));
                        building = service.Retrieve("new_building", relatedEntity.Id, new ColumnSet("new_providerid"));
                        _tracingService.Trace("Retrieved {0}", building.Id);
                        accountId = new Guid(building["new_providerid"].ToString());
                        _tracingService.Trace("Provider = {0}", accountId.ToString());
                    }

                    if (targetEntity.Id != null && relatedEntity.Id != null)
                    {
                        //    relationshipEventContact = new Relationship("new_new_events_contacts");
                        //    if (context.MessageName.ToLower() == "associate")
                        //    {
                        //        service.Associate("new_event", eventEntID, relationshipEventContact, eqEntites);
                        //    }
                        //    if (context.MessageName.ToLower() == "disassociate")
                        //    {
                        //        service.Disassociate("new_event", eventEntID, relationshipEventContact, eqEntites);
                        //    }
                        Entity acceptance = new Entity("new_acceptance");

                        acceptance.Attributes["new_opportunityid"] = new EntityReference("opportunity", opportunityId);
                        acceptance.Attributes["new_providerid"] = new EntityReference("account", accountId);

                        var acceptanceId = service.Create(acceptance);

                        _tracingService.Trace("{0} {1} created, ", acceptance.LogicalName, acceptanceId);
                    }
                    else
                    {
                        _tracingService.Trace("Associate Plugin not completed successfully");
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                _tracingService.Trace(string.Format("Associate Plugin error: {0}", ex.Message ));
            }
        }
    }
}