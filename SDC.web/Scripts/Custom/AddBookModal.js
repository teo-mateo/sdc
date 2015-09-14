$(function () {
    var _m = $('#newBookModal');

    prepareBookModal(_m, _addBookModal);

    _m.on('shown.bs.modal', function () {
        _m.find('.bookTitle').focus();
    });



    //click on the save button
    $('.btnSaveNewBook').click(function () {

        //construct a json object holding all the book properties
        var bookJson =
        {
            "ShelfId": _m.find(".shelfId").val(),
            "Id": 0,
            "Title": _m.find(".bookTitle").val(),
            "Year": _m.find(".bookYear").val(),
            "Authors": [],
            "Genres": [],
            "Language": {
                "Code": _m.find('.bookLanguage').find(":selected").attr("value"),
                "Name": _m.find('.bookLanguage').find(":selected").val()
            },
            "ISBN": _m.find('.bookISBN').val(),
            "Publisher": {
                "Id": _m.find('.bookPublisher').attr('data-selected-id'),
                "Name": _m.find('.bookPublisher').attr('data-selected-name')
            }, 
            "Description": _m.find('.bookDescription').val()
        };

        //gather all authors
        var authors_li = _m.find('.listAuthors').find('li').toArray();
        bookJson.Authors = authors_li.map(function (li) {
            return {
                "Id": $(li).attr('data-auth-id'),
                "Name": $(li).attr('data-auth-name')
            };
        });

        //and all genres
        var genres = $('.bookGenres option:selected').toArray();
        bookJson.Genres = genres.map(function (g) {
            return {
                "Id": g.value,
                "Name": g.innerText
            };
        });

        var valid = validateBook(_m, bookJson);

        if (valid) {
            //send json 
            $.ajax({
                url: _addBookModal.addBookUrl,
                type: "POST",
                data: JSON.stringify(bookJson),
                contentType: "application/json; charset=utf-8",
                error: function (response) {
                    toastr.error('error saving book', 'error');
                },
                success: function (response) {
                    _m.modal('hide');
                    location.reload();
                }
            });
        }

    });





});