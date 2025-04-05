using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myrtille.Library;


public enum HttpApplicationStateVariables
{
	Cache,
	RemoteSessions,
	SharedRemoteSessions
}

public enum HttpSessionStateVariables
{
	ClientIP,
	ClientKey,
	EnterpriseSession,
	RemoteSession,
	GuestInfo
}

public enum HttpRequestCookies
{
	ClientKey
}