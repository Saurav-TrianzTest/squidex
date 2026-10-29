var element = document.getElementById('editor');

// When the field is instantiated it notifies the UI that it has been loaded.
// 
// Furthermore it sends the current size to the parent.
var field = new SquidexFormField();

document.getElementById('expanded').addEventListener('click', function () {
    field.toggleExpanded();
});

document.getElementById('fullscreen').addEventListener('click', function () {
    field.toggleFullscreen();
});

field.onValueChanged(function (value) {
    if (value) {
        element.value = JSON.stringify(value);
    } else {
        element.value = '';
    }
});

field.onDisabled(function (disabled) {
    element.disabled = disabled;
});

element.addEventListener('change', function ()  {
    var value = element.value;

    if (value) {
        field.valueChanged(JSON.parse(value));
    } else {
        field.valueChanged(undefined);
    }
});
