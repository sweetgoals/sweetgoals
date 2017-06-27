
$(document).ready(function () {
    $('#registerButton').on('click', function () {
        var dotCount = $('#Email').val().split('.');
        if (dotCount.length > 2)
        {
            alert('To many dots your email is fake!');
            return false;
        }
    });
})