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

    _links.table = $('#authorsTable').DataTable({
        'paging': true,
        'ajax': _links.fetchAuthorsJson + '?onlyWithBooks=' + $('#chkBooksFilter').is(':checked'),
        'columnDefs': [
            {
                "render": function (data, type, row) {
                    var viewAuthorUrl = _links.viewAuthor + '/' + row[0];
                    return "<a href='" + viewAuthorUrl + "'>" + data + "</a>";
                },
                "targets": 1
            }, {
                "render": function (data, type, row) {
                    var singleButton = $('.singleButton');
                    var clone = singleButton.clone();
                    clone.removeClass('singleButton');
                    clone.addClass('authorActions-' + row[0]);
                    clone.attr('data-auth-id', row[0]);
                    clone.attr('data-auth-valid', row[2]);
                    clone.addClass('pull-right');

                    var html = clone.wrap('<p/>').parent().html();
                    return data + '&nbsp;' + html;
                }, targets:5
            }
        ]
    });

    $('#authorsTable').on('draw.dt', function () {
        var c = 1;
        var buttons = $("div[data-auth-id]");
        buttons.each(function (ix) {
            var b = $(this);
            if (b.attr('data-auth-valid') == "False") {
                var approveAuthorUrl = _links.approveAuthor + '/' + b.attr('data-auth-id');
                b.find('.lnkApprove').click(function (args) {
                    $.ajax({
                        url: approveAuthorUrl,
                        type: 'POST',
                        success: function (response) {
                            var row = _links.table.row(b.parent().parent());
                            var rowdata = row.data();
                            rowdata[2] = 'True';
                            row.data(rowdata).draw();
                        },
                        error: function (response) {
                            toastr.error('error approving author');
                        }
                    })
                });
            } else {
                b.find('.lnkApprove').remove();
            }

            b.find('.lnkDelete').click(function (args) {
                var deleteAuthorUrl = _links.deleteAuthor + '/' + b.attr('data-auth-id');
                $.ajax({
                    url: deleteAuthorUrl,
                    type: 'POST',
                    success: function (response) {
                        //just reload for now. 
                        //todo: figure out how to remove just this row. 
                        //idea: remove it from the json and then refresh the table
                        location.reload();
                    },
                    error: function (response) {
                        toastr.error('error deleting author');
                    }
                })
            });
        });
    });
});
