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

function getUserName(applicationConfig) {

    initMSAL(applicationConfig);
    var user = userAgentApplication.getUser();
    if (user) {
        return user.displayableId;
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
                resolve({
                    idToken: token
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
