oidc.Log.setLogger(console);

new oidc.UserManager({ response_mode: 'query' }).signinPopupCallback().catch(error => {
    console.error(error);
});
