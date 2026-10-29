var element = document.getElementById('editor');

// When the field is instantiated it notifies the UI that it has been loaded.
// 
// Furthermore it sends the current size to the parent.
var field = new SquidexFormField();

// Init is called once with a context that contains the app name, schema name and authentication information.
field.onInit(function (context) {
    element.innerHTML = JSON.stringify(context, null, 2);
});
