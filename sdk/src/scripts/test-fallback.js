// Fallback script loader: if the CDN script failed to load, load from local origin.
if (typeof window !== 'undefined') {
    var cdnScript = document.querySelector('script[src="https://cdn.squidex.io/scripts/test.js"]');
    if (cdnScript && cdnScript.dataset && !cdnScript.dataset.loaded) {
        var fallbackScript = document.createElement('script');
        fallbackScript.src = '/scripts/test.js';
        document.head.appendChild(fallbackScript);
    }
}
