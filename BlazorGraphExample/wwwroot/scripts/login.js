var userAgentApplication = null;

function initMSAL(applicationConfig) {
    if (userAgentApplication === null) {
        var authConfig = {
            clientId: applicationConfig.clientId,
            authority: 'https://login.microsoftonline.com/common',
            postLogoutRedirectUri: window.location.href // redirect doesn't seem to work ??
        };

        userAgentApplication = new Msal.UserAgentApplication({
            auth: authConfig
        });

        userAgentApplication.handleRedirectCallback((error, response) => {
        });
    }
    window.applicationConfig = applicationConfig; // store for applogin.js to access on callback
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
    var user = userAgentApplication.getAccount();
    return createGraphAccount(user);
}

function doLogin(applicationConfig, success, error) {
    initMSAL(applicationConfig);
    userAgentApplication.loginPopup(applicationConfig)
        .then(function (response) {
            success();
        })
        .catch(function (err) {
            error();
        });
    return "";
}

function getTokenWithRetry(applicationConfig, resolve, reject, retry) {
    userAgentApplication.acquireTokenSilent(applicationConfig)
        .then(function (token) {
            var user = userAgentApplication.getAccount();
            var account = createGraphAccount(user);
            var expires = new Date();
            expires = new Date(expires.getTime() + user.idToken.exp);

            resolve({
                idToken: token.accessToken,
                expires: expires,
                accountId: account.accountId
            });
        }).catch(function (error) {
            if (retry) {
                doLogin(applicationConfig,
                    function () {
                        getTokenWithRetry(applicationConfig, resolve, reject, false);
                    },
                    function () {
                        reject("Unable to acquire token");
                    });
            } else {
                reject("Unable to acquire token");
            }
        });
}

function getTokenAsync(applicationConfig) {
    initMSAL(applicationConfig);

    return new Promise(function (resolve, reject) {

        getTokenWithRetry(applicationConfig, resolve, reject, true);
    });
}

function logout(applicationConfig) {
    initMSAL(applicationConfig);
    userAgentApplication.logout();
    return "";
}
