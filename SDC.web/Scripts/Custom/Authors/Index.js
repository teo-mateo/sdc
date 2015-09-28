$(function () {

    var getJsonUrl = function () {
        var onlyWithBooks = $('#chkBooksFilter').is(':checked');
        return _links.fetchAuthorsJson + "?onlyWithBooks=" + onlyWithBooks;
    }

    //returns the column to sort to, based on the hidden value (it comes from the URL)
    var getSortCol = function () {
        var v = $('#colSortIndex').val();
        return v === '' ? 0 : v;
    }

    //returns the sort direction, based on the hidden value (it comes from the URL)
    var getSortDirection = function () {
        var v = $('#colSortDirection').val();
        return v === '' ? 'asc' : v;
    }

    $('#chkBooksFilter').bootstrapSwitch({
        'handleWidth': 75
    });
    $('#chkBooksFilter').on('switchChange.bootstrapSwitch', (function (event, state) {
        $('#authorsTable').DataTable().ajax.url(getJsonUrl());
        
    }));

    //initialize Datatable
    _links.table = $('#authorsTable').DataTable({
        'paging': true,
        'serverSide': true,
        'processing': true,
        'ajax': getJsonUrl(),
        'columnDefs': [
            {
                //author link
                "render": function (data, type, row) {
                    var viewAuthorUrl = _links.viewAuthor + '/' + row[0];
                    return "<a href='" + viewAuthorUrl + "'>" + data + "</a>";
                },
                "targets": 1
            }, {
                //link to the profile of the user that added the author
                "render": function (data, type, row) {
                    var viewUserProfileUrl = _links.viewUserProfile + '/' + row[4]; 
                    return "<a href='" + viewUserProfileUrl + "'>" + data + "</a>";
                }, targets:4
            }, {
                //quick-action button.
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
        ],
        'order': [[getSortCol(), getSortDirection()]] // use the default sort column and direction 
    });

    //reset sort values to nothing
    $('#colSortIndex').val('');
    $('#colSortDirection').val('');


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

        var sort_col_id = _links.table.order()[0][0];
        var sort_col_order = _links.table.order()[0][1];
        var url=_links.current + "?col=" + sort_col_id + "&ord=" + sort_col_order;
        window.history.pushState("", "Authors", url);
    });
});
