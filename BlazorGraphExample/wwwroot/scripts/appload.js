// MSAL loads the entire app in an iframe...
// This nasty appload.js makes it only load the minimal login script needed inside the iframe to complete the auth call.
if (window !== window.parent && !window.opener) {
    // in iframe - just load MSAL script...
    if (window.parent.applicationConfig) {
        var msalJs = document.createElement('script');
        msalJs.src = 'scripts/msal/msal.min.js';
        msalJs.onload = function () {
            var loginJs = document.createElement('script');
            loginJs.src = 'scripts/login.js';
            loginJs.onload = function () {
                var url = document.location.href;
                initMSAL(window.parent.applicationConfig);
            };
            document.body.appendChild(loginJs);
        };
        document.body.appendChild(msalJs);
    }
} else {
    document.writeln('<app>Loading...</app>');
    document.writeln('<script src="_framework/components.webassembly.js"></script>');
    document.writeln('<script src="https://cdnjs.cloudflare.com/ajax/libs/bluebird/3.3.4/bluebird.min.js"></script>');
    document.writeln('<script src="https://cdnjs.cloudflare.com/ajax/libs/fetch/2.0.3/fetch.min.js"></script>');
    document.writeln('<script src="scripts/msal/msal.min.js" type="text/javascript"></script>');
    document.writeln('<script src="scripts/login.js"></script>');
}