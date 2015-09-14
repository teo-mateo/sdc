$(function () {
    var _m = $('#editBookModal');
    var id = $('#bookId').val();

    prepareBookModal(_m, _editBookModal);

    _m.on('shown.bs.modal', function () {
        //load the book, ajax call to the Book controller
        $.ajax({
            url: _editBookModal.fetchBookUrl,
            type: 'GET',
            data: { id: id },
            error: function (response) {
                toastr.error('error fetching book')
            },
            success: function (response) {
                //save it to the global object
                _editBookModal.bookJson = response;

                //load it into the form controls

                _m.find('.bookTitle').val(response.Title);
                _m.find('.bookYear').val(response.Year);
                _m.find('.bookISBN').val(response.ISBN);
                if (response.Publisher != null) {
                    _m.find('.bookPublisher').val(response.Publisher.Name);
                }

                //select all the book genres
                response.Genres.forEach(function (g) {
                    _m.find(".bookGenres option[value='" + g.Id + "']").attr('selected', 'selected');
                });
                _m.find(".bookGenres").multiselect("refresh");

                //display book authors
                var container = _m.find('.listAuthors');

                //clear authors container
                container[0].innerHTML = '';

                response.Authors.forEach(function (g) {
                    var elem = $('<li></li>');
                    elem.attr('data-auth-id', g.Id);
                    elem.attr('data-auth-name', g.Name);

                    container.append(elem);
                    elem.html("<span>" + g.Name + "</span>");

                    var removeAuthorLink = $("<a href='#' class='glyphicon glyphicon-remove removeAuthor'></a>");
                    removeAuthorLink.click(function () {
                        $(removeAuthorLink).parent().remove();
                    });
                    elem.append(removeAuthorLink);
                });

                _m.find('.bookDescription').val(response.Description);
                _m.find(".bookLanguage option[value='" + response.Language.Code + "']").attr('selected', 'selected');
            }
        });
    });

    //click on the save button
    $('.btnUpdateBook').click(function () {
        _editBookModal.bookJson.AddedDate = changeDateFormat(_editBookModal.bookJson.AddedDate);
        _editBookModal.bookJson.Title = _m.find(".bookTitle").val();
        _editBookModal.bookJson.Year = _m.find(".bookYear").val();
        _editBookModal.bookJson.ISBN = _m.find('.bookISBN').val();
        _editBookModal.bookJson.Description = _m.find('.bookDescription').val();
        _editBookModal.bookJson.Language = {
            "Code": _m.find('.bookLanguage').find(":selected").attr("value"),
            "Name": _m.find('.bookLanguage').find(":selected").val()
        };
        _editBookModal.bookJson.Publisher = {
            "Id": _m.find('.bookPublisher').attr('data-selected-id'),
            "Name": _m.find('.bookPublisher').attr('data-selected-name')
        };

        //gather all authors
        var authors_li = _m.find('.listAuthors').find('li').toArray();
        _editBookModal.bookJson.Authors = authors_li.map(function (li) {
            return {
                "Id": $(li).attr('data-auth-id'),
                "Name": $(li).attr('data-auth-name')
            };
        });

        //and all genres
        var genres = $('.bookGenres option:selected').toArray();
        _editBookModal.bookJson.Genres = genres.map(function (g) {
            return {
                "Id": g.value,
                "Name": g.innerText
            };
        });

        var valid = validateBook(_m, _editBookModal.bookJson);

        if (valid) {
            //send json 
            $.ajax({
                url: _editBookModal.updateBookUrl,
                type: "POST",
                data: JSON.stringify(_editBookModal.bookJson),
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


})