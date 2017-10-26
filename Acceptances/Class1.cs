using System;
//using System.Collections;
//using System.Text;
using Microsoft.Xrm.Sdk;
//using System.Web;
//using System.Xml;
//using System.Diagnostics;
using Microsoft.Xrm.Sdk.Query;
//using Microsoft.Xrm.Sdk.Client;
//using Microsoft.Crm.Sdk.Messages;
namespace Termination
{
    public class Delegates : IPlugin
    {
        private ITracingService _tracingService;

        public void Execute(IServiceProvider serviceProvider)
        {
            _tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            if (context.Depth > 1)
                return;
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            EntityReference targetEntity = null;
            string relationshipName = string.Empty;
            EntityReferenceCollection relatedEntities = null;
            EntityReference relatedEntity = null;
            //Entity eventBooking = null;
            //Guid eventEntID = Guid.Empty;
            //EntityReferenceCollection eqEntites = null;
            //Relationship relationshipEventContact = null;

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

                    _tracingService.Trace("Association");

                    // Get Entity EventBooking reference from Target Key from context
                    if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is EntityReference)
                    {
                        targetEntity = (EntityReference)context.InputParameters["Target"];
                        _tracingService.Trace("Target Entity = " + targetEntity.Id.ToString());
                        //eventBooking = service.Retrieve("new_eventbooking", targetEntity.Id, new ColumnSet("new_eventname"));
                        //if (eventBooking.Attributes.Contains("new_eventname"))
                        //{
                        //    var eventEnt = (EntityReference)eventBooking["new_eventname"];
                        //    eventEntID = eventEnt.Id;
                        //}
                    }

                    // Get Entity Contact reference from RelatedEntities Key from context
                    if (context.InputParameters.Contains("RelatedEntities") && context.InputParameters["RelatedEntities"] is EntityReferenceCollection)
                    {
                        relatedEntities = context.InputParameters["RelatedEntities"] as EntityReferenceCollection;
                        relatedEntity = relatedEntities[0];
                        _tracingService.Trace("Related Entity = " + relatedEntity.Id.ToString());
                        //eqEntites = new EntityReferenceCollection();
                        //eqEntites.Add(new EntityReference("contact", relatedEntities[0].Id));
                    }

                    //if (eqEntites != null && eventEntID != null)
                    //{
                    //    relationshipEventContact = new Relationship("new_new_events_contacts");
                    //    if (context.MessageName.ToLower() == "associate")
                    //    {
                    //        service.Associate("new_event", eventEntID, relationshipEventContact, eqEntites);
                    //    }
                    //    if (context.MessageName.ToLower() == "disassociate")
                    //    {
                    //        service.Disassociate("new_event", eventEntID, relationshipEventContact, eqEntites);
                    //    }
                    //    trace.Trace("Event-Delegate Associate & Dissassociate Plugin copleted Successfully");
                    //}
                    //else
                    //{
                    //    trace.Trace("Event-Delegate Associate & Dissassociate Plugin not completed Successfully");
                    //}
                }
                #endregion
            }
            catch (Exception ex)
            {
                trace.Trace(string.Format("Associate Plugin error: {0}", new[] { ex.ToString() }));
            }
        }
    }
}