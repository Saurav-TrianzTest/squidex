oidc.Log.setLogger(console);

new oidc.UserManager({ response_mode: 'query' }).signinSilentCallback().catch(error => {
    console.error(error);
});
