var deleteBook = function (id) {
    bootbox.confirm("Are you sure you want to delete this book?", function (result) {
        if (result) {
            $.ajax({
                url: _pageJsObject.deleteBookUrl,
                type: 'POST',
                data: { deleteBookId: id },
                error: function (response) {
                    toastr.error('error deleting book', 'error');
                },
                success: function (response) {
                    location.href = _pageJsObject.redirectUrl
                }
            });
        }
    });
};