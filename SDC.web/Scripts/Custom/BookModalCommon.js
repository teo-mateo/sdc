﻿function prepareBookModal(_m, urls) {

    //on focus on the search author input, clear data
    _m.find('.searchAuthor').focus(function () {
        _m.find('.searchAuthor').attr('data-selected-id', null);
        _m.find('.searchAuthor').attr('data-selected-name', null);
    });

    //autocomplete for author search
    _m.find('.searchAuthor').autocomplete({
        source: urls.searchAuthorUrl,
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
        source: urls.searchPublisherUrl,
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

    //click on the remove author link
    $('.removeAuthor').click(function () {
        $(this).parent().remove();
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
}

// see http://www.codeproject.com/Articles/272335/JSON-Serialization-and-Deserialization-in-ASP-NET
function changeDateFormat(jsondate) {
    jsondate = jsondate.replace("/Date(", "").replace(")/", "");
    if (jsondate.indexOf("+") > 0) {
        jsondate = jsondate.substring(0, jsondate.indexOf("+"));
    }
    else if (jsondate.indexOf("-") > 0) {
        jsondate = jsondate.substring(0, jsondate.indexOf("-"));
    }

    var date = new Date(parseInt(jsondate, 10));
    var month = date.getMonth() + 1 < 10 ?
       "0" + (date.getMonth() + 1) : date.getMonth() + 1;
    var currentDate = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
    return date.getFullYear() + "-" + month + "-" + currentDate;
}

function prepareJqueryUpload(_m, urls, successCallback) {
    // Initialize the jQuery File Upload widget:
    _m.find('#fileUpload').fileupload({

        // This function is called when a file is added to the queue
        add: function (e, data) {
            //This area will contain file list and progress information.
            var tpl = $('<li class="working">'+
                        '<input type="text" value="0" data-width="48" data-height="48" data-fgColor="#0788a5" data-readOnly="1" data-bgColor="#3e4043" />'+
                        '<p></p><span></span></li>' );

            
            // Append the file name and file size
            tpl.find('p').text(data.files[0].name)
                         .append('<i>' + formatFileSize(data.files[0].size) + '</i>');

            var ul = _m.find("#fileList");

            // Add the HTML to the UL element
            data.context = tpl.appendTo(ul);

            // Initialize the knob plugin. This part can be ignored, if you are showing progress in some other way.
            tpl.find('input').knob();

            // Listen for clicks on the cancel icon
            tpl.find('span').click(function(){
                if(tpl.hasClass('working')){
                    jqXHR.abort();
                }
                tpl.fadeOut(function(){
                    tpl.remove();
                });
            });

            // Automatically upload the file once it is added to the queue
            var jqXHR = data.submit();
        },
        progress: function(e, data){

            // Calculate the completion percentage of the upload
            var progress = parseInt(data.loaded / data.total * 100, 10);

            // Update the hidden input field and trigger a change
            // so that the jQuery knob plugin knows to update the dial
            data.context.find('input').val(progress).change();

            if(progress == 100){
                data.context.removeClass('working');
                successCallback();
            }
        },
        done: function (e, data) {
            alert(data.result);
        }

    });

}

//Helper function for calculation of progress
function formatFileSize(bytes) {
    if (typeof bytes !== 'number') {
        return '';
    }
}
