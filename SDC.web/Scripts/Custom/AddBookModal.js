$(function () {

    var _m = $('#newBookModal');
    _m.on('shown.bs.modal', function () {
        _m.find('.bookTitle').focus();
    });

    //click on the remove author link
    $('.removeAuthor').click(function () {
        $(this).parent().remove();
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

        var genres = $('.bookGenres option:selected').toArray();
        bookJson.Genres = genres.map(function (g) {
            return {
                "Id": g.value,
                "Name": g.innerText
            };
        });

        //validate the object
        var valid = true;
        if (bookJson["Title"].length == 0 || bookJson["Title"].length > 100) {
            _m.find('.bookTitle').parent().addClass("has-error");
            valid = false;
        } else{
            _m.find('.bookTitle').parent().removeClass("has-error");
        }

        if (bookJson["Authors"].length == 0) {
            _m.find('.searchAuthor').parent().addClass("has-error");
            valid = false;
        } else {
            _m.find('.searchAuthor').parent().removeClass("has-error");
        }


        var yearAsInt = parseInt(bookJson["Year"]);
        if (isNaN(yearAsInt) || (yearAsInt < 1000 || yearAsInt > 2016)) {
            _m.find('.bookYear').parent().addClass("has-error");
            valid = false;
        } else {
            _m.find('.bookYear').parent().removeClass("has-error");
        }

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

    //add author
    _m.find('.btnAddAuthor').click(function () {
        var sbox = _m.find('.searchAuthor');
        var container = _m.find('.listAuthors');

        if (sbox.val().length == 0)
            return false;

        var authId = sbox.attr('data-selected-id');
        var authName = sbox.attr('data-selected-name');
        if (authId == undefined || authName == undefined) {
            authId = 0;
            authName = sbox.val();
        }
        var elem = $('<li></li>');
        elem.attr('data-auth-id', authId);
        elem.attr('data-auth-name', authName);

        container.append(elem);
        elem.html("<span>" + authName + "</span>");

        var removeAuthorLink = $("<a href='#' class='glyphicon glyphicon-remove removeAuthor'></a>");
        removeAuthorLink.click(function () {
            $(removeAuthorLink).parent().remove();
        });
        elem.append(removeAuthorLink);

        sbox.val(null);
        return false;
    });


    //on focus on the search author input, clear data
    _m.find('.searchAuthor').focus(function () {
        _m.find('.searchAuthor').attr('data-selected-id', null);
        _m.find('.searchAuthor').attr('data-selected-name', null);
    });

    //autocomplete for author search
    _m.find('.searchAuthor').autocomplete({
        source: _addBookModal.searchAuthorUrl,
        minLength: 2,
        open: function (event, ui) {
            $(".ui-autocomplete").css("z-index", 1100);
        },
        focus: function (event, ui) {
            _m.find('.searchAuthor').val(ui.item.label);
            return false;
        },
        select: function (event, ui) {
            event.preventDefault();
            _m.find('.searchAuthor').val(ui.item.label);
            _m.find('.searchAuthor').attr('data-selected-id', ui.item.value);
            _m.find('.searchAuthor').attr('data-selected-name', ui.item.label);
        }
    })
    .data("ui-autocomplete")._renderItem = function (ul, item) {
        return $("<li></li>")
            .data("item.autocomplete", item)
            .append("<a href='#'>" + item.label + "</a>")
            .appendTo(ul);
    };


    //autocomplete for new book modal - publisher
    _m.find('.bookPublisher').autocomplete({
        source: _addBookModal.searchPublisherUrl,
        minLength: 2,
        open: function (event, ui) {
            $(".ui-autocomplete").css("z-index", 1100);
        },
        focus: function (event, ui) {
            _m.find('bookPublisher').val(ui.item.label);
            return false;
        },
        select: function (event, ui) {
            event.preventDefault();
            _m.find('.bookPublisher').val(ui.item.label);
            _m.find('.bookPublisher').attr('data-selected-id', ui.item.value);
            _m.find('.bookPublisher').attr('data-selected-name', ui.item.label);
        }
    })
    .data('ui-autocomplete')._renderItem = function (ul, item) {
        return $("<li></li>")
            .data("item.autocomplete", item)
            .append("<a href='#'>" + item.label + "</a>")
            .appendTo(ul);
    };


    //multiselect genres
    _m.find('.bookGenres').multiselect({
        enableCaseInsensitiveFiltering: true,
        maxHeight: 300
    });

});