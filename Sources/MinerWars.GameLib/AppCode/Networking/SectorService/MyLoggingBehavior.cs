using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using SysUtils.Utils;
using KeenSoftwareHouse.Library.Trace;

namespace MinerWars.AppCode.Networking.SectorService
{
    class MyLoggingBehavior: IEndpointBehavior, IClientMessageInspector
    {
        // INSPECTOR
        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
        }

        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
        {
            var action = request.Headers.Action;
            action = action.Replace("http://tempuri.org/", "");
            action = action.Replace("IMySectorService", "");
            action = action.Replace("/", "");
            string message = "SERVER CALL: " + action;
            MyMwcLog.WriteLine(message);
            MyTrace.Send(TraceWindow.Server, message);
            // Not sure what return
            //return new System.Guid();
            return null;
        }

        // ENDPOINT BEHAVIOR
        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(this);
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }        
    }
}
