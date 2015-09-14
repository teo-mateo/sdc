function validateBook(_m, bookJson) {
    //validate the object
    var valid = true;
    if (bookJson["Title"].length == 0 || bookJson["Title"].length > 100) {
        _m.find('.bookTitle').parent().addClass("has-error");
        valid = false;
    } else {
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

    return valid;
}