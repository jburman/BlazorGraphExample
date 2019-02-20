var userAgentApplication = null;

var logger = new Msal.Logger(loggerCallback, { level: Msal.LogLevel.Error, correlationId: '11111', piiLoggingEnabled: false });

function loggerCallback(logLevel, message, piiLoggingEnabled) {
    console.log(message);
}

function authCallback(errorDesc, token, error, tokenType) {
    if (token) {
        console.log('token acquired');
    } else {
        console.log(error + ":" + errorDesc);
    }
}

function initMSAL(applicationConfig) {
    if (userAgentApplication === null) {
        userAgentApplication = new Msal.UserAgentApplication(applicationConfig.clientId,
            'https://login.microsoftonline.com/common',
            authCallback,
            {
                logger: logger,
                storeAuthStateInCookie: true,
                cacheLocation: 'localStorage',
                navigateToLoginRequestUrl: false
            });
        window.applicationConfig = applicationConfig; // stash a ref to the config for the child iframe to access
    }
}

function createGraphAccount(user) {
    if (user && user.idToken) {
        var idToken = user.idToken;
        var tenantId = idToken.tid;
        var objectId = idToken.oid;
        var accountId = objectId + '.' + tenantId;
        var accountName = user.idToken.preferred_username;
        var expires = new Date();
        expires = new Date(expires.getTime() + idToken.exp);

        var account = {
            accountId: accountId,
            accountName: accountName,
            expires: expires,
            identityProvider: user.identityProvider,
            azureADObjectId: objectId,
            tenantId: tenantId
        };
        return account;
    } else {
        return null;
    }
}

function getUserAccount(applicationConfig) {

    initMSAL(applicationConfig);
    var user = userAgentApplication.getUser();
    return createGraphAccount(user);
}

function loginRedirect(applicationConfig) {
    initMSAL(applicationConfig);
    userAgentApplication.loginRedirect(applicationConfig.scopes);
    return "";
}

function getTokenAsync(applicationConfig) {
    initMSAL(applicationConfig);

    return new Promise(function (resolve, reject) {

        userAgentApplication.acquireTokenSilent(applicationConfig.scopes)
            .then(function (token) {
                var user = userAgentApplication.getUser();
                var account = createGraphAccount(user);
                var expires = new Date();
                expires = new Date(expires.getTime() + user.idToken.exp);

                resolve({
                    idToken: token,
                    expires: expires,
                    accountId: account.accountId
                });
            }, function (error) {
                if (error) {
                    console.info(error);
                    loginRedirect(applicationConfig);
                }
            });
    });
}

function logout(applicationConfig) {
    initMSAL(applicationConfig);
    userAgentApplication.logout();
    return "";
}
