// When the field is instantiated it notifies the UI that it has been loaded.
//
// Furthermore it sends the current size to the parent.
var field = new SquidexFormField();

let selectedContents;

document.getElementById('alert').addEventListener('click', function () {
    field.notifyError('ERROR Text');
});

document.getElementById('info').addEventListener('click', function () {
    field.notifyInfo('INFO Text');
});

document.getElementById('confirm').addEventListener('click', function () {
    field.confirm('CONFIRM Title', 'CONFIRM Text', function (confirmed) {
        console.log('CONFIRMED: ' + confirmed);
    });
});

document.getElementById('pickAssets').addEventListener('click', function () {
    field.pickAssets(function (assets) {
        console.log('ASSETS: ' + JSON.stringify(assets, undefined, 2));
    });
});

document.getElementById('pickContents').addEventListener('click', function () {
    field.pickContents(undefined, function (contents) {
        selectedContents = contents.map(x => x.id);

        console.log('ASSETS: ' + JSON.stringify(contents, undefined, 2));
    }, undefined, selectedContents);
});
