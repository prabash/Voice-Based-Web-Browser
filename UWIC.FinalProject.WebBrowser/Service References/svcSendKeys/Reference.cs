﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UWIC.FinalProject.WebBrowser.svcSendKeys {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="svcSendKeys.ISendKeysService")]
    public interface ISendKeysService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISendKeysService/PostMessage", ReplyAction="http://tempuri.org/ISendKeysService/PostMessageResponse")]
        string PostMessage(string message);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISendKeysService/PostMessage", ReplyAction="http://tempuri.org/ISendKeysService/PostMessageResponse")]
        System.Threading.Tasks.Task<string> PostMessageAsync(string message);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ISendKeysServiceChannel : UWIC.FinalProject.WebBrowser.svcSendKeys.ISendKeysService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class SendKeysServiceClient : System.ServiceModel.ClientBase<UWIC.FinalProject.WebBrowser.svcSendKeys.ISendKeysService>, UWIC.FinalProject.WebBrowser.svcSendKeys.ISendKeysService {
        
        public SendKeysServiceClient() {
        }
        
        public SendKeysServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public SendKeysServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public SendKeysServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public SendKeysServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string PostMessage(string message) {
            return base.Channel.PostMessage(message);
        }
        
        public System.Threading.Tasks.Task<string> PostMessageAsync(string message) {
            return base.Channel.PostMessageAsync(message);
        }
    }
}
