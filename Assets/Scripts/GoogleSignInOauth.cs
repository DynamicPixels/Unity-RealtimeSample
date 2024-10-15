using System.Collections;
using System.Collections.Generic;
using i5.Toolkit.Core.OpenIDConnectClient;
using i5.Toolkit.Core.ServiceCore;
using UnityEngine;

public class GoogleSignInOauth : BaseServiceBootstrapper
{
    [SerializeField] private ClientDataObject googleClientDataObject;
    
    protected override void RegisterServices()
    {
        OpenIDConnectService oidc = new OpenIDConnectService();
        oidc.OidcProvider = new GoogleOidcProvider();
        oidc.OidcProvider.ClientData = googleClientDataObject.clientData;
        oidc.RedirectURI = "https://dynamicpixels.dev/";
        ServiceManager.RegisterService(oidc);
    }

    protected override void UnRegisterServices()
    {
        ServiceManager.RemoveService<OpenIDConnectService>();
    }

}
