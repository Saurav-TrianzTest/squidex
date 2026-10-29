function grow(element) {
    element.style.height = "5px";
    element.style.height = (element.scrollHeight)+"px";
}

var element = document.getElementById('editor');

element.addEventListener('input', function() {
    grow(element);
});

// When the plugin is instantiated it notifies the UI that it has been loaded.
// 
// Furthermore it sends the current size to the parent.
var plugin = new SquidexPlugin();

// The content is only available when it is used as a sidebar plugin for single content items.
plugin.onContentChanged(function (content) {
    element.innerHTML = JSON.stringify(content, null, 2);

    grow(element);
});
