function grow(element) {
    element.style.height = "5px";
    element.style.height = (element.scrollHeight)+"px";
}

var element = document.getElementById('editor');

element.addEventListener('input', function() {
    grow(element);
});

// When the field is instantiated it notifies the UI that it has been loaded.
// 
// Furthermore it sends the current size to the parent.
var plugin = new SquidexPlugin();

// Init is called once with a context that contains the app name, schema name and authentication information.
plugin.onInit(function (context) {
    element.innerHTML = JSON.stringify(context, null, 2);

    grow(element);
});
