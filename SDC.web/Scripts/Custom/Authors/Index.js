$(function () {

    $('#chkBooksFilter').bootstrapSwitch({
        'handleWidth': 75
    });
    $('#chkBooksFilter').on('switchChange.bootstrapSwitch', (function (event, state) {
        if ($(this).is(":checked")) {
            $('#authorsTable').DataTable().ajax.url(_links.fetchAuthorsJson + '?onlyWithBooks=true').load();
        }
        else {
            $('#authorsTable').DataTable().ajax.url(_links.fetchAuthorsJson).load();
        }
        
    }));

    $('#authorsTable').DataTable({
        'paging': true,
        'ajax': _links.fetchAuthorsJson + '?onlyWithBooks=' + $('#chkBooksFilter').is(':checked'),
        'columnDefs': [
            {
                "render": function (data, type, row) {
                    var viewAuthorUrl = _links.viewAuthor + '/' + row[0];
                    return "<a href='" + viewAuthorUrl + "'>" + data + "</a>";
                },
                "targets": 1
            }
        ]
    });
});
