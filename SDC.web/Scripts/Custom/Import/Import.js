var timerId;
function clearImportTimer() {
    if (timerId !== undefined) {
        window.clearInterval(timerId);
        timerId = undefined;
    }
}

$('.importLink').on('click', function () {
    var max = $(this).attr('data-sdc-max');
    importData(max);
});

function importData(max){
    var d = { "Max": max };
    $.ajax({
        url: _jsPageObject.importUrl,
        type: 'POST',
        data: JSON.stringify(d),
        contentType: 'application/json',
        success: function (response) {
            toastr.info('import started.');
            startStatusCheck();
        },
        error: function (response) {
            toastr.error('failed to start import');
        }
    });
}

$('#cancelLink').on('click', function () {
    $.ajax({
        url: _jsPageObject.cancelUrl,
        type: 'GET',
        success: function (response) {
            toastr.info('canceled.');
            clearImportTimer();
        },
        error: function (response) {
            toastr.error('error canceling import');
            clearImportTimer();
        }
    })
});

function startStatusCheck() {
    timerId = window.setInterval(function () {
        $.ajax({
            url: _jsPageObject.statusUrl,
            type: 'GET',
            error: function (response) {
                var html = $('#status').html();
                $('#status').html(html + " Error.");
                toastr.error('error getting status');
                clearImportTimer();
            },
            success: function (response) {
                if (response.running === false) {
                    $('#status').html(html + " Done.");
                    clearImportTimer();
                } else {
                    $('#status').html('Imported ' + response.count + '/' + response.target + ' books in ' + response.time + '.');
                }

            }
        })
    }, 1000);


}