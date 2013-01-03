#PortBridge

A fork of Clemens Vasters'
[PortBridge](http://vasters.com/clemensv/PermaLink,guid,3e35d8bd-b755-453f-8c63-1a57c570eb4c.aspx). PortBridge20100610.zip  
Documentaion here is extract from the [original post](http://vasters.com/clemensv/PermaLink,guid,3e35d8bd-b755-453f-8c63-1a57c570eb4c.aspx) by Clemens Vasters.

## Updated for Visual Studio 2010.

Fixed a bug in firewall rules preventing rules with high numbered IP
addresses from matching.

## Updated for Visual Studio 2012 & Windows Azure SDK 1.8
Removed installation projects, as they are not supported in Visual Studio 2012.
The new InstallShield project type (free one) does not support installation for
Windows Service compontents.

## How-to install windows service written in .NET

Siply `xcopy` deploy your service to a folder of your will. Open `Administrator` command prompt, then execute the following command:  

`installutil x:\full\path\to\Your\Windows\Service.exe`  

[Here](http://msdn.microsoft.com/en-us/library/sd8zc8ha(v=vs.100).aspx) is a quick startup with InstallUtil, and [here](http://msdn.microsoft.com/en-us/library/50614e95(v=vs.100).aspx) is complete documentation on the tool.

## PortBridge service
The service's exe file is "PortBridge.exe" and is both a console app and a Windows Service. If the Windows Service isn't registered, the app will always start as a console app. If the Windows Service is registered (with the installer or with installutil.exe) you can force console-mode with the `-c` command line option.  

The app.config file on the Service Side (PortBridge/app.config, PortBridge.exe.config in the binaries folder) specifies what ports or named pipes you want to project into Service Bus:   
   

```XML
  <portBridge serviceBusNamespace="mynamespace"  
			  serviceBusIssuerName="owner"  
			  serviceBusIssuerSecret="xxxxxxxx"  
			  localHostName="mybox">  
    <hostMappings>  
      <add targetHost="localhost" allowedPorts="3389" />  
    </hostMappings>  
  </portBridge>    
````
  
The `serviceBusNamespace` attribute takes your Service Bus namespace name, and the `serviceBusIssuerSecret` the respective secret. The serviceBusIssuerName should remain "owner" unless you know why you want to change it. If you don't have an Azure account you might not understand what I'm writing about: [Go make one](http://www.windowsazure.com/).  

The `localHostName` attribute is optional and when set, it's the name that's being used to map "localhost" into your Service Bus namespace. By default the name that's being used is the good old Windows computer-name.

The `hostMappings` section contains a list of hosts and rules for what you want to project out to Service Bus. Mind that all inbound connections to the endpoints generated from the host mappings section are protected by the Access Control service and require a token that grants access to your namespace - which is already very different from opening up a port in your firewall. If you open up port 3389 (Remote Desktop) through your firewall and NAT, everyone can walk up to that port and try their password-guessing skills. If you open up port 3389 via Port Bridge, you first need to get through the Access Control gate before you can even get at the remote port. 

New host mappings are added with the add element. You can add any host that the machine running the Port Bridge service can "see" via the network. The allowedPorts and allowedPipes attributes define with TCP ports and/or which local named pipes are accessible. Examples:

`<add targetHost="localhost" allowedPorts="3389" />` project the local machine into Service Bus and only allow Remote Desktop (3389)   
`<add targetHost="localhost" allowedPorts="3389,1433" />` project the local machine into Service Bus and allow Remote Desktop (3389) and SQL Server TDS (1433)   
`<add targetHost="localhost" allowedPorts="*" />` project the local machine into Service Bus and only allow any TCP port connection   
`<add targetHost="localhost" allowedPipes="sql/query" />` project the local machine into Service Bus and allow no TCP connections but all named pipe connections to \.\pipes\sql\query   
`<add targetHost="otherbox" allowedPorts="1433" />` project the machine "otherbox" into Service Bus and allow SQL Server TDS connections via TCP   

## Agent

The agent's exe file is "PortBridgeAgent.exe" and is also both a console app and a Windows Service.   

The app.config file on the Agent side (PortBridgeAgent/app.config, PortBridgeAgent.exe.config in the binaries folder) specifies which ports or pipes you want to project into the Agent machine and whether and how you want to firewall these ports. The firewall rules here are not interacting with your local firewall. This is an additional layer of protection.  
```XML
  <portBridgeAgent serviceBusNamespace="mysolution"  
				   serviceBusIssuerName="owner"  
				   serviceBusIssuerSecret="xxxxxxxx">  
    <portMappings>  
      <port localTcpPort="13389" targetHost="mymachine" remoteTcpPort="3389">  
        <firewallRules>  
          <rule source="127.0.0.1" />  
          <rule sourceRangeBegin="10.0.0.0" sourceRangeEnd="10.255.255.255" />  
        </firewallRules>  
      </port>  
    </portMappings>  
  </portBridgeAgent>  
```  
Again, the `serviceBusNamespace` attribute takes your Service Bus namespace name, and the `serviceBusIssuerSecret` the respective secret.   

The `portMappings` collection holds the individual ports or pipes you want to bring onto the local machine. Shown above is a mapping of Remote Desktop (port 3389 on the machine with the computer name or localHostName 'mymachine') to the local port 13389. Once Service and Agent are running, you can connect to the agent machine on port 13389 using the Remote Desktop client - with PortBridge mapping that to port 3389 on the remote box.   

The `firewallRules` collection allows (un-)constraining the TCP clients that may connect to the projected port. By default, only connections from the same machine are permitted.  

For named pipes, the configuration is similar, even though there are no firewall rules and named pipes are always constrained to local connectivity by a set of ACLs that are applied to the pipe. Pipe names must be relative. Here's how a named pipe projection of a default SQL Server instance could look like:  
`
     <port localPipe="sql/remote" targetHost="mymachine" remotePipe="sql/query"/>  
`  


Rob Blackwell / Anton Staykov  
January 2012

