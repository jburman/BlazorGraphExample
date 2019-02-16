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
            null,
            authCallback,
            {
                logger: logger,
                cacheLocation: 'localStorage'
            });
    }
}

function isLoggedIn(applicationConfig) {
    initMSAL(applicationConfig);
    var user = userAgentApplication.getUser();
    if (!user) {
        return false;
    }
    return true;
}

function getUserAccount(applicationConfig) {

    initMSAL(applicationConfig);
    var user = userAgentApplication.getUser();
    if (user) {
        var idToken = user.idToken;
        var tenantId = idToken.tid;
        var objectId = idToken.oid;
        var accountId = objectId + '.' + tenantId;
        var accountName = user.idToken.preferred_username;
        var account = {
            accountId: accountId,
            accountName: accountName,
            identityProvider: user.identityProvider,
            azureObjectId: objectId,
            azureTenantId: tenantId
        };
        return account;
    } else {
        return "";
    }
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
                var expires = new Date();
                expires = new Date(expires.getTime() + user.idToken.exp);

                resolve({
                    idToken: token,
                    expires: expires,
                    account: getUserAccount(applicationConfig)
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
