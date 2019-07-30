// MSAL loads the entire app in an iframe after successful login.
// This script is designed to detect the re-load and to then load the minimal 
// login script needed inside the iframe to complete the auth call.
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
    var loadingHtml = '<app>' +
        '<div id="pageloader" class="pageloader is-active">' +
            '<span class="title">' +
                '<span class="icon">' +
                    '<i class="oi oi-fire"></i>' +
                '</span>' +
                '<span>' +
                    'Hang tight!' +
                '</span>' +
            '</span>' +
        '</div>' +
    '</app>';

    document.writeln(loadingHtml);
    document.writeln('<script src="_framework/blazor.webassembly.js"></script>');
    document.writeln('<script src="https://cdnjs.cloudflare.com/ajax/libs/bluebird/3.3.4/bluebird.min.js"></script>');
    document.writeln('<script src="https://cdnjs.cloudflare.com/ajax/libs/fetch/2.0.3/fetch.min.js"></script>');
    document.writeln('<script src="scripts/msal/msal.min.js" type="text/javascript"></script>');
    document.writeln('<script src="scripts/login.js"></script>');
}